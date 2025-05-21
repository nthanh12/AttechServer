using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Consts;
using AttechServer.Applications.UserModules.Abstracts;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace AttechServer.Shared.Middlewares
{
    public class PermissionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionMiddleWare> _logger;
        private readonly IMemoryCache _cache;
        private const string PERMISSION_CHECK_CACHE_KEY = "permission_check_{0}_{1}_{2}";
        private const int CACHE_DURATION_MINUTES = 5;

        public PermissionMiddleWare(
            RequestDelegate next,
            ILogger<PermissionMiddleWare> logger,
            IMemoryCache cache)
        {
            _next = next;
            _logger = logger;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (!context.User.Identity!.IsAuthenticated)
                {
                    await _next(context);
                    return;
                }

                var userId = GetUserId(context);
                var userType = GetUserType(context);

                if (userType == UserTypes.ADMIN)
                {
                    await _next(context);
                    return;
                }

                var path = context.Request.Path.Value?.ToLower() ?? "";
                var method = context.Request.Method.ToUpper();
                var permissionKey = GetPermissionKeyFromQuery(context);

                if (await CheckPermission(context, userId, path, method, permissionKey))
                {
                    await _next(context);
                }
                else
                {
                    await ReturnUnauthorizedResponse(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking permission");
                await ReturnUnauthorizedResponse(context);
            }
        }

        private int GetUserId(HttpContext context)
        {
            var userIdClaim = context.User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID");
            }
            return userId;
        }

        private int GetUserType(HttpContext context)
        {
            var userTypeClaim = context.User.FindFirst("user_type")?.Value;
            if (string.IsNullOrEmpty(userTypeClaim) || !int.TryParse(userTypeClaim, out int userType))
            {
                throw new UnauthorizedAccessException("Invalid user type");
            }
            return userType;
        }

        private string? GetPermissionKeyFromQuery(HttpContext context)
        {
            return context.Request.Query
                .FirstOrDefault(q => q.Key.Contains(QueryParamKeys.Permission))
                .Value;
        }

        private async Task<bool> CheckPermission(
            HttpContext context,
            int userId,
            string path,
            string method,
            string? permissionKey)
        {
            var apiEndpointService = context.RequestServices.GetRequiredService<IApiEndpointService>();
            var permissionService = context.RequestServices.GetRequiredService<IPermissionService>();

            var cacheKey = string.Format(PERMISSION_CHECK_CACHE_KEY, userId, path, method);

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);

                // Check API endpoint permission
                var hasApiPermission = await apiEndpointService.CheckApiPermission(path, method, userId);
                if (!hasApiPermission)
                {
                    return false;
                }

                // If specific permission key is provided, check that permission
                if (!string.IsNullOrEmpty(permissionKey))
                {
                    return permissionService.CheckPermission(permissionKey);
                }

                // Check all permissions required for this API endpoint
                var apiPermissions = permissionService.GetAllPermissionKeyByApiEndpoint(path);
                return apiPermissions.Length == 0 || permissionService.CheckPermission(apiPermissions);
            });
        }

        private async Task ReturnUnauthorizedResponse(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                message = ErrorMessage.UserNotHavePermission
            }));
        }
    }
}
