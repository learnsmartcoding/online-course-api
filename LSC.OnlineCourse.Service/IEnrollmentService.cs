using AutoMapper;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;

namespace LSC.OnlineCourse.Service
{
    public interface IEnrollmentService
    {
        Task<CourseEnrollmentModel> EnrollCourseAsync(CourseEnrollmentModel enrollmentModel);
        Task<CourseEnrollmentModel> GetEnrollmentAsync(int id);
        Task<List<CourseEnrollmentModel>> GetUserEnrollmentsAsync(int userId);
    }

    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CourseEnrollmentModel> EnrollCourseAsync(CourseEnrollmentModel enrollmentModel)
        {
            var enrollment = _mapper.Map<Enrollment>(enrollmentModel);
            enrollment.EnrollmentDate = DateTime.UtcNow; // Set the enrollment date
            enrollment.PaymentStatus = "Completed"; // Temporary status

            // Create a new payment
            var payment = new Payment
            {
                EnrollmentId = enrollment.EnrollmentId,
                Amount = 0, // Full fee waiver for now
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = enrollmentModel.CoursePaymentModel.PaymentMethod,
                PaymentStatus = "Completed" // Mark as completed
            };

            enrollment.Payments.Add(payment);
            var result = await _repository.AddEnrollmentAsync(enrollment);
            return _mapper.Map<CourseEnrollmentModel>(result);
        }

        public async Task<CourseEnrollmentModel> GetEnrollmentAsync(int id)
        {
            var enrollment = await _repository.GetEnrollmentByIdAsync(id);
            return _mapper.Map<CourseEnrollmentModel>(enrollment);
        }

        public async Task<List<CourseEnrollmentModel>> GetUserEnrollmentsAsync(int userId)
        {
            var enrollments = await _repository.GetEnrollmentByUserIdAsync(userId);
            return _mapper.Map<List<CourseEnrollmentModel>>(enrollments);
        }
    }


}
