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

            var newRole = new Role()
            {
                Name = input.Name,
                Description = input.Description,
                Status = input.Status,
            };

            _dbContext.Roles.Add(newRole);
            await _dbContext.SaveChangesAsync();
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
                .Include(r => r.Users.Where(u => !u.Deleted))
                .Where(r => !r.Deleted
                    && (string.IsNullOrEmpty(input.Keyword) || r.Name.Contains(input.Keyword)));

            var totalItems = await query.CountAsync();
            var items = await query.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Status = r.Status,
                UserCount = r.Users.Count(u => !u.Deleted),
                CreatedAt = r.CreatedDate ?? DateTime.MinValue,
                UpdatedAt = r.ModifiedDate
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
                .Where(r => !r.Deleted && r.Id == id)
                .Select(c => new DetailRoleDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Status = c.Status
                })
                .FirstOrDefaultAsync()
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);

            return roleResult;
        }

        public async Task Update(UpdateRoleDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Id == input.Id)
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);

            role.Name = input.Name;
            role.Description = input.Description;
            role.Status = input.Status;
            await _dbContext.SaveChangesAsync();
        }

    }
}
