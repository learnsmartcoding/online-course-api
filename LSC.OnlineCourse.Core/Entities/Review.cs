using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class Review
{
    public int ReviewId { get; set; }

    public int CourseId { get; set; }

    public int UserId { get; set; }

    public int Rating { get; set; }

    public string? Comments { get; set; }

    public DateTime ReviewDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
}
