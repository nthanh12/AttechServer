using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Permission;
using AttechServer.Domains.Entities;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.AppicationBase.Common;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AttechServer.Applications.UserModules.Implements
{
    public class PermissionService : IPermissionService
    {
        private readonly ILogger<PermissionService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMemoryCache _cache;
        private const string PERMISSION_CACHE_KEY = "permissions";
        private const string USER_PERMISSIONS_CACHE_KEY = "user_permissions_{0}";

        public PermissionService(
            ILogger<PermissionService> logger,
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContext,
            IMemoryCache cache)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContext = httpContext;
            _cache = cache;
        }

        public bool CheckPermission(string[] permissionKeys)
        {
            var currentUserId = _httpContext.GetCurrentUserId();
            var currentUserType = _httpContext.GetCurrentUserType();
            _logger.LogInformation($"{nameof(CheckPermission)}: permissionKeys = {permissionKeys}, userId: {currentUserId}, userType: {currentUserType}");

            if (currentUserType == UserTypes.ADMIN)
                return true;

            var userPermissions = GetUserPermissions(currentUserId);
            return permissionKeys.Any(pk => userPermissions.Contains(pk));
        }

        public List<PermissionDto> FindAll()
        {
            _logger.LogInformation($"{nameof(FindAll)}");

            return _cache.GetOrCreate(PERMISSION_CACHE_KEY, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var permissions = _dbContext.Permissions
                    .Where(p => !p.Deleted)
                    .OrderBy(p => p.ParentId)
                    .ThenBy(p => p.PermissionLabel)
                    .ToList();

                return BuildPermissionTree(permissions);
            }) ?? new List<PermissionDto>();
        }

        public List<string> GetPermissionsByCurrentUserId()
        {
            var currentUserId = _httpContext.GetCurrentUserId();
            var currentUserType = _httpContext.GetCurrentUserType();
            _logger.LogInformation($"{nameof(GetPermissionsByCurrentUserId)}: userId: {currentUserId}, userType: {currentUserType}");

            if (currentUserType == UserTypes.ADMIN)
            {
                return _dbContext.Permissions
                    .Where(p => !p.Deleted)
                    .Select(p => p.PermissionKey)
                    .ToList();
            }

            return GetUserPermissions(currentUserId);
        }

        private List<string> GetUserPermissions(int userId)
        {
            var cacheKey = string.Format(USER_PERMISSIONS_CACHE_KEY, userId);

            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                return (from user in _dbContext.Users
                        join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                        join role in _dbContext.Roles on userRole.RoleId equals role.Id
                        join rolePermission in _dbContext.RolePermissions on role.Id equals rolePermission.RoleId
                        join permission in _dbContext.Permissions on rolePermission.PermissionId equals permission.Id
                        where user.Id == userId
                            && !user.Deleted
                            && role.Status == CommonStatus.ACTIVE
                            && !userRole.Deleted
                            && !rolePermission.Deleted
                            && !permission.Deleted
                        select permission.PermissionKey)
                        .Distinct()
                        .ToList();
            }) ?? new List<string>();
        }

        private List<PermissionDto> BuildPermissionTree(List<Permission> permissions)
        {
            var permissionDict = permissions.ToDictionary(p => p.Id);
            var rootPermissions = new List<PermissionDto>();

            foreach (var permission in permissions)
            {
                var dto = new PermissionDto
                {
                    Id = permission.Id,
                    PermissionKey = permission.PermissionKey,
                    PermissionLabel = permission.PermissionLabel,
                    Description = permission.Description,
                    ParentId = permission.ParentId
                };

                if (permission.ParentId == null)
                {
                    rootPermissions.Add(dto);
                }
                else if (permissionDict.TryGetValue(permission.ParentId.Value, out var parent))
                {
                    var parentDto = rootPermissions.FirstOrDefault(p => p.Id == parent.Id);
                    if (parentDto != null)
                    {
                        parentDto.Children.Add(dto);
                    }
                }
            }

            return rootPermissions;
        }

        public string[] GetAllPermissionKeyByApiEndpoint(string api)
        {
            return (from apiEndpoint in _dbContext.ApiEndpoints
                    join permissionOfApi in _dbContext.PermissionForApiEndpoints on apiEndpoint.Id equals permissionOfApi.ApiEndpointId
                    join permission in _dbContext.Permissions on permissionOfApi.PermissionId equals permission.Id
                    where api.ToLower().Contains(apiEndpoint.Path)
                    select permission.PermissionKey)
                    .ToArray();
        }
    }
}
