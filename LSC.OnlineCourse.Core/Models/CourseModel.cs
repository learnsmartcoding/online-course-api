using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Core.Models
{
    /*
     we need to build model for our course related APIs.
    Based on need we need to build, let me show what I have.
     */

    //now I want to build course details, where I need more information specific to a course..
    public class CourseDetailModel: CourseModel // this will bring in all course property
    {
        public List<UserReviewModel> Reviews { get; set; } = new List<UserReviewModel>();

        public List<SessionDetailModel> SessionDetails { get; set; } = new List<SessionDetailModel>();
    }
    //this is to bring user review information
    public class UserReviewModel
    {
        public int CourseId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string? Comments { get; set; }

        public DateTime ReviewDate { get; set; }
    }

    //this brings all sessions for a given course
    public class SessionDetailModel
    {
        public int SessionId { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? VideoUrl { get; set; }

        public int VideoOrder { get; set; }

    }
    public class CourseModel
    {
        public int CourseId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string CourseType { get; set; } = null!;

        public int? SeatsAvailable { get; set; }

        public decimal Duration { get; set; }

        public int CategoryId { get; set; }

        public int InstructorId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public CourseCategoryModel Category { get; set; } = null!; //I want to show category for each course
        public UserRatingModel UserRating { get; set; } // I want to gather avegrage rating for a course

    }
    public class UserRatingModel
    {
        public int CourseId { get; set; }

        public decimal AverageRating { get; set; } //this will hold average rating from N number of users rating/review

        public int TotalRating { get; set; }//count of total user ratings... 
    }
}
