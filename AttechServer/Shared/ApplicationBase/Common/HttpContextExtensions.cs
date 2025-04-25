using System.Security.Claims;

namespace AttechServer.Shared.AppicationBase.Common
{
    public static class HttpContextExtensions
    {
        private static Claim FindClaim(
            this IHttpContextAccessor httpContextAccessor,
            string claimType
        )
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var test = httpContextAccessor.HttpContext.Request.Headers;
            var claim =
                claims?.FindFirst(claimType)
                ?? throw new InvalidOperationException($"Claim \"{claimType}\" not found.");
            return claim;
        }

        public static int GetCurrentUserType(this IHttpContextAccessor httpContextAccessor)
        {
            var claim = httpContextAccessor.FindClaim("user_type");
            return int.Parse(claim.Value);
        }
        public static int GetCurrentUserId(this IHttpContextAccessor httpContextAccessor)
        {
            var claim = httpContextAccessor.FindClaim("user_id");
            return int.Parse(claim.Value);
        }
    }
}
