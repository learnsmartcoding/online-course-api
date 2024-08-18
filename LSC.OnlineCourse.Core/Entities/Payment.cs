using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int EnrollmentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public virtual Enrollment Enrollment { get; set; } = null!;
}
