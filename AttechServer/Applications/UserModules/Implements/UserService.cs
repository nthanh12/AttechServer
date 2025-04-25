using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Domains.Entities;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;

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
    }
}
