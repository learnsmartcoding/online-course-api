using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class SessionDetail
{
    public int SessionId { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? VideoUrl { get; set; }

    public int VideoOrder { get; set; }

    public virtual Course Course { get; set; } = null!;
}
