using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ApiEndpoint;
using AttechServer.Domains.Entities;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.Consts;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AttechServer.Applications.UserModules.Implements
{
    public class ApiEndpointService : IApiEndpointService
    {
        private readonly ILogger<ApiEndpointService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string API_ENDPOINTS_CACHE_KEY = "api_endpoints";
        private const string API_PERMISSIONS_CACHE_KEY = "api_permissions_{0}_{1}";

        public ApiEndpointService(
            ILogger<ApiEndpointService> logger,
            ApplicationDbContext dbContext,
            IMemoryCache cache)
        {
            _logger = logger;
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<List<ApiEndpointDto>> FindAll()
        {
            _logger.LogInformation($"{nameof(FindAll)}");

            return await _cache.GetOrCreateAsync(API_ENDPOINTS_CACHE_KEY, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var endpoints = await _dbContext.ApiEndpoints
                    .Include(a => a.PermissionForApiEndpoints)
                    .Where(a => !a.Deleted)
                    .OrderBy(a => a.Path)
                    .ThenBy(a => a.HttpMethod)
                    .ToListAsync();

                return endpoints.Select(e => new ApiEndpointDto
                {
                    Id = e.Id,
                    Path = e.Path,
                    HttpMethod = e.HttpMethod,
                    Description = e.Description,
                    RequireAuthentication = e.RequireAuthentication,
                    PermissionIds = e.PermissionForApiEndpoints
                        .Where(p => !p.Deleted)
                        .Select(p => p.PermissionId)
                        .ToList()
                }).ToList();
            }) ?? new List<ApiEndpointDto>();
        }

        public async Task<ApiEndpointDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var endpoint = await _dbContext.ApiEndpoints
                .Include(a => a.PermissionForApiEndpoints)
                .FirstOrDefaultAsync(a => !a.Deleted && a.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.ApiEndpointNotFound);

            return new ApiEndpointDto
            {
                Id = endpoint.Id,
                Path = endpoint.Path,
                HttpMethod = endpoint.HttpMethod,
                Description = endpoint.Description,
                RequireAuthentication = endpoint.RequireAuthentication,
                PermissionIds = endpoint.PermissionForApiEndpoints
                    .Where(p => !p.Deleted)
                    .Select(p => p.PermissionId)
                    .ToList()
            };
        }

        public async Task Create(CreateApiEndpointDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {System.Text.Json.JsonSerializer.Serialize(input)}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var endpoint = new ApiEndpoint
                {
                    Path = input.Path,
                    HttpMethod = input.HttpMethod,
                    Description = input.Description,
                    RequireAuthentication = input.RequireAuthentication
                };

                _dbContext.ApiEndpoints.Add(endpoint);
                await _dbContext.SaveChangesAsync();

                if (input.PermissionIds?.Any() == true)
                {
                    var permissions = input.PermissionIds.Select(pid => new PermissionForApiEndpoint
                    {
                        ApiEndpointId = endpoint.Id,
                        PermissionId = pid,
                        IsRequired = true
                    });

                    _dbContext.PermissionForApiEndpoints.AddRange(permissions);
                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                _cache.Remove(API_ENDPOINTS_CACHE_KEY);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(UpdateApiEndpointDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {System.Text.Json.JsonSerializer.Serialize(input)}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var endpoint = await _dbContext.ApiEndpoints
                    .Include(a => a.PermissionForApiEndpoints)
                    .FirstOrDefaultAsync(a => !a.Deleted && a.Id == input.Id)
                    ?? throw new UserFriendlyException(ErrorCode.ApiEndpointNotFound);

                endpoint.Path = input.Path;
                endpoint.HttpMethod = input.HttpMethod;
                endpoint.Description = input.Description;
                endpoint.RequireAuthentication = input.RequireAuthentication;

                // Update permissions
                var existingPermissions = endpoint.PermissionForApiEndpoints
                    .Where(p => !p.Deleted)
                    .ToList();

                var permissionsToAdd = input.PermissionIds
                    .Where(pid => !existingPermissions.Any(p => p.PermissionId == pid))
                    .Select(pid => new PermissionForApiEndpoint
                    {
                        ApiEndpointId = endpoint.Id,
                        PermissionId = pid,
                        IsRequired = true
                    });

                var permissionsToRemove = existingPermissions
                    .Where(p => !input.PermissionIds.Contains(p.PermissionId));

                foreach (var permission in permissionsToRemove)
                {
                    permission.Deleted = true;
                }

                _dbContext.PermissionForApiEndpoints.AddRange(permissionsToAdd);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                _cache.Remove(API_ENDPOINTS_CACHE_KEY);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");

            var endpoint = await _dbContext.ApiEndpoints
                .FirstOrDefaultAsync(a => !a.Deleted && a.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.ApiEndpointNotFound);

            endpoint.Deleted = true;
            await _dbContext.SaveChangesAsync();
            _cache.Remove(API_ENDPOINTS_CACHE_KEY);
        }

        public async Task<bool> CheckApiPermission(string path, string method, int userId)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(method))
                return false;

            var cacheKey = string.Format(API_PERMISSIONS_CACHE_KEY, path, method);

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                var endpoint = await _dbContext.ApiEndpoints
                    .Include(a => a.PermissionForApiEndpoints)
                    .FirstOrDefaultAsync(a => !a.Deleted
                        && a.Path.ToLower() == path.ToLower()
                        && a.HttpMethod.ToUpper() == method.ToUpper());

                if (endpoint == null)
                    return false;

                if (!endpoint.RequireAuthentication)
                    return true;

                var userPermissions = await _dbContext.UserRoles
                    .Include(ur => ur.Role)
                    .Where(ur => !ur.Deleted
                        && ur.UserId == userId
                        && ur.Role != null
                        && ur.Role.Status == CommonStatus.ACTIVE
                        && !ur.Role.Deleted)
                    .Join(_dbContext.RolePermissions,
                        ur => ur.RoleId,
                        rp => rp.RoleId,
                        (ur, rp) => new { UserRole = ur, RolePermission = rp })
                    .Where(x => !x.RolePermission.Deleted)
                    .Select(x => x.RolePermission.PermissionId)
                    .Distinct()
                    .ToListAsync();

                var requiredPermissions = endpoint.PermissionForApiEndpoints
                    .Where(p => !p.Deleted && p.IsRequired)
                    .Select(p => p.PermissionId)
                    .ToList();

                return requiredPermissions.All(p => userPermissions.Contains(p));
            });
        }
    }
}