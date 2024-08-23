using LSC.OnlineCourse.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public interface ICourseRepository
    {
        //This will return just list of course models, course high level details for us to show for users to select.
        //this method is alsoble to filter the course based on categoryid
        Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null);

        //this will return detailed particular course .
        Task<CourseDetailModel> GetCourseDetailAsync(int courseId);
    }
}
