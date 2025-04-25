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
        private readonly ILogger _logger;

        public KeyPermissionService(ApplicationDbContext dbContext, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public void Create(CreateKeyPermissionDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            if (_dbContext.KeyPermission.Any(k => k.PermissionKey == input.PermissionKey))
            {
                throw new UserFriendlyException(ErrorCode.KeyPermissionHasBeenExist);
            }
            var transaction = _dbContext.Database.BeginTransaction();
            var maxOrderPriority = _dbContext.KeyPermission.Where(k => k.ParentId == input.ParentId).Select(c => c.OrderPriority).Max();
            if (input.OrderPriority > maxOrderPriority + 1)
            {
                throw new UserFriendlyException(ErrorCode.KeyPermissionOrderFailed);
            }
            _dbContext.KeyPermission.Where(k => k.ParentId == input.ParentId && k.OrderPriority >= input.OrderPriority)
                                    .ExecuteUpdate(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority + 1));
            _dbContext.SaveChanges();

            var newKeyPermission = new KeyPermission()
            {
                PermissionKey = input.PermissionKey,
                PermissionLabel = input.PermissionLabel,
                OrderPriority = input.OrderPriority,
                ParentId = input.ParentId,
            };

            _dbContext.KeyPermission.Add(newKeyPermission);
            _dbContext.SaveChanges();

            transaction.Commit();
        }

        public void Delete(int id)
        {
            var keyPermission = _dbContext.KeyPermission.FirstOrDefault(x => x.Id == id)
                                ?? throw new UserFriendlyException(ErrorCode.KeyPermissionNotFound);
            var transaction = _dbContext.Database.BeginTransaction();

            _dbContext.KeyPermission.Where(k => k.ParentId == keyPermission.ParentId && k.OrderPriority > keyPermission.OrderPriority)
                                        .ExecuteUpdate(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority - 1));
            _dbContext.SaveChanges();

            _dbContext.KeyPermission.Remove(keyPermission);
            _dbContext.SaveChanges();

            transaction.Commit();
        }

        public void Update(UpdateKeyPermissionDto input)
        {
            var keyPermission = _dbContext.KeyPermission.FirstOrDefault(x => x.Id == input.Id)
                              ?? throw new UserFriendlyException(ErrorCode.KeyPermissionNotFound);
            var transaction = _dbContext.Database.BeginTransaction();
            var maxOrderPriority = _dbContext.KeyPermission.Where(k => k.Id == input.ParentId).Select(k => k.OrderPriority).Max();
            if (input.OrderPriority > maxOrderPriority + 1)
            {
                throw new UserFriendlyException(ErrorCode.KeyPermissionOrderFailed);
            }
            if (input.OrderPriority < keyPermission.OrderPriority)
            {
                _dbContext.KeyPermission.Where(k => k.ParentId == input.ParentId && k.OrderPriority >= input.OrderPriority && k.OrderPriority < keyPermission.OrderPriority)
                                        .ExecuteUpdate(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority + 1));
                _dbContext.SaveChanges();
            }
            else if (input.OrderPriority > keyPermission.OrderPriority)
            {
                _dbContext.KeyPermission.Where(k => k.ParentId == input.ParentId && k.OrderPriority <= input.OrderPriority && k.OrderPriority > keyPermission.OrderPriority)
                                        .ExecuteUpdate(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority - 1));
                _dbContext.SaveChanges();
            }

            keyPermission.OrderPriority = input.OrderPriority;
            keyPermission.ParentId = input.ParentId;
            keyPermission.PermissionLabel = input.PermissionLabel;
            keyPermission.PermissionKey = input.PermissionKey;
            _dbContext.SaveChanges();
            transaction.Commit();
        }

        public List<KeyPermissionDto> FindAllByCurrentUserId()
        {
            throw new NotImplementedException();
        }

        public KeyPermissionDto FindById(int id)
        {
            var keyPermission = _dbContext.KeyPermission.Select(k => new KeyPermissionDto
            {
                Id = k.Id,
                ParentId = k.ParentId,
                PermissionLabel = k.PermissionLabel,
                //Children = k.Children,
                PermissionKey = k.PermissionKey,
                OrderPriority = k.OrderPriority
            }).FirstOrDefault(k => k.Id == id) ?? throw new UserFriendlyException(ErrorCode.KeyPermissionNotFound);
            return keyPermission;
        }

        public List<KeyPermissionDto> FindAll()
        {
            var rootPermissions = _dbContext.KeyPermission
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
                            .ToList();

            foreach (var permission in rootPermissions)
            {
                LoadChildRecursive(permission);
            }

            return rootPermissions;
        }

        private void LoadChildRecursive(KeyPermissionDto permission)
        {
            var childrenPermissions = _dbContext.KeyPermission
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
                .ToList();

            permission.Children = childrenPermissions;
            foreach (var childrenPermission in childrenPermissions)
            {
                LoadChildRecursive(childrenPermission);
            }
        }

        public void CreatePermissionConfig(CreatePermissionApiDto input)
        {
            _logger.LogInformation($"{nameof(CreatePermissionConfig)}: input = {JsonSerializer.Serialize(input)}");
            var transaction = _dbContext.Database.BeginTransaction();
            var newApiEndpoint = new ApiEndpoint()
            {
                Path = input.Path,
                Description = input.Description
            };
            _dbContext.ApiEndpoints.Add(newApiEndpoint);
            _dbContext.SaveChanges();

            var existedPK = _dbContext.KeyPermission
                .Where(kp => input.PermissionKeys.Select(pk => pk.PermissionKey).Contains(kp.PermissionKey))
                .ToDictionary(kp => kp.PermissionKey, kp => kp);

            var newPermissionForApi = new List<PermissionForApiEndpoint>();
            var newKeyPermissions = new List<KeyPermission>();

            foreach (var pkInput in input.PermissionKeys)
            {
                KeyPermission keyPermission;
                if (existedPK.TryGetValue(pkInput.PermissionKey, out var existingKeyPermission))
                {
                    keyPermission = existingKeyPermission;
                }
                else
                {
                    keyPermission = new KeyPermission
                    {
                        PermissionKey = pkInput.PermissionKey,
                        PermissionLabel = pkInput.PermissionLabel,
                        ParentId = pkInput.ParentId, // Có thể null cho permission key gốc
                        OrderPriority = pkInput.OrderPriority
                    };
                    newKeyPermissions.Add(keyPermission);
                }

                newPermissionForApi.Add(new PermissionForApiEndpoint
                {
                    KeyPermission = keyPermission,
                    ApiEndpoint = newApiEndpoint
                });
            }

            if (newKeyPermissions.Any())
            {
                foreach (var newKeyPermission in newKeyPermissions)
                {
                    var maxOrderPriority = _dbContext.KeyPermission.Where(k => k.ParentId == newKeyPermission.ParentId).Select(k => k.OrderPriority).Max();
                    if (newKeyPermission.OrderPriority > maxOrderPriority + 1)
                    {
                        throw new UserFriendlyException(ErrorCode.KeyPermissionOrderFailed);
                    }
                    _dbContext.KeyPermission.Where(k => k.ParentId == newKeyPermission.ParentId && k.OrderPriority >= newKeyPermission.OrderPriority)
                                   .ExecuteUpdate(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority + 1));
                    _dbContext.SaveChanges();
                }
                _dbContext.KeyPermission.AddRange(newKeyPermissions);
            }

            _dbContext.PermissionForApiEndpoints.AddRange(newPermissionForApi);
            _dbContext.SaveChanges();

            transaction.Commit();
        }

        public void UpdatePermissionConfig(UpdatePermissionConfigDto input)
        {
            _logger.LogInformation($"{nameof(UpdatePermissionConfig)}: input = {JsonSerializer.Serialize(input)}");
            var transaction = _dbContext.Database.BeginTransaction();

            // Lấy ApiEndpoint cần cập nhật
            var apiEndpoint = _dbContext.ApiEndpoints.FirstOrDefault(a => a.Id == input.Id)
                                ?? throw new UserFriendlyException(ErrorCode.ApiEndpointNotFound);
            apiEndpoint.Path = input.Path;
            apiEndpoint.Description = input.Description;

            // Lấy tất cả KeyPermission hiện có liên quan đến input
            var existingKeyPermissions = _dbContext.KeyPermission
                .Where(kp => input.PermissionKeys.Select(pk => pk.Id).Contains(kp.Id) ||
                             input.PermissionKeys.Select(pk => pk.PermissionKey).Contains(kp.PermissionKey))
                .ToDictionary(kp => kp.PermissionKey);

            var updatedOrNewKeyPermissions = new List<KeyPermission>();

            foreach (var permissionKeyInput in input.PermissionKeys)
            {
                var maxOrderPriority = _dbContext.KeyPermission.Where(k => k.ParentId == permissionKeyInput.ParentId).Select(k => k.OrderPriority).Max();
                if (existingKeyPermissions.TryGetValue(permissionKeyInput.PermissionKey, out var existingKeyPermission))
                {
                    if (permissionKeyInput.OrderPriority >= maxOrderPriority + 1)
                    {
                        //permissionKeyInput.OrderPriority = maxOrderPriority + 1;
                        throw new UserFriendlyException(ErrorCode.KeyPermissionOrderFailed);
                    }
                    // Cập nhật nếu đã tồn tại
                    if (permissionKeyInput.OrderPriority > existingKeyPermission.OrderPriority)
                    {
                        _dbContext.KeyPermission.Where(k => k.ParentId == permissionKeyInput.ParentId && k.OrderPriority <= permissionKeyInput.OrderPriority && k.OrderPriority > existingKeyPermission.OrderPriority)
                                                .ExecuteUpdate(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority - 1));
                    }
                    else if (permissionKeyInput.OrderPriority < existingKeyPermission.OrderPriority)
                    {
                        _dbContext.KeyPermission.Where(k => k.ParentId == permissionKeyInput.ParentId && k.OrderPriority >= permissionKeyInput.OrderPriority && k.OrderPriority < existingKeyPermission.OrderPriority)
                                                .ExecuteUpdate(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority + 1));
                    }
                    existingKeyPermission.PermissionLabel = permissionKeyInput.PermissionLabel;
                    existingKeyPermission.OrderPriority = permissionKeyInput.OrderPriority;
                    existingKeyPermission.ParentId = permissionKeyInput.ParentId; // Có thể null cho permission key gốc
                    updatedOrNewKeyPermissions.Add(existingKeyPermission);
                }
                else
                {
                    if (permissionKeyInput.OrderPriority > maxOrderPriority + 1)
                    {
                        //permissionKeyInput.OrderPriority = maxOrderPriority + 1;
                        throw new UserFriendlyException(ErrorCode.KeyPermissionOrderFailed);
                    }
                    // Thêm mới nếu chưa tồn tại
                    var newKeyPermission = new KeyPermission
                    {
                        PermissionKey = permissionKeyInput.PermissionKey,
                        PermissionLabel = permissionKeyInput.PermissionLabel,
                        OrderPriority = permissionKeyInput.OrderPriority,
                        ParentId = permissionKeyInput.ParentId // Có thể null cho permission key gốc
                    };
                    _dbContext.KeyPermission.Where(k => k.ParentId == newKeyPermission.ParentId && k.OrderPriority >= newKeyPermission.OrderPriority)
                                  .ExecuteUpdate(kp => kp.SetProperty(k => k.OrderPriority, k => k.OrderPriority + 1));
                    _dbContext.KeyPermission.Add(newKeyPermission);
                    updatedOrNewKeyPermissions.Add(newKeyPermission);
                }
            }
            _dbContext.SaveChanges();

            // Cập nhật PermissionForApiEndpoints
            var existingPermissionsForApi = _dbContext.PermissionForApiEndpoints
                .Where(pfa => pfa.ApiEndpointId == input.Id)
                .ToList();
            _dbContext.PermissionForApiEndpoints.RemoveRange(existingPermissionsForApi);

            var newPermissionsForApi = updatedOrNewKeyPermissions.Select(kp => new PermissionForApiEndpoint
            {
                ApiEndpointId = input.Id,
                KeyPermissionId = kp.Id
            });
            _dbContext.PermissionForApiEndpoints.AddRange(newPermissionsForApi);

            _dbContext.SaveChanges();
            transaction.Commit();
        }

        public PagingResult<PermissionApiDto> GetAllPermissionApi(PermissionApiRequestDto input)
        {
            var query = _dbContext.ApiEndpoints.Select(c => new PermissionApiDto
            {
                Id = c.Id,
                Path = c.Path,
                Description = c.Description,
            }).AsQueryable();

            var result = new PagingResult<PermissionApiDto>()
            {
                TotalItems = query.Count(),
            };

            if (input.PageSize != -1)
            {
                query = query.Skip(input.GetSkip()).Take(input.PageSize);
            }

            result.Items = query;

            return result;
        }

        public PermissionApiDetailDto GetPermissionApiById(int id)
        {
            var result = _dbContext.ApiEndpoints.Where(c => c.Id == id).Include(c => c.PermissionForApiEndpoints)
                                                    .ThenInclude(c => c.KeyPermission)
                                                    .Select(api => new PermissionApiDetailDto
                                                    {
                                                        Id = api.Id,
                                                        Path = api.Path,
                                                        Description = api.Description,
                                                        KeyPermissions = api.PermissionForApiEndpoints.Select(permissionForApi => new KeyPermissionDto
                                                        {
                                                            Id = permissionForApi.KeyPermission.Id,
                                                            PermissionKey = permissionForApi.KeyPermission.PermissionKey,
                                                            PermissionLabel = permissionForApi.KeyPermission.PermissionLabel,
                                                            ParentId = permissionForApi.KeyPermission.ParentId,
                                                            Parent = permissionForApi.KeyPermission.ParentId.HasValue ? new
                                                            {
                                                                Key = permissionForApi.KeyPermission.ParentId,
                                                                Label = permissionForApi.KeyPermission.Parent!.PermissionLabel,
                                                                Data = permissionForApi.KeyPermission.Parent!.PermissionKey,
                                                            } : null,
                                                            OrderPriority = permissionForApi.KeyPermission.OrderPriority
                                                        }).ToList()
                                                    }).FirstOrDefault() ?? throw new UserFriendlyException(ErrorCode.ApiEndpointNotFound);
            return result;
        }

    }
}
