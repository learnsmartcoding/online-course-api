using System.Security.Claims;

namespace LSC.OnlineCourse.API.Common
{
    public interface IUserClaims
    {
        string GetCurrentUserEmail();
        string GetCurrentUserId();
        List<string> GetUserRoles();
        int GetUserId();
    }
    public class UserClaims : IUserClaims
    {
        public UserClaims(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public string GetCurrentContextUserId()
        {
            return GetCurrentUserId();
        }
        private string GetClaimInfo(string property)
        {
            var propertyData = "";
            var identity = HttpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                // or
                propertyData = identity.Claims.FirstOrDefault(d => d.Type.Contains(property))?.Value;

            }
            return propertyData;
        }
       
        public string GetCurrentUserEmail()
        {
            return GetClaimInfo("emails");
        }

        public string GetCurrentUserId()
        {            
            return GetClaimInfo("objectidentifier");
        }
        public List<string> GetUserRoles()
        {
            var roles = GetClaimInfo("extension_userRoles"); ;
            return string.IsNullOrEmpty(roles)?new List<string>(): roles.Split(',').ToList();
        }

        public int GetUserId()
        {
            var userId = GetClaimInfo("extension_userId");
            return Convert.ToInt32(userId);
        }
    }
}
