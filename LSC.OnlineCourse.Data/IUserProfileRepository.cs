using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public interface IUserProfileRepository
    {
        Task UpdateUserProfilePicture(int userId, string pictureUrl);
        Task UpdateUserBio(int userId, string bio);
        Task<UserModel?> GetUserInfoAsync(int userId);
    }

    public class UserProfileRepository: IUserProfileRepository
    {
        private readonly OnlineCourseDbContext _context;

        public UserProfileRepository(OnlineCourseDbContext context)
        {
            _context = context;
        }

        public async Task UpdateUserProfilePicture(int userId, string pictureUrl)
        {
            var user = await _context.UserProfiles.FindAsync(userId);
            if (user != null)
            {
                user.ProfilePictureUrl = pictureUrl;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserBio(int userId, string bio)
        {
            var user = await _context.Instructors.FirstOrDefaultAsync(f=>f.UserId==userId);
            if (user != null)
            {
                user.Bio = bio;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserModel?> GetUserInfoAsync(int userId)
        {
            var user = await  _context.UserProfiles.Include(u => u.Instructors).FirstOrDefaultAsync(f => f.UserId == userId);
            if (user != null)
            {
                
                var userInfo =  new UserModel()
                {
                    UserId = user.UserId,
                    Bio = "",
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    UserRoleModel = new List<UserRoleModel>()
                };
                if (user.Instructors.Any())
                {
                    userInfo.Bio = user.Instructors.FirstOrDefault().Bio;
                }
                return userInfo;
            }

            return null;
        }
    }
}
