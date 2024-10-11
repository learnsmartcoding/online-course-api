using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReviewModel?>> GetReviewById(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<UserReviewModel>>> GetReviewsByCourseId(int courseId)
        {
            var reviews = await _reviewService.GetReviewsByCourseIdAsync(courseId);
            return Ok(reviews);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserReviewModel>>> GetUserReviews(int userId)
        {
            var reviews = await _reviewService.GetUserReviewsAsync(userId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<ActionResult> AddReview([FromBody] UserReviewModel reviewModel)
        {
            await _reviewService.AddReviewAsync(reviewModel);
            return CreatedAtAction(nameof(GetReviewById), new { id = reviewModel.ReviewId }, reviewModel);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReview(int id, [FromBody] UserReviewModel reviewModel)
        {
            if (id != reviewModel.ReviewId)
            {
                return BadRequest();
            }

            await _reviewService.UpdateReviewAsync(reviewModel);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
    }
}
