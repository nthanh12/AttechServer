using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Consts;
using AttechServer.Applications.UserModules.Abstracts;
using System.Text.Json;

namespace AttechServer.Shared.Middlewares
{
    public class PermissionMiddleWare
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.User.Identity!.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var permissionService = context.RequestServices.GetRequiredService<IPermissionService>();
            var httpMethod = context.Request.Method;
            var endpoint = $"{context.Request.Path.Value!.ToLower()}";
            //Lấy các permission có thể truy cập của api
            var keyPermissionOfApi = permissionService.GetAllPermissionKeyByApiEndpoint(endpoint);
            bool isGranted;
            string? keyPermission = context.Request.Query.FirstOrDefault(q => q.Key.Contains(QueryParamKeys.Permission)).Value;
            var userId = context.User.FindFirst("user_id")?.Value ?? "";
            var userType = context.User.FindFirst("user_type")?.Value ?? "";
            if ((int.Parse(userType!) == UserTypes.ADMIN))
            {
                await _next(context);
            }
            else
            {
                var listPermissionOfUser = permissionService.GetPermissionsByCurrentUserId();
                isGranted = keyPermission is not null ? permissionService!.CheckPermission([keyPermission]) : (keyPermissionOfApi.Count() == 0 || permissionService!.CheckPermission(keyPermissionOfApi));
                if (!(int.Parse(userType!) == UserTypes.ADMIN || int.Parse(userType!) == UserTypes.CUSTOMER && isGranted))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        message = ErrorMessage.UserNotHavePermission
                    }));
                    return;
                }
                await _next(context);
            }
        }
    }
}
