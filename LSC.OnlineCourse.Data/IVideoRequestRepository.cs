using LSC.OnlineCourse.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public interface IVideoRequestRepository
    {
        Task<IEnumerable<VideoRequest>> GetAllAsync();
        Task<VideoRequest?> GetByIdAsync(int id);
        Task<IEnumerable<VideoRequest>> GetByUserIdAsync(int userId);
        Task<VideoRequest> AddAsync(VideoRequest videoRequest);
        Task<VideoRequest> UpdateAsync(VideoRequest videoRequest);
        Task DeleteAsync(int id);
    }
}
