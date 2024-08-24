using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public class CourseCategoryRepository : ICourseCategoryRepository
    {
        private readonly OnlineCourseDbContext dbContext;

        public CourseCategoryRepository(OnlineCourseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Task<CourseCategory?> GetById(int id)
        {
           return dbContext.CourseCategories.FindAsync(id).AsTask();
        }

        public Task<List<CourseCategory>> GetCourseCategories()
        {
            return dbContext.CourseCategories.ToListAsync();
        }
    }
}
