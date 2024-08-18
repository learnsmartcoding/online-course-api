using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class UserProfile
{
    public int UserId { get; set; }

    public string DisplayName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string AdObjId { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
