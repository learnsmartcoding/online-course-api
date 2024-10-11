using LSC.OnlineCourse.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public interface ICourseRepository
    {   
        Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null);
        Task<CourseDetailModel> GetCourseDetailAsync(int courseId);
        Task AddCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int courseId);
        Task<Course> GetCourseByIdAsync(int courseId);
        void RemoveSessionDetail(SessionDetail sessionDetail);
        Task<List<Instructor>> GetAllInstructorsAsync();

        Task<bool> UpdateCourseThumbnail(string courseThumbnailUrl, int courseId);
    }
}
