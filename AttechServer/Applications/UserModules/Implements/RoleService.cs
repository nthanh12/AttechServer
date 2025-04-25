using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Role;
using AttechServer.Domains.Entities;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Consts;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AttechServer.Applications.UserModules.Implements
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public RoleService(ApplicationDbContext dbContext, ILogger<RoleService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Create(CreateRoleDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var newRole = new Role()
                    {
                        Name = input.Name,
                        Status = CommonStatus.ACTIVE
                    };

                    _dbContext.Roles.Add(newRole);
                    _dbContext.SaveChanges();

                    var newRoleId = newRole.Id;

                    if (input.PermissionKeys?.Count > 0)
                    {
                        var newRolePermissions = input.PermissionKeys.Select(x => new RolePermission()
                        {
                            PermissionKey = x,
                            RoleId = newRoleId
                        });

                        _dbContext.RolePermissions.AddRange(newRolePermissions);
                    }

                    _dbContext.SaveChanges();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");
            var role = _dbContext.Roles.FirstOrDefault(x => x.Id == id) ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);
            role.Deleted = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PagingResult<RoleDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");
            var query = _dbContext.Roles.AsNoTracking()
                .Where(r => !r.Deleted
                    && (string.IsNullOrEmpty(input.Keyword) || r.Name.Contains(input.Keyword)));

            var totalItems = await query.CountAsync();
            var items = await query.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Status = r.Status,
            }).ToListAsync();
            if (input.PageSize != -1)
            {
                items.Skip(input.GetSkip())
                .Take(input.PageSize);
            }

            var result = new PagingResult<RoleDto>
            {
                TotalItems = totalItems,
                Items = items
            };

            return result;
        }

        public async Task<DetailRoleDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            var roleResult = await _dbContext.Roles
                                            .Include(r => r.RolePermissions)
                                            .Where(r => !r.Deleted && r.Id == id && r.Status == CommonStatus.ACTIVE)
                                            .Select(c => new DetailRoleDto
                                            {
                                                Id = c.Id,
                                                Name = c.Name,
                                                PermissionKeys = c.RolePermissions.Where(rp => !rp.Deleted).Select(rp => rp.PermissionKey).Distinct().ToList(),
                                            })
                                            .FirstOrDefaultAsync()
                    ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);

            return new DetailRoleDto
            {
                Id = roleResult.Id,
                Name = roleResult.Name,
                PermissionKeys = roleResult.PermissionKeys
            };
        }

        public async Task Update(UpdateRoleDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == input.Id) ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    role.Name = input.Name;
                    _dbContext.SaveChanges();
                    var currentRolePermissions = _dbContext.RolePermissions.Where(x => !x.Deleted && x.RoleId == input.Id)
                                                                         .Select(e => e.PermissionKey).ToList();
                    //list permission cần xóa
                    var removeRolePermissions = currentRolePermissions.Except(input.PermissionKeys).ToList();
                    await _dbContext.RolePermissions.Where(rp => rp.RoleId == input.Id && removeRolePermissions.Contains(rp.PermissionKey)).ExecuteDeleteAsync();
                    _dbContext.SaveChanges();

                    var roleForUpdate = input.PermissionKeys.Select(c => new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionKey = c
                    });
                    _dbContext.RolePermissions.AddRange(roleForUpdate);

                    _dbContext.SaveChanges();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task UpdateStatusRole(int id, int status)
        {
            _logger.LogInformation($"{nameof(UpdateStatusRole)}: Id = {id}, status = {status}");
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.Deleted && r.Status == CommonStatus.ACTIVE)
                            ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);
            role.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }
}
