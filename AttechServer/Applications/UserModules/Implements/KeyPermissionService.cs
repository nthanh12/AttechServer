using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ConfigPermission;
using AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission;
using AttechServer.Domains.Entities;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AttechServer.Applications.UserModules.Implements
{
    public class KeyPermissionService : IKeyPermissionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeyPermissionService> _logger;

        public KeyPermissionService(
            ApplicationDbContext dbContext,
            IConfiguration configuration,
            ILogger<KeyPermissionService> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Create(CreateKeyPermissionDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");

            if (await _dbContext.Permissions.AnyAsync(k => k.PermissionKey == input.PermissionKey))
            {
                throw new UserFriendlyException(ErrorCode.KeyPermissionHasBeenExist);
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var maxOrderPriority = await _dbContext.Permissions
                    .Where(k => k.ParentId == input.ParentId)
                    .Select(c => c.OrderPriority)
                    .DefaultIfEmpty(0)
                    .MaxAsync();

                if (input.OrderPriority > maxOrderPriority + 1)
                {
                    throw new UserFriendlyException(ErrorCode.KeyPermissionOrderFailed);
                }

                await _dbContext.Permissions
                    .Where(k => k.ParentId == input.ParentId && k.OrderPriority >= input.OrderPriority)
                    .ExecuteUpdateAsync(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority + 1));

                var newPermission = new Permission
                {
                    PermissionKey = input.PermissionKey,
                    PermissionLabel = input.PermissionLabel,
                    OrderPriority = input.OrderPriority,
                    ParentId = input.ParentId,
                };

                _dbContext.Permissions.Add(newPermission);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
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

            var permission = await _dbContext.Permissions
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.KeyPermissionNotFound);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _dbContext.Permissions
                    .Where(k => k.ParentId == permission.ParentId && k.OrderPriority > permission.OrderPriority)
                    .ExecuteUpdateAsync(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority - 1));

                permission.Deleted = true;
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(UpdateKeyPermissionDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");

            var permission = await _dbContext.Permissions
                .FirstOrDefaultAsync(x => x.Id == input.Id)
                ?? throw new UserFriendlyException(ErrorCode.KeyPermissionNotFound);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var maxOrderPriority = await _dbContext.Permissions
                    .Where(k => k.ParentId == input.ParentId)
                    .Select(k => k.OrderPriority)
                    .DefaultIfEmpty(0)
                    .MaxAsync();

                if (input.OrderPriority > maxOrderPriority + 1)
                {
                    throw new UserFriendlyException(ErrorCode.KeyPermissionOrderFailed);
                }

                if (input.OrderPriority < permission.OrderPriority)
                {
                    await _dbContext.Permissions
                        .Where(k => k.ParentId == input.ParentId
                            && k.OrderPriority >= input.OrderPriority
                            && k.OrderPriority < permission.OrderPriority)
                        .ExecuteUpdateAsync(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority + 1));
                }
                else if (input.OrderPriority > permission.OrderPriority)
                {
                    await _dbContext.Permissions
                        .Where(k => k.ParentId == input.ParentId
                            && k.OrderPriority <= input.OrderPriority
                            && k.OrderPriority > permission.OrderPriority)
                        .ExecuteUpdateAsync(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority - 1));
                }

                permission.OrderPriority = input.OrderPriority;
                permission.ParentId = input.ParentId;
                permission.PermissionLabel = input.PermissionLabel;
                permission.PermissionKey = input.PermissionKey;

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<KeyPermissionDto>> FindAllByCurrentUserId()
        {
            throw new NotImplementedException();
        }

        public async Task<KeyPermissionDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var permission = await _dbContext.Permissions
                .Select(k => new KeyPermissionDto
                {
                    Id = k.Id,
                    ParentId = k.ParentId,
                    PermissionLabel = k.PermissionLabel,
                    PermissionKey = k.PermissionKey,
                    OrderPriority = k.OrderPriority
                })
                .FirstOrDefaultAsync(k => k.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.KeyPermissionNotFound);

            return permission;
        }

        public async Task<List<KeyPermissionDto>> FindAll()
        {
            _logger.LogInformation($"{nameof(FindAll)}");

            var rootPermissions = await _dbContext.Permissions
                .Include(m => m.Children)
                .Where(m => !m.Deleted && m.ParentId == null)
                .Select(m => new KeyPermissionDto
                {
                    Id = m.Id,
                    ParentId = m.ParentId,
                    PermissionKey = m.PermissionKey,
                    PermissionLabel = m.PermissionLabel,
                    OrderPriority = m.OrderPriority,
                })
                .OrderBy(m => m.OrderPriority)
                .ThenBy(m => m.Id)
                .ToListAsync();

            foreach (var permission in rootPermissions)
            {
                await LoadChildRecursive(permission);
            }

            return rootPermissions;
        }

        private async Task LoadChildRecursive(KeyPermissionDto permission)
        {
            var childrenPermissions = await _dbContext.Permissions
                .Include(k => k.Children)
                .Select(k => new KeyPermissionDto
                {
                    Id = k.Id,
                    ParentId = k.ParentId,
                    ParentKey = permission.PermissionKey,
                    PermissionKey = k.PermissionKey,
                    PermissionLabel = k.PermissionLabel,
                    OrderPriority = k.OrderPriority,
                })
                .Where(k => k.ParentId == permission.Id)
                .OrderBy(k => k.OrderPriority)
                .ThenBy(k => k.Id)
                .ToListAsync();

            permission.Children = childrenPermissions;
            foreach (var childrenPermission in childrenPermissions)
            {
                await LoadChildRecursive(childrenPermission);
            }
        }

        public async Task CreatePermissionConfig(CreatePermissionApiDto input)
        {
            _logger.LogInformation($"{nameof(CreatePermissionConfig)}: input = {JsonSerializer.Serialize(input)}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var newApiEndpoint = new ApiEndpoint
                {
                    Path = input.Path,
                    Description = input.Description
                };

                _dbContext.ApiEndpoints.Add(newApiEndpoint);
                await _dbContext.SaveChangesAsync();

                var existedPermissions = await _dbContext.Permissions
                    .Where(p => input.PermissionKeys.Select(pk => pk.PermissionKey).Contains(p.PermissionKey))
                    .ToDictionaryAsync(p => p.PermissionKey, p => p);

                var newPermissionForApi = new List<PermissionForApiEndpoint>();
                var newPermissions = new List<Permission>();

                foreach (var pkInput in input.PermissionKeys)
                {
                    Permission permission;
                    if (existedPermissions.TryGetValue(pkInput.PermissionKey, out var existingPermission))
                    {
                        permission = existingPermission;
                    }
                    else
                    {
                        permission = new Permission
                        {
                            PermissionKey = pkInput.PermissionKey,
                            PermissionLabel = pkInput.PermissionLabel,
                            ParentId = pkInput.ParentId,
                            OrderPriority = pkInput.OrderPriority
                        };
                        newPermissions.Add(permission);
                    }

                    newPermissionForApi.Add(new PermissionForApiEndpoint
                    {
                        Permission = permission,
                        ApiEndpoint = newApiEndpoint
                    });
                }

                if (newPermissions.Any())
                {
                    foreach (var newPermission in newPermissions)
                    {
                        var maxOrderPriority = await _dbContext.Permissions
                            .Where(k => k.ParentId == newPermission.ParentId)
                            .Select(k => k.OrderPriority)
                            .DefaultIfEmpty(0)
                            .MaxAsync();

                        if (newPermission.OrderPriority > maxOrderPriority + 1)
                        {
                            throw new UserFriendlyException(ErrorCode.KeyPermissionOrderFailed);
                        }

                        await _dbContext.Permissions
                            .Where(k => k.ParentId == newPermission.ParentId && k.OrderPriority >= newPermission.OrderPriority)
                            .ExecuteUpdateAsync(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority + 1));
                    }

                    _dbContext.Permissions.AddRange(newPermissions);
                    await _dbContext.SaveChangesAsync();
                }

                _dbContext.PermissionForApiEndpoints.AddRange(newPermissionForApi);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdatePermissionConfig(UpdatePermissionConfigDto input)
        {
            _logger.LogInformation($"{nameof(UpdatePermissionConfig)}: input = {JsonSerializer.Serialize(input)}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var apiEndpoint = await _dbContext.ApiEndpoints
                    .Include(a => a.PermissionForApiEndpoints)
                    .FirstOrDefaultAsync(a => a.Id == input.Id)
                    ?? throw new UserFriendlyException(ErrorCode.ApiEndpointNotFound);

                apiEndpoint.Path = input.Path;
                apiEndpoint.Description = input.Description;

                // Get current permissions
                var currentPermissions = apiEndpoint.PermissionForApiEndpoints
                    .Where(p => !p.Deleted)
                    .Select(p => p.PermissionId)
                    .ToList();

                // Find permissions to remove
                var permissionsToRemove = currentPermissions
                    .Except(input.PermissionIds)
                    .ToList();

                // Find permissions to add
                var permissionsToAdd = input.PermissionIds
                    .Except(currentPermissions)
                    .ToList();

                // Remove permissions
                if (permissionsToRemove.Any())
                {
                    await _dbContext.PermissionForApiEndpoints
                        .Where(p => p.ApiEndpointId == input.Id && permissionsToRemove.Contains(p.PermissionId))
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Deleted, true));
                }

                // Add new permissions
                if (permissionsToAdd.Any())
                {
                    var newPermissionForApi = permissionsToAdd.Select(pid => new PermissionForApiEndpoint
                    {
                        ApiEndpointId = apiEndpoint.Id,
                        PermissionId = pid
                    });

                    _dbContext.PermissionForApiEndpoints.AddRange(newPermissionForApi);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PagingResult<PermissionApiDto>> GetAllApiPermissions(PermissionApiRequestDto input)
        {
            _logger.LogInformation($"{nameof(GetAllApiPermissions)}: input = {JsonSerializer.Serialize(input)}");

            var query = _dbContext.ApiEndpoints
                .AsNoTracking()
                .Where(a => !a.Deleted
                    && (string.IsNullOrEmpty(input.Keyword)
                        || a.Path.Contains(input.Keyword)
                        || a.Description.Contains(input.Keyword)));

            var totalItems = await query.CountAsync();

            // Apply pagination at database level
            if (input.PageSize != -1)
            {
                query = query
                    .OrderBy(a => a.Id)
                    .Skip(input.GetSkip())
                    .Take(input.PageSize);
            }
            else
            {
                query = query.OrderBy(a => a.Id);
            }

            var items = await query
                .Select(a => new PermissionApiDto
                {
                    Id = a.Id,
                    Path = a.Path,
                    Description = a.Description
                })
                .ToListAsync();

            return new PagingResult<PermissionApiDto>
            {
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<PermissionApiDetailDto> GetApiPermissionById(int id)
        {
            _logger.LogInformation($"{nameof(GetApiPermissionById)}: id = {id}");

            var result = await _dbContext.ApiEndpoints
                .Include(a => a.PermissionForApiEndpoints)
                    .ThenInclude(p => p.Permission)
                .Where(a => !a.Deleted && a.Id == id)
                .Select(a => new PermissionApiDetailDto
                {
                    Id = a.Id,
                    Path = a.Path,
                    Description = a.Description,
                    Permissions = a.PermissionForApiEndpoints
                        .Where(p => !p.Deleted)
                        .Select(p => new KeyPermissionDto
                        {
                            Id = p.Permission.Id,
                            ParentId = p.Permission.ParentId,
                            PermissionKey = p.Permission.PermissionKey,
                            PermissionLabel = p.Permission.PermissionLabel,
                            OrderPriority = p.Permission.OrderPriority
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync()
                ?? throw new UserFriendlyException(ErrorCode.ApiEndpointNotFound);

            return result;
        }
    }
}
