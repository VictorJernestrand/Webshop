using System.Linq;
using System.Security.Claims;

namespace Webshop.Services
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get user Id from logged in user
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static int UserId(this ClaimsPrincipal claims)
        {
            return int.Parse(claims.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value);
        }

        /// <summary>
        /// Get user email from logged in user
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static string UserEmail(this ClaimsPrincipal claims)
        {
            return claims.Claims.FirstOrDefault(x => x.Type == "UserEmail")?.Value;
        }
    }
}
