using LSC.OnlineCourse.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Core.Models
{
    public class CourseEnrollmentModel
    {
        public int EnrollmentId { get; set; }

        public int CourseId { get; set; }
        public string? CourseTitle { get; set; } = string.Empty;

        public int UserId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string PaymentStatus { get; set; } = null!;
        public CoursePaymentModel CoursePaymentModel { get; set; }
    }
    public  class CoursePaymentModel
    {
        public int PaymentId { get; set; }

        public int EnrollmentId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;
        
    }

}
