using LSC.OnlineCourse.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public interface ICourseCategoryRepository
    {
        Task<CourseCategory?> GetById(int id);
        Task<List<CourseCategory>> GetCourseCategories();
    }
}
