using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Core.Models
{
    public class UserModel
    {
        public int UserId { get; set; }

        public string DisplayName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string AdObjId { get; set; } = null!; 
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public required List<UserRoleModel> UserRoleModel { get; set; }
    }

    public class UserRoleModel
    {
        public int UserRoleId { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public int UserId { get; set; }
    }

    public class RoleModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = null!;
    }

   

}
