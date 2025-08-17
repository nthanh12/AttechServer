using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos;
using AttechServer.Applications.UserModules.Dtos.User;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Filters;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    /// <summary>
    /// User Management Controller
    /// </summary>
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IActivityLogService _activityLogService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IAuthService authService,
            IActivityLogService activityLogService) : base(logger)
        {
            _userService = userService;
            _authService = authService;
            _activityLogService = activityLogService;
        }



        /// <summary>
        /// Get user list
        /// </summary>
        [HttpGet]
        [PermissionFilter(PermissionKeys.ViewUsers)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _userService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get user details by ID
        /// </summary>
        [HttpGet("{id}")]
        [PermissionFilter(PermissionKeys.ViewUsers)]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _userService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Edit user details
        /// </summary>
        [HttpPut]
        [PermissionFilter(PermissionKeys.EditUser)]
        [SuperAdminProtectionFilter]
        public async Task<ApiResponse> Update([FromBody] UpdateUserDto input)
        {
            try
            {
                await _userService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionFilter(PermissionKeys.DeleteUser)]
        [SuperAdminProtectionFilter]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _userService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Add role to user
        /// </summary>
        [HttpPost("{userId}/roles/{roleId}")]
        [PermissionFilter(PermissionKeys.EditUser)]
        [SuperAdminProtectionFilter]
        public ApiResponse AddRoleToUser(int userId, int roleId)
        {
            try
            {
                _userService.AddRoleToUser(roleId, userId);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpDelete("{userId}/roles/{roleId}")]
        [PermissionFilter(PermissionKeys.EditUser)]
        [SuperAdminProtectionFilter]
        public ApiResponse RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                _userService.RemoveRoleFromUser(roleId, userId);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
