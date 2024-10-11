using AutoMapper;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
   
    public interface ICourseService
    {       
        Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null);
        Task<CourseDetailModel> GetCourseDetailAsync(int courseId);
        Task AddCourseAsync(CourseDetailModel courseModel);
        Task UpdateCourseAsync(CourseDetailModel courseModel);
        Task DeleteCourseAsync(int courseId);
        Task<List<InstructorModel>> GetAllInstructorsAsync();
        Task<bool> UpdateCourseThumbnail(string courseThumbnailUrl, int courseId);
    }
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository courseRepository;
        private readonly IMapper mapper;

        public CourseService(ICourseRepository courseRepository, IMapper mapper)
        {
            this.courseRepository = courseRepository;
            this.mapper = mapper;
        }
        
        public Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null)
        {
            return courseRepository.GetAllCoursesAsync(categoryId);
        }


        public Task<CourseDetailModel> GetCourseDetailAsync(int courseId)
        {
            return courseRepository.GetCourseDetailAsync(courseId);
        }

        public async Task AddCourseAsync(CourseDetailModel model)
        {
            // Map the CourseModel to the Course entity
            var courseEntity = new Course
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                CourseType = model.CourseType,
                SeatsAvailable = model.SeatsAvailable,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                InstructorId = model.InstructorId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                SessionDetails = model.SessionDetails.Select(session => new SessionDetail
                {
                    Title = session.Title,
                    Description = session.Description,
                    VideoUrl = session.VideoUrl,
                    VideoOrder = session.VideoOrder
                }).ToList()
            };

            await courseRepository.AddCourseAsync(courseEntity);
        }

        public async Task UpdateCourseAsync(CourseDetailModel courseModel)
        {
            var course = await courseRepository.GetCourseByIdAsync(courseModel.CourseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }

            // Update course fields
            course.Title = courseModel.Title;
            course.Description = courseModel.Description;
            course.Price = courseModel.Price;
            course.CourseType = courseModel.CourseType;
            course.SeatsAvailable = courseModel.SeatsAvailable;
            course.Duration = courseModel.Duration;
            course.CategoryId = courseModel.CategoryId;
            course.InstructorId = courseModel.InstructorId;
            course.StartDate = courseModel.StartDate;
            course.EndDate = courseModel.EndDate;

            // Handle session details update
            // Remove any session details that were removed in the updated model
            var existingSessionIds = course.SessionDetails.Select(s => s.SessionId).ToList();
            var updatedSessionIds = courseModel.SessionDetails.Select(s => s.SessionId).ToList();

            // Remove sessions that are not in the updated list
            var sessionsToRemove = course.SessionDetails.Where(s => !updatedSessionIds.Contains(s.SessionId)).ToList();
            foreach (var session in sessionsToRemove)
            {
                course.SessionDetails.Remove(session);
                courseRepository.RemoveSessionDetail(session); // This removes the session from the database
            }

            // Update or add session details
            foreach (var sessionModel in courseModel.SessionDetails)
            {
                var existingSession = course.SessionDetails.FirstOrDefault(s => s.SessionId == sessionModel.SessionId);
                if (existingSession != null)
                {
                    // Update existing session details
                    existingSession.Title = sessionModel.Title;
                    existingSession.Description = sessionModel.Description;
                    existingSession.VideoUrl = sessionModel.VideoUrl;
                    existingSession.VideoOrder = sessionModel.VideoOrder;
                }
                else
                {
                    // Add new session details
                    var newSession = new SessionDetail
                    {
                        Title = sessionModel.Title,
                        Description = sessionModel.Description,
                        VideoUrl = sessionModel.VideoUrl,
                        VideoOrder = sessionModel.VideoOrder,
                        CourseId = course.CourseId
                    };
                    course.SessionDetails.Add(newSession);
                }
            }

            // Call repository to update the course along with its session details
            await courseRepository.UpdateCourseAsync(course);
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            await courseRepository.DeleteCourseAsync(courseId);
        }

        public async Task<List<InstructorModel>> GetAllInstructorsAsync()
        {
            var instructors = await courseRepository.GetAllInstructorsAsync();
            return mapper.Map<List<InstructorModel>>(instructors);
        }

        public Task<bool> UpdateCourseThumbnail(string courseThumbnailUrl, int courseId)
        {
            return courseRepository.UpdateCourseThumbnail(courseThumbnailUrl, courseId);
        }
    }
}
