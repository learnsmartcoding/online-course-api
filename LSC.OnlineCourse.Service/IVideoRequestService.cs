using LSC.OnlineCourse.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public interface IVideoRequestService
    {
        Task<List<VideoRequestModel>> GetAllAsync();
        Task<VideoRequestModel?> GetByIdAsync(int id);
        Task<IEnumerable<VideoRequestModel>> GetByUserIdAsync(int userId);
        Task<VideoRequestModel> CreateAsync(VideoRequestModel model);
        Task<VideoRequestModel> UpdateAsync(int id, VideoRequestModel model);
        Task DeleteAsync(int id);

        Task<VideoModelRequest> SendVideoRequestAckEmail(VideoRequestModel model);
    }
}
