﻿using AttechServer.Applications.UserModules.Abstracts;
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
                    await _dbContext.SaveChangesAsync();

                    var newRoleId = newRole.Id;

                    if (input.PermissionIds?.Count > 0)
                    {
                        var newRolePermissions = input.PermissionIds.Select(x => new RolePermission()
                        {
                            PermissionId = x,
                            RoleId = newRoleId
                        });

                        _dbContext.RolePermissions.AddRange(newRolePermissions);
                        await _dbContext.SaveChangesAsync();
                    }

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
            var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);
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
                items = items.Skip(input.GetSkip())
                    .Take(input.PageSize)
                    .ToList();
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
                    .ThenInclude(rp => rp.Permission)
                .Where(r => !r.Deleted && r.Id == id && r.Status == CommonStatus.ACTIVE)
                .Select(c => new DetailRoleDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PermissionIds = c.RolePermissions
                        .Where(rp => !rp.Deleted)
                        .Select(rp => rp.PermissionId)
                        .Distinct()
                        .ToList(),
                })
                .FirstOrDefaultAsync()
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);

            return roleResult;
        }

        public async Task Update(UpdateRoleDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            var role = await _dbContext.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == input.Id)
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    role.Name = input.Name;
                    await _dbContext.SaveChangesAsync();

                    // Get current permissions
                    var currentRolePermissions = role.RolePermissions
                        .Where(x => !x.Deleted)
                        .Select(e => e.PermissionId)
                        .ToList();

                    // Find permissions to remove
                    var permissionsToRemove = currentRolePermissions
                        .Except(input.PermissionIds)
                        .ToList();

                    // Find permissions to add
                    var permissionsToAdd = input.PermissionIds
                        .Except(currentRolePermissions)
                        .ToList();

                    // Remove permissions
                    if (permissionsToRemove.Any())
                    {
                        await _dbContext.RolePermissions
                            .Where(rp => rp.RoleId == input.Id && permissionsToRemove.Contains(rp.PermissionId))
                            .ExecuteUpdateAsync(s => s.SetProperty(rp => rp.Deleted, true));
                    }

                    // Add new permissions
                    if (permissionsToAdd.Any())
                    {
                        var newRolePermissions = permissionsToAdd.Select(pid => new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = pid
                        });

                        _dbContext.RolePermissions.AddRange(newRolePermissions);
                        await _dbContext.SaveChangesAsync();
                    }

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
            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Id == id && !r.Deleted && r.Status == CommonStatus.ACTIVE)
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);
            role.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }
}
