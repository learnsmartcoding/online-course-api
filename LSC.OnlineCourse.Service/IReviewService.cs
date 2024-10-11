using AutoMapper;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public interface IReviewService
    {
        Task<UserReviewModel?> GetReviewByIdAsync(int id);
        Task<IEnumerable<UserReviewModel>> GetReviewsByCourseIdAsync(int courseId);
        Task<IEnumerable<UserReviewModel>> GetUserReviewsAsync(int userId);
        Task AddReviewAsync(UserReviewModel reviewModel);
        Task UpdateReviewAsync(UserReviewModel reviewModel);
        Task DeleteReviewAsync(int reviewId);
    }

    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<UserReviewModel?> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            return _mapper.Map<UserReviewModel>(review);
        }

        public async Task<IEnumerable<UserReviewModel>> GetReviewsByCourseIdAsync(int courseId)
        {
            var reviews = await _reviewRepository.GetReviewsByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<UserReviewModel>>(reviews);
        }

        public async Task<IEnumerable<UserReviewModel>> GetUserReviewsAsync(int userId)
        {
            var reviews = await _reviewRepository.GetUserReviewsAsync(userId);
            return _mapper.Map<IEnumerable<UserReviewModel>>(reviews);
        }

        public async Task AddReviewAsync(UserReviewModel reviewModel)
        {
            var review = _mapper.Map<Review>(reviewModel);
            await _reviewRepository.AddReviewAsync(review);
        }

        public async Task UpdateReviewAsync(UserReviewModel reviewModel)
        {
            var review = _mapper.Map<Review>(reviewModel);
            await _reviewRepository.UpdateReviewAsync(review);
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            await _reviewRepository.DeleteReviewAsync(reviewId);
        }
    }
}
