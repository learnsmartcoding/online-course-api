using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LSC.OnlineCourse.API.Chat
{
    //NOTE: When you test, you might not have admin in claims so adjust the logic accordingly or make yourself admin
    // Or contact me
    [Authorize]
    public class ChatHub : Hub
    {
        public ChatHub(IUserProfileService userProfileService)
        {
            this._userProfileService = userProfileService;
        }

        // Stores all connected users
        private static List<ConnectedUser> ConnectedUsers = new List<ConnectedUser>();
        private static List<ConnectedUser> ChatQueue = new List<ConnectedUser>();

        // Admins can view and pick users from the queue
        private static List<ConnectedUser> Admins = new List<ConnectedUser>();
        private static List<MessageHistory> CurrentChatMessages = new List<MessageHistory>();
        private static List<ConnectedUser> CurrentChatMembers = new List<ConnectedUser>();

        private static List<MessageHistory> MessagesToAdmin = new List<MessageHistory>();

        private readonly IUserProfileService _userProfileService;

        public override async Task OnConnectedAsync()
        {
            // Get user identity information
            var identity = Context.User.Identity as ClaimsIdentity;
            var userIdClaim = identity?.Claims.FirstOrDefault(c => c.Type == "extension_userId")?.Value;
            var userId = Convert.ToInt32(userIdClaim);
            var userInfo = await _userProfileService.GetUserInfoAsync(userId);

            // Store user info in memory, including ConnectionId
            var connectedUser = new ConnectedUser
            {
                ConnectionId = Context.ConnectionId,
                UserId = userId,
                DisplayName = userInfo.DisplayName,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                IsAdmin = identity?.Claims.FirstOrDefault(c => c.Type == "extension_userRoles" && c.Value.Contains("Admin")) != null
            };

            if (!ConnectedUsers.Any(connectedUser => connectedUser.ConnectionId == Context.ConnectionId))
            {
                ConnectedUsers.Add(connectedUser);
            }
            await Clients.All.SendAsync("UpdateConnectedUsers", ConnectedUsers);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = ConnectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);

            if (user != null)
            {
                ConnectedUsers.Remove(user);
                if (CurrentChatMembers.Any(a => a.ConnectionId == Context.ConnectionId))
                {
                    ChatQueue.Remove(user); // In case they were in the queue
                    Admins.Remove(user);    // In case they were an admin
                    await EndChat(); // end the chat for all parties
                }
                await Clients.All.SendAsync("UpdateConnectedUsers", ConnectedUsers);
                await Clients.All.SendAsync("UpdateChatQueue", ChatQueue);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Send message between connected user and admin
        public async Task SendMessage(string message)
        {
            var sender = CurrentChatMembers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);//or we can use senderConnectionId
            if (sender != null)
            {
                MessagesToAdmin.Add(new MessageHistory()
                {
                    Message = message,
                    SentDateTime = DateTime.Now,
                    SenderConnectionId = sender.ConnectionId,
                    UserName = sender.DisplayName
                });

                foreach (var user in CurrentChatMembers)
                {
                    await Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", sender.DisplayName, message);

                }
            }
        }

        // User joins the chat queue
        public async Task JoinChatQueue()
        {
            var user = ConnectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);

            if (user != null && !user.IsAdmin)
            {
                if (!ChatQueue.Any(a => a.ConnectionId == Context.ConnectionId))
                {
                    ChatQueue.Add(user);
                }
                await Clients.All.SendAsync("UpdateChatQueue", ChatQueue);
            }
        }

        public async Task EndChat()
        {
            foreach (var user in CurrentChatMembers)
            {
                await Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", "System", "Your chat has been ended");
                await Clients.Client(user.ConnectionId).SendAsync("EndChat", user);
            }
            CurrentChatMembers.Clear();
            //save chat conversation CurrentChatMessages to database // TODO
            CurrentChatMessages.Clear();
        }

        // Admin picks a user to chat with
        public async Task ConnectWithUser(string connectionId)
        {
            var admin = Admins.FirstOrDefault(a => a.ConnectionId == Context.ConnectionId);
            var user = ChatQueue.FirstOrDefault(u => u.ConnectionId == connectionId);

            if (admin != null && user != null)
            {
                // Notify both admin and user about the chat
                await Clients.Client(admin.ConnectionId).SendAsync("StartChat", user);
                await Clients.Client(user.ConnectionId).SendAsync("StartChat", admin);

                //Add to current chat member list
                if (!CurrentChatMembers.Any(a => a.ConnectionId == user.ConnectionId))
                {
                    CurrentChatMembers.Add(user);
                }

                if (!CurrentChatMembers.Any(a => a.ConnectionId == admin.ConnectionId))
                {
                    CurrentChatMembers.Add(admin);
                }

                //initial message
                var initialMessage = new MessageHistory() { Message = $"Chat started with Admin and {user.DisplayName}", SentDateTime = DateTime.Now, SenderConnectionId = admin.ConnectionId };
                CurrentChatMessages.Add(initialMessage);

                //Send to both Admin and user
                await Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", "System", initialMessage.Message);
                await Clients.Client(admin.ConnectionId).SendAsync("ReceiveMessage", "System", initialMessage.Message);

                // Remove user from chat queue after connection
                ChatQueue.Remove(user);
                await Clients.All.SendAsync("UpdateChatQueue", ChatQueue);
            }
        }

        // This would be called when an admin connects
        public async Task RegisterAdmin()
        {
            var admin = ConnectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (admin != null && !Admins.Any(a => a.ConnectionId == admin.ConnectionId))
            {
                admin.IsAdmin = true;
                Admins.Add(admin);
                await Clients.Client(admin.ConnectionId).SendAsync("AdminRegistered", admin);

                //send all admin messages as he was reconnected
                MessagesToAdmin.OrderBy(o => o.SentDateTime).ToList().ForEach(async m =>
                {
                    await Clients.Client(admin.ConnectionId).SendAsync("SendMessageToAdmin", m.UserName, m.Message);
                });
            }
        }

        // Send message between connected user and admin
        public async Task SendMessageToAdmin(string message)
        {
            var sender = ConnectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);//or we can use senderConnectionId
            //var admin = Admins.FirstOrDefault();
            if (sender != null && Admins.Any())
            {
                Admins.ForEach(async admin =>
                {
                    await Clients.Client(admin.ConnectionId).SendAsync("SendMessageToAdmin", sender.DisplayName, message);
                });

                await Clients.Client(sender.ConnectionId).SendAsync("SendMessageToAdmin", sender.DisplayName + " (You)", message);
            }
        }

        // Class to hold connected user details
        private class ConnectedUser
        {
            public string ConnectionId { get; set; }
            public int UserId { get; set; }
            public string DisplayName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool IsAdmin { get; set; }
        }

        private class MessageHistory
        {
            public string? SenderConnectionId { get; set; }
            public string? Message { get; set; }
            public DateTime SentDateTime { get; set; } = DateTime.Now;
            public string? UserName { get; set; } = string.Empty;

        }
    }
}
