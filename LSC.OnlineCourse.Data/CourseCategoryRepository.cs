using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LSC.OnlineCourse.Data
{
    /*
    //All these are called syn methods, we will learn about async methods.
    //this is primary constructor and available from .net 8
    public class CourseCategoryRepository(OnlineCourseDbContext dbContext) : ICourseCategoryRepository
    {
        private readonly OnlineCourseDbContext dbContext = dbContext;

        //this means if a record not present it will return null, nulable return type
        public CourseCategory? GetById(int id)
        {
            //find is the sync method will try to find record with its primary key
            var data = dbContext.CourseCategories.Find(id); 
            return data;
        }

        public List<CourseCategory> GetCourseCategories()
        {
            var data = dbContext.CourseCategories.ToList();
            return data;
        }
    }

    */

    public class CourseCategoryRepository : ICourseCategoryRepository
    {
        private readonly OnlineCourseDbContext dbContext;

        public CourseCategoryRepository(OnlineCourseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Task<CourseCategory?> GetByIdAsync(int id)
        {
            //as long as we dont nee data immediately, we can return task itself. we will see more example
            var data = dbContext.CourseCategories.FindAsync(id).AsTask();
            return data;
        }

        public Task<List<CourseCategory>> GetCourseCategoriesAsync()
        {
            var data = dbContext.CourseCategories.ToListAsync();
            return data;
        }
    }
}
