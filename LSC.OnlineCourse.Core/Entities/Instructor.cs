using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class Instructor
{
    public int InstructorId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Bio { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual UserProfile User { get; set; } = null!;
}
