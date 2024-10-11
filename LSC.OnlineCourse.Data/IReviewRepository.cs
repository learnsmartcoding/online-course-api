using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LSC.OnlineCourse.Data
{

    public interface IReviewRepository
    {
        Task<Review?> GetReviewByIdAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByCourseIdAsync(int courseId);
        Task<IEnumerable<Review>> GetUserReviewsAsync(int userId);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int reviewId);
    }

    public class ReviewRepository : IReviewRepository
    {
        private readonly OnlineCourseDbContext _context;

        public ReviewRepository(OnlineCourseDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Course)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        public async Task<IEnumerable<Review>> GetReviewsByCourseIdAsync(int courseId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(int userId)
        {
            return await _context.Reviews
                .Include(r => r.Course)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

    }
}