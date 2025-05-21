using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.User;
using AttechServer.Domains.Entities;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AttechServer.Applications.UserModules.Implements
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserService(ILogger<UserService> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public void AddRoleToUser(int roleId, int userId)
        {
            _logger.LogInformation($"{nameof(AddRoleToUser)}, roleId = {roleId}, userId = {userId}");
            if (_dbContext.UserRoles.Any(ur => !ur.Deleted && ur.UserId == userId && ur.RoleId == roleId))
            {
                var userRole = _dbContext.UserRoles.FirstOrDefault(ur => !ur.Deleted && ur.UserId == userId && ur.RoleId == roleId)
                    ?? throw new UserFriendlyException(ErrorCode.RoleOrUserNotFound);
                userRole.Deleted = false;
            }
            else
            {
                var userRole = new UserRole()
                {
                    RoleId = roleId,
                    UserId = userId
                };
                _dbContext.UserRoles.Add(userRole);
            }

            _dbContext.SaveChanges();
        }

        public void RemoveRoleFromUser(int roleId, int userId)
        {
            _logger.LogInformation($"{nameof(RemoveRoleFromUser)}, roleId = {roleId}, userId = {userId}");
            var userRole = _dbContext.UserRoles.FirstOrDefault(ur => !ur.Deleted && ur.UserId == userId && ur.RoleId == roleId)
                            ?? throw new UserFriendlyException(ErrorCode.RoleOrUserNotFound);
            userRole.Deleted = true;
            _dbContext.SaveChanges();
        }

        public async Task<PagingResult<UserDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.Users.AsNoTracking()
                .Where(u => !u.Deleted
                    && (string.IsNullOrEmpty(input.Keyword) || u.Username.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(u => u.Id)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Status = u.Status,
                    UserType = u.UserType,
                    RoleIds = u.UserRoles
                        .Where(ur => !ur.Deleted)
                        .Select(ur => ur.RoleId)
                        .ToList()
                })
                .ToListAsync();

            return new PagingResult<UserDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<UserDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var user = await _dbContext.Users
                .Include(u => u.UserRoles.Where(ur => !ur.Deleted))
                .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Status = user.Status,
                UserType = user.UserType,
                RoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList()
            };
        }

        public async Task Update(UpdateUserDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");

            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => !u.Deleted && u.Id == input.Id)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            // Check if username is already taken by another user
            if (await _dbContext.Users.AnyAsync(u => !u.Deleted && u.Username == input.Username && u.Id != input.Id))
            {
                throw new UserFriendlyException(ErrorCode.UsernameIsExist);
            }

            user.Username = input.Username;
            user.Status = input.Status;
            user.UserType = input.UserType;

            // Update roles
            var currentRoleIds = user.UserRoles.Where(ur => !ur.Deleted).Select(ur => ur.RoleId).ToList();
            var rolesToAdd = input.RoleIds.Except(currentRoleIds);
            var rolesToRemove = currentRoleIds.Except(input.RoleIds);

            foreach (var roleId in rolesToAdd)
            {
                AddRoleToUser(roleId, user.Id);
            }

            foreach (var roleId in rolesToRemove)
            {
                RemoveRoleFromUser(roleId, user.Id);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            user.Deleted = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
