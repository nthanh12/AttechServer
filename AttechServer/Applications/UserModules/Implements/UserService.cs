using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.User;
using AttechServer.Domains.Entities;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AttechServer.Shared.Consts;

namespace AttechServer.Applications.UserModules.Implements
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContext;

        public UserService(ILogger<UserService> logger, ApplicationDbContext dbContext, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContext = httpContext;
        }

        public void AddRoleToUser(int roleId, int userId)
        {
            _logger.LogInformation($"{nameof(AddRoleToUser)}, roleId = {roleId}, userId = {userId}");

            var user = _dbContext.Users.FirstOrDefault(u => !u.Deleted && u.Id == userId)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            
            var currentUserLevel = _httpContext.GetCurrentUserLevel();
            
            if (user.UserLevel == UserLevels.SYSTEM && currentUserLevel != UserLevels.SYSTEM)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            if (user.UserLevel == UserLevels.MANAGER && currentUserLevel == UserLevels.MANAGER)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

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

            var user = _dbContext.Users.FirstOrDefault(u => !u.Deleted && u.Id == userId)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            
            var currentUserLevel = _httpContext.GetCurrentUserLevel();
            
            if (user.UserLevel == UserLevels.SYSTEM && currentUserLevel != UserLevels.SYSTEM)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            if (user.UserLevel == UserLevels.MANAGER && currentUserLevel == UserLevels.MANAGER)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            var userRole = _dbContext.UserRoles.FirstOrDefault(ur => !ur.Deleted && ur.UserId == userId && ur.RoleId == roleId)
                            ?? throw new UserFriendlyException(ErrorCode.RoleOrUserNotFound);
            userRole.Deleted = true;
            _dbContext.SaveChanges();
        }

        public async Task<PagingResult<UserDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.Users.AsNoTracking()
                .Include(u => u.UserRoles.Where(ur => !ur.Deleted))
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions.Where(rp => !rp.Deleted))
                .ThenInclude(rp => rp.Permission)
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
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    LastLogin = u.LastLogin,
                    Status = u.Status == CommonStatus.ACTIVE ? "active" : "inactive",
                    UserLevel = u.UserLevel == UserLevels.SYSTEM ? "system" :
                               u.UserLevel == UserLevels.MANAGER ? "manager" :
                               u.UserLevel == UserLevels.STAFF ? "staff" : "unknown",
                    RoleIds = u.UserRoles
                        .Where(ur => !ur.Deleted)
                        .Select(ur => ur.RoleId)
                        .ToList(),
                    RoleNames = u.UserRoles
                        .Where(ur => !ur.Deleted)
                        .Select(ur => ur.Role.Name)
                        .ToList(),
                    Roles = u.UserRoles
                        .Where(ur => !ur.Deleted)
                        .Select(ur => new UserRoleDto
                        {
                            Id = ur.Role.Id,
                            Name = ur.Role.Name,
                            Status = ur.Role.Status
                        })
                        .ToList(),
                    Permissions = u.UserRoles
                        .Where(ur => !ur.Deleted)
                        .SelectMany(ur => ur.Role.RolePermissions.Where(rp => !rp.Deleted))
                        .Select(rp => rp.Permission.PermissionLabel)
                        .Distinct()
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
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions.Where(rp => !rp.Deleted))
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Status = user.Status == CommonStatus.ACTIVE ? "active" : "inactive",
                UserLevel = user.UserLevel == UserLevels.SYSTEM ? "system" :
                           user.UserLevel == UserLevels.MANAGER ? "manager" :
                           user.UserLevel == UserLevels.STAFF ? "staff" : "unknown",
                LastLogin = user.LastLogin,
                RoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList(),
                RoleNames = user.UserRoles
                    .Where(ur => !ur.Deleted)
                    .Select(ur => ur.Role.Name)
                    .ToList(),
                Roles = user.UserRoles
                    .Where(ur => !ur.Deleted)
                    .Select(ur => new UserRoleDto
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name,
                        Status = ur.Role.Status
                    })
                    .ToList(),
                Permissions = user.UserRoles
                    .Where(ur => !ur.Deleted)
                    .SelectMany(ur => ur.Role.RolePermissions.Where(rp => !rp.Deleted))
                    .Select(rp => rp.Permission.PermissionLabel)
                    .Distinct()
                    .ToList()
            };
        }

        public async Task Update(UpdateUserDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");

            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => !u.Deleted && u.Id == input.Id)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            // B?o v? theo Strict Hierarchy
            var currentUserLevel = _httpContext.GetCurrentUserLevel();
            
            // SuperAdmin: ch? SuperAdmin m?i du?c thay d?i SuperAdmin kh�c
            if (user.UserLevel == UserLevels.SYSTEM && currentUserLevel != UserLevels.SYSTEM)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            // Admin: kh�ng du?c thay d?i Admin kh�c (ch? qu?n l� STAFF)
            if (user.UserLevel == UserLevels.MANAGER && 
                currentUserLevel == UserLevels.MANAGER)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            // Kh�ng cho ph�p n�ng c?p l�n SuperAdmin n?u kh�ng ph?i SuperAdmin
            if (input.UserLevel == UserLevels.SYSTEM && currentUserLevel != UserLevels.SYSTEM)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            // Kh�ng cho ph�p Admin n�ng c?p user l�n Admin (ch? SuperAdmin m?i du?c)
            if (input.UserLevel == UserLevels.MANAGER && currentUserLevel != UserLevels.SYSTEM)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            // Check if username is already taken by another user
            if (await _dbContext.Users.AnyAsync(u => !u.Deleted && u.Username == input.Username && u.Id != input.Id))
            {
                throw new UserFriendlyException(ErrorCode.UsernameIsExist);
            }

            // Check if email is already taken by another user
            if (!string.IsNullOrEmpty(input.Email))
            {
                if (await _dbContext.Users.AnyAsync(u => !u.Deleted && u.Email == input.Email && u.Id != input.Id))
                {
                    throw new UserFriendlyException(ErrorCode.EmailIsExist);
                }
            }

            user.Username = input.Username;
            user.FullName = input.FullName;
            user.Email = input.Email;
            user.Phone = input.Phone;
            user.Status = input.Status;
            user.UserLevel = input.UserLevel;

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

            // B?o v? theo Strict Hierarchy
            var currentUserLevel = _httpContext.GetCurrentUserLevel();
            var currentUserId = _httpContext.GetCurrentUserId();
            
            // SuperAdmin: ch? SuperAdmin m?i du?c x�a SuperAdmin kh�c
            if (user.UserLevel == UserLevels.SYSTEM && currentUserLevel != UserLevels.SYSTEM)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            // Admin: kh�ng du?c x�a Admin kh�c (ch? qu?n l� STAFF)
            if (user.UserLevel == UserLevels.MANAGER && currentUserLevel == UserLevels.MANAGER)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            // Kh�ng cho ph�p t? x�a ch�nh m�nh (t?t c? user levels)
            if (user.Id == currentUserId)
            {
                throw new UserFriendlyException(ErrorCode.AccessDenied);
            }

            user.Deleted = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
