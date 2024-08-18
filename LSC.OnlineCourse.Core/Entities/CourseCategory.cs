using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class CourseCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
