using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public interface IUserProfileService
    {
        Task UpdateUserProfilePicture(int userId, string pictureUrl);
        Task UpdateUserBio(int userId, string bio);
        Task<UserModel?> GetUserInfoAsync(int userId);
    }

    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository userProfileRepository;

        public UserProfileService(IUserProfileRepository userProfileRepository)
        {
            this.userProfileRepository = userProfileRepository;
        }

        public async Task UpdateUserProfilePicture(int userId, string pictureUrl)
        {
            await userProfileRepository.UpdateUserProfilePicture(userId, pictureUrl);
        }

        public async Task UpdateUserBio(int userId, string bio)
        {
            await userProfileRepository.UpdateUserBio(userId, bio);
         }

        public Task<UserModel?> GetUserInfoAsync(int userId)
        {
            return userProfileRepository.GetUserInfoAsync(userId);
        }
    }

}
