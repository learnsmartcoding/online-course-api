using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    //this is the place we need to perform business logic, since our app is small we might not see business logic immediately, w
    //as we grow I will show how it is done in service layer
    public interface ICourseCategoryService
    {
        Task<CourseCategoryModel?> GetByIdAsync(int id);
        Task<List<CourseCategoryModel>> GetCourseCategories();
    }

    //when you create always create these files in separate file.

    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly ICourseCategoryRepository categoryRepository;

        public CourseCategoryService(ICourseCategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        
        public async Task<CourseCategoryModel?> GetByIdAsync(int id)
        {
            //When we need data, we need to await for the call to get data
            // we cant return as our return model is different than entity model

            // This await keyword tells compiler to pause the execution untill data is retireved
            var data = await categoryRepository.GetByIdAsync(id);
            return data==null? null : new CourseCategoryModel()
            {
                CategoryId = data.CategoryId,
                CategoryName = data.CategoryName,
                Description = data.Description
            };
        }

        public async Task<List<CourseCategoryModel>> GetCourseCategories()
        {
            var data = await categoryRepository.GetCourseCategoriesAsync();
            var modelData = data.Select(s=>new CourseCategoryModel()
            {
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName,
                Description = s.Description

            }).ToList();

            return modelData;
        }
    }
}
