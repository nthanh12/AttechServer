using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Consts;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Shared.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;
        private IPermissionService? _permissionService;
        private IHttpContextAccessor? _httpContext;

        public PermissionFilterAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        private void GetServices(AuthorizationFilterContext context)
        {
            _permissionService =
                context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
            _httpContext =
                context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            GetServices(context);
            var userType = _httpContext!.GetCurrentUserLevel();
            if (userType == UserLevels.SYSTEM || userType == UserLevels.MANAGER)
            {
                return;
            }

            bool isGrant = _permissionService!.CheckPermission(_permissions);
            var permissionQueryParam = context
               .HttpContext.Request.Query[QueryParamKeys.Permission]
               .ToString()
               .Trim();
            if (
                !string.IsNullOrEmpty(permissionQueryParam)
                && isGrant
                && !_permissionService!.CheckPermission(permissionQueryParam)
                && _permissions.Contains(permissionQueryParam)
            )
            {
                isGrant = false;
            }

            if (!isGrant)
            {
                context.Result = new UnauthorizedObjectResult(
                    new { message = ErrorMessage.UserNotHavePermission }
                );
            }
        }
    }
}
