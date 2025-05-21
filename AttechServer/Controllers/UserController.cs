using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos;
using AttechServer.Applications.UserModules.Dtos.User;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Filters;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IAuthService authService) : base(logger)
        {
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Tạo tài khoản mới (chỉ dành cho Admin)
        /// </summary>
        [HttpPost]
        [PermissionFilter(PermissionKeys.CreateUser)]
        public ApiResponse Create([FromBody] CreateUserDto input)
        {
            try
            {
                _authService.RegisterUser(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng
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
        /// Lấy thông tin người dùng theo ID
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
        /// Cập nhật thông tin người dùng
        /// </summary>
        [HttpPut]
        [PermissionFilter(PermissionKeys.EditUser)]
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
        /// Xóa người dùng
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionFilter(PermissionKeys.DeleteUser)]
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
        /// Thêm role cho người dùng
        /// </summary>
        [HttpPost("{userId}/roles/{roleId}")]
        [PermissionFilter(PermissionKeys.EditUser)]
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
        /// Xóa role của người dùng
        /// </summary>
        [HttpDelete("{userId}/roles/{roleId}")]
        [PermissionFilter(PermissionKeys.EditUser)]
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
