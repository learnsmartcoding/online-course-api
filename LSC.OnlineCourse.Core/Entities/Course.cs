using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class Course
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
    public string? Thumbnail { get; set; }

    public virtual CourseCategory Category { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<SessionDetail> SessionDetails { get; set; } = new List<SessionDetail>();
}
