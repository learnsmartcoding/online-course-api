using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int CourseId { get; set; }

    public int UserId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual UserProfile User { get; set; } = null!;
}
