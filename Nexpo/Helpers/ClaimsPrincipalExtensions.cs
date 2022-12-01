using Nexpo.Controllers;
using Nexpo.Models;
using System;
using System.Security.Claims;

namespace Nexpo.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetId(this ClaimsPrincipal claimsPrincipal)
        {
            return Convert.ToInt32(claimsPrincipal.FindFirstValue(UserClaims.Id));
        }

        public static Role GetRole(this ClaimsPrincipal claimsPrincipal)
        {
            var roleString = claimsPrincipal.FindFirstValue(UserClaims.Role);
            Enum.TryParse<Role>(roleString, out Role role);
            return role;
        }

        public static int? GetStudentId(this ClaimsPrincipal claimsPrincipal)
        {
            var studentId = Convert.ToInt32(claimsPrincipal.FindFirstValue(UserClaims.StudentId));
            return studentId != 0 ? studentId : null;
        }

        public static int? GetCompanyId(this ClaimsPrincipal claimsPrincipal)
        {
            var companyId = Convert.ToInt32(claimsPrincipal.FindFirstValue(UserClaims.CompanyId));
            return companyId != 0 ? companyId : null;
        }
    }
}
