using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace LSC.OnlineCourse.API.Common
{

    namespace LSC.OnlineCourse.API.Common
    {
        public class AdminRoleAttribute : Attribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var userClaims = context.HttpContext.RequestServices.GetService<IUserClaims>();
                var userRoles = userClaims?.GetUserRoles();
                if (userRoles == null || !userRoles.Contains("Admin"))
                {
                    // Return 403 Forbidden if the user does not have the Admin role
                    context.Result = new ForbidResult();
                }
            }
        }
    }

}
