using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Consts;
using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ApiEndpoint;
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
                // Check if the endpoint has [AllowAnonymous] attribute
                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>() != null)
                {
                    await _next(context);
                    return;
                }

                if (!context.User.Identity!.IsAuthenticated)
                {
                    await _next(context);
                    return;
                }

                var userId = GetUserId(context);
                var userLevel = GetUserLevel(context);
                
                _logger.LogInformation($"PermissionMiddleware: userId={userId}, userLevel={userLevel}, path={context.Request.Path}");

                if (userLevel == UserLevels.SYSTEM || userLevel == UserLevels.MANAGER)
                {
                    _logger.LogInformation($"Bypassing permission check for userLevel={userLevel}");
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

        private int GetUserLevel(HttpContext context)
        {
            var userLevelClaim = context.User.FindFirst("user_level")?.Value;
            if (string.IsNullOrEmpty(userLevelClaim) || !int.TryParse(userLevelClaim, out int userLevel))
            {
                throw new UnauthorizedAccessException("Invalid user level");
            }
            return userLevel;
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

            // Get user permissions from JWT token
            var userPermissions = context.User.FindAll("permission").Select(c => c.Value).ToHashSet();
            _logger.LogInformation($"User {userId} has {userPermissions.Count} permissions in JWT token");

            var cacheKey = string.Format(PERMISSION_CHECK_CACHE_KEY, userId, path, method);

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);

                try
                {
                    // 1. If specific permission key is provided via query param, check that specific permission
                    if (!string.IsNullOrEmpty(permissionKey))
                    {
                        var hasSpecificPermission = userPermissions.Contains(permissionKey);
                        _logger.LogInformation($"Specific permission check: {permissionKey} = {hasSpecificPermission}");
                        return hasSpecificPermission;
                    }

                    // 2. Get API endpoint configuration
                    var apiEndpoint = await GetApiEndpointConfiguration(apiEndpointService, path, method);
                    
                    if (apiEndpoint == null)
                    {
                        _logger.LogWarning($"API endpoint not found: {method} {path} - Denying access");
                        return false;
                    }

                    // 3. If endpoint doesn't require authentication, allow access
                    if (!apiEndpoint.RequireAuthentication)
                    {
                        _logger.LogInformation($"Endpoint {path} doesn't require authentication - Allowing access");
                        return true;
                    }

                    // 4. Get required permissions for this endpoint
                    var requiredPermissions = await GetRequiredPermissionsForEndpoint(permissionService, path);
                    
                    if (!requiredPermissions.Any())
                    {
                        _logger.LogInformation($"No specific permissions required for {path} - Allowing authenticated user");
                        return true;
                    }

                    // 5. Check if user has at least one required permission
                    var hasRequiredPermission = requiredPermissions.Any(rp => userPermissions.Contains(rp));
                    
                    _logger.LogInformation($"Permission check for {path}: Required={string.Join(",", requiredPermissions)}, HasAccess={hasRequiredPermission}");
                    
                    return hasRequiredPermission;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during permission check for {path}");
                    return false; // Fail secure - deny access on error
                }
            });
        }

        private async Task<ApiEndpointDto?> GetApiEndpointConfiguration(IApiEndpointService apiEndpointService, string path, string method)
        {
            try
            {
                // Get all API endpoints from service
                var allEndpoints = await apiEndpointService.FindAll();
                
                // Normalize path for comparison (remove leading slash and convert to lowercase)
                var normalizedPath = path.TrimStart('/').ToLower();
                
                // Find exact matching endpoint
                var endpoint = allEndpoints.FirstOrDefault(e => 
                    e.Path.TrimStart('/').ToLower() == normalizedPath && 
                    e.HttpMethod.ToUpper() == method.ToUpper());
                
                if (endpoint != null)
                {
                    return endpoint;
                }

                // If exact match not found, try pattern matching for parameterized routes
                foreach (var e in allEndpoints.Where(ep => ep.HttpMethod.ToUpper() == method.ToUpper()))
                {
                    if (IsPathMatch(normalizedPath, e.Path.TrimStart('/').ToLower()))
                    {
                        return e;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting API endpoint configuration for {path} {method}");
                return null;
            }
        }

        private bool IsPathMatch(string requestPath, string endpointPath)
        {
            // Simple pattern matching for routes like "api/users/{id}" vs "api/users/123"
            var requestSegments = requestPath.Split('/');
            var endpointSegments = endpointPath.Split('/');

            if (requestSegments.Length != endpointSegments.Length)
                return false;

            for (int i = 0; i < requestSegments.Length; i++)
            {
                if (endpointSegments[i].StartsWith("{") && endpointSegments[i].EndsWith("}"))
                {
                    // This is a parameter segment, skip comparison
                    continue;
                }

                if (!string.Equals(requestSegments[i], endpointSegments[i], StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<List<string>> GetRequiredPermissionsForEndpoint(IPermissionService permissionService, string path)
        {
            try
            {
                var permissions = permissionService.GetAllPermissionKeyByApiEndpoint(path);
                return permissions?.ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting required permissions for endpoint {path}");
                return new List<string>();
            }
        }


        private async Task ReturnUnauthorizedResponse(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                message = ErrorMessage.UserNotHavePermission
            }, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        }
    }
}
