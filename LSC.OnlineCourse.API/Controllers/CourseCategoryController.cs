using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LSC.OnlineCourse.API.Controllers
{
    //this means, prefix 'api/controllername'/actionmethod
    [Route("api/[controller]")]
    [ApiController]
    public class CourseCategoryController : ControllerBase
    {
        private readonly ICourseCategoryService categoryService;

        public CourseCategoryController(ICourseCategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            //when you use .Result for an async method, it will become sync method, do not do that
            //var category = categoryService.GetByIdAsync(id).Result;
            var category = await categoryService.GetByIdAsync(id);

            //what if the id is not present?
            if(category == null)
            {
                return NotFound();
            }

            return Ok(category);// ok means it returns 200 status code

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await categoryService.GetCourseCategories();
            // always when a list is returned without any specific reousrce, NotFound not applicable, 
            //that is why we dont check this list is empty or not, if it is empty its will be empty array returned
            return Ok(categories);
        }
    }
}
