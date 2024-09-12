using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class VideoRequest
{
    public int VideoRequestId { get; set; }

    public int UserId { get; set; }

    public string Topic { get; set; } = null!;

    public string SubTopic { get; set; } = null!;

    public string RequestStatus { get; set; } = null!;

    public string ShortTitle { get; set; } = null!;

    public string RequestDescription { get; set; } = null!;

    public string? Response { get; set; }

    public string? VideoUrls { get; set; }

    public virtual UserProfile User { get; set; } = null!;
}
