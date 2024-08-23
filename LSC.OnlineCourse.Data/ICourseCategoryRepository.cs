using LSC.OnlineCourse.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    /*
    public interface ICourseCategoryRepository
    {
        CourseCategory? GetById(int id);
        List<CourseCategory> GetCourseCategories();
    }
    */

    public interface ICourseCategoryRepository
    {
        //Async methods will always return Task<T> and its method named as Async at suffix
        Task<CourseCategory?> GetByIdAsync(int id);
        Task<List<CourseCategory>> GetCourseCategoriesAsync();
    }
}
