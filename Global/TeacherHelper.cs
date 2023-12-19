using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Munkanaplo2.Global
{
    public static class TeacherHelper
    {
        public static bool IsTeacher(string userName)
        {
            if (userName.Contains(" [Tanár]")) return true;
            else return false;
        }
        public static bool IsTeacher(ClaimsPrincipal user)
        {
            if (user.Identity.Name.ToString().Contains(" [Tanár]")) return true;
            else return false;
        }
    }
}