using AutoMapper;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;

namespace LSC.OnlineCourse.API.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VideoRequest, VideoRequestModel>()
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName}, {src.User.LastName}"));

            CreateMap<VideoRequestModel, VideoRequest>()
                .ForMember(dest => dest.User, opt => opt.Ignore()); // We don't map User here since it's handled separately

            CreateMap<CourseEnrollmentModel, Enrollment>();
            CreateMap<Enrollment, CourseEnrollmentModel>()
    .ForMember(dest => dest.CoursePaymentModel,
        opt => opt.MapFrom(src =>
            src.Payments.OrderByDescending(o => o.PaymentDate).FirstOrDefault()))
    .ForMember(dest => dest.CourseTitle,
        opt => opt.MapFrom(src => src.Course.Title));  // Mapping for CourseTitle


            CreateMap<CoursePaymentModel, Payment>();
            CreateMap<Payment, CoursePaymentModel>();

            CreateMap<Review, UserReviewModel>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.LastName}, {src.User.FirstName}"));

            CreateMap<UserReviewModel, Review>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore());

            CreateMap<InstructorModel, Instructor>();
            CreateMap< Instructor, InstructorModel>();
        }
    }
}
