using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]   
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentController(IEnrollmentService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseEnrollmentModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EnrollCourse([FromBody] CourseEnrollmentModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid enrollment data.");
            }

            var enrollment = await _service.GetUserEnrollmentsAsync(model.UserId);
            if (enrollment != null && enrollment.FirstOrDefault(f => f.CourseId == model.CourseId) != null)
            {
                return BadRequest("Enrollment to this course is already exists!");
            }
            var enrolledCourse = await _service.EnrollCourseAsync(model);
            return Ok(enrolledCourse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseEnrollmentModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            var enrollment = await _service.GetEnrollmentAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return Ok(enrollment);
        }

        [HttpGet("user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CourseEnrollmentModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserEnrollments(int id)
        {
            var enrollment = await _service.GetUserEnrollmentsAsync(id);

            return Ok(enrollment);
        }
    }

}
