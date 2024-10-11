using LSC.OnlineCourse.API.Common.LSC.OnlineCourse.API.Common;
using LSC.OnlineCourse.API.Model;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;

namespace LSC.OnlineCourse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]    
    public class CourseController : ControllerBase
    {
        private readonly ICourseService courseService;
        private readonly IAzureBlobStorageService blobStorageService;

        public CourseController(ICourseService courseService, IAzureBlobStorageService  blobStorageService)
        {
            this.courseService = courseService;
            this.blobStorageService = blobStorageService;
        }

        //These 3 get methods are publicly available from our UI, no need to authenticate!
        // GET: api/Course
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<CourseModel>>> GetAllCoursesAsync()
        {
            var courses = await courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // GET: api/Course/ByCategory?categoryId=1
        [HttpGet("Category/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CourseModel>>> GetAllCoursesByCategoryIdAsync([FromRoute] int categoryId)
        {
            var courses = await courseService.GetAllCoursesAsync(categoryId);
            return Ok(courses);
        }

        // GET: api/Course/Detail/5
        [HttpGet("Detail/{courseId}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseDetailModel>> GetCourseDetailAsync(int courseId)
        {
            var courseDetail = await courseService.GetCourseDetailAsync(courseId);
            if (courseDetail == null)
            {
                return NotFound();
            }
            return Ok(courseDetail);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await courseService.GetCourseDetailAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpPost]
        [Authorize]
        [AdminRole]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> AddCourse([FromBody] CourseDetailModel courseModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await courseService.AddCourseAsync(courseModel);
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize]
        [AdminRole]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDetailModel courseModel)
        {
            if (id != courseModel.CourseId)
            {
                return BadRequest("Course ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await courseService.UpdateCourseAsync(courseModel);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [AdminRole]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            await courseService.DeleteCourseAsync(id);
            return NoContent();
        }

        [HttpGet("Instructors")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<List<InstructorModel>>> GetInstructors()
        {
            var instructors = await courseService.GetAllInstructorsAsync();
            return Ok(instructors);
        }

        [HttpPost("upload-thumbnail")]
        [Authorize]
        [AdminRole]
        public async Task<IActionResult> UploadThumbnail(IFormFile file)
        {
            var courseId = Convert.ToInt32(Request.Form["courseId"]);
            string thumbnailUrl = null;
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var course = await courseService.GetCourseDetailAsync(courseId);
            if (course == null)
                return NotFound("Course not found");

            if (file != null)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    // Upload the byte array or stream to Azure Blob Storage
                    thumbnailUrl = await blobStorageService.UploadAsync(
                        stream.ToArray(), $"{courseId}_{course.Title.Trim().Replace(' ', '_')}.{file.FileName.Split('.').LastOrDefault()}", "course-preview");
                }

                // Update the profile picture URL in the database
                await courseService.UpdateCourseThumbnail(thumbnailUrl, courseId);
            }



            return Ok(new { message = "Thumbnail uploaded successfully", thumbnailUrl });
        }
    }

}
