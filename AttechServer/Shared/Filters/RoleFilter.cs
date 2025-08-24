using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Consts;
using AttechServer.Shared.ApplicationBase.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Shared.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int _requiredRoleLevel;
        private IHttpContextAccessor? _httpContext;

        public RoleFilterAttribute(int requiredRoleLevel = 3) // Default to Editor (3)
        {
            _requiredRoleLevel = requiredRoleLevel;
        }

        private void GetServices(AuthorizationFilterContext context)
        {
            _httpContext = context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            GetServices(context);
            
            var userRoleId = _httpContext!.GetCurrentUserLevel(); // Still using GetCurrentUserLevel for compatibility
            
            // Role hierarchy: 1=superadmin, 2=admin, 3=editor (lower number = higher permission)
            if (userRoleId <= _requiredRoleLevel)
            {
                return; // Access granted
            }

            // Access denied
            context.Result = new UnauthorizedObjectResult(
                new { message = ErrorMessage.UserNotHavePermission }
            );
        }
    }
}