using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Permission;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.AppicationBase.Common;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Consts;

namespace AttechServer.Applications.UserModules.Implements
{
    public class PermissionService : IPermissionService
    {
        private readonly ILogger<PermissionService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContext;

        public PermissionService(ILogger<PermissionService> logger, ApplicationDbContext dbContext, IHttpContextAccessor httpContext = null)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContext = httpContext;
        }
        public bool CheckPermission(string[] permissionKeys)
        {
            var currentUserId = _httpContext.GetCurrentUserId();
            var currentUserType = _httpContext.GetCurrentUserType();
            _logger.LogInformation($"{nameof(CheckPermission)}: permissionKeys = {permissionKeys}, userId: {currentUserId}, userType: {currentUserType}");
            return currentUserType == UserTypes.ADMIN || GetListPermissionKeyContains(currentUserId, permissionKeys).Any();
        }

        public List<PermissionDto> FindAll()
        {
            _logger.LogInformation($"{nameof(FindAll)}");
            var result = PermissionConfig.appConfigs.Select(p => new PermissionDto
            {
                PermisisonKey = p.Key,
                PermissionLabel = p.Value.PermissionLabel,
                ParentKey = p.Value.ParentKey ?? ""
            }).ToList();
            return result;
        }

        public string[] GetAllPermissionKeyByApiEndpoint(string api)
        {
            var query = (from apiEndpoint in _dbContext.ApiEndpoints
                         join permissionOfApi in _dbContext.PermissionForApiEndpoints on apiEndpoint.Id equals permissionOfApi.ApiEndpointId
                         join permissionKey in _dbContext.KeyPermissions on permissionOfApi.KeyPermissionId equals permissionKey.Id
                         where api.ToLower().Contains(apiEndpoint.Path)
                         select permissionKey.PermissionKey).ToArray<string>();
            return query;
        }

        public List<string> GetPermissionsByCurrentUserId()
        {
            var currentUserId = _httpContext.GetCurrentUserId();
            var currentUserType = _httpContext.GetCurrentUserType();
            _logger.LogInformation($"{nameof(GetPermissionsByCurrentUserId)}: userId: {currentUserId}, userType: {currentUserType}");

            var result = new List<string>();
            if (currentUserType == UserTypes.ADMIN)
            {
                var temp = PermissionConfig.appConfigs.Select(c => c.Key);
                result.AddRange(PermissionConfig.appConfigs.Select(c => c.Key));
            }
            else
            {
                result = (from user in _dbContext.Users
                          join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                          join role in _dbContext.Roles on userRole.RoleId equals role.Id
                          join rolePermission in _dbContext.RolePermissions on role.Id equals rolePermission.RoleId
                          into rps
                          from rp in rps
                          where user.Id == currentUserId && !user.Deleted && role.Status == CommonStatus.ACTIVE && !userRole.Deleted
                          select rp.PermissionKey).ToList();
            }
            return result;
        }

        private IQueryable<string?> GetListPermissionKeyContains(
            int userId,
            string[] permissionKeys
        )
        {
            return from userRole in _dbContext.UserRoles
                   join role in _dbContext.Roles on userRole.RoleId equals role.Id
                   join rolePermission in _dbContext.RolePermissions
                       on role.Id equals rolePermission.RoleId
                   where
                       userRole.UserId == userId
                       && !role.Deleted
                       && !userRole.Deleted
                       && role.Status == CommonStatus.ACTIVE
                       && permissionKeys.Contains(rolePermission.PermissionKey)
                   select rolePermission.PermissionKey;
        }
    }
}
