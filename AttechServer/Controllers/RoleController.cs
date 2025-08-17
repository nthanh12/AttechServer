using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Role;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Filters;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/role")]
    [ApiController]
    [Authorize]
    public class RoleController : ApiControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService) : base(logger)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Role list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [PermissionFilter(PermissionKeys.MenuRoleManager)]
        [HttpGet("find-all")]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _roleService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Role details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PermissionFilter(PermissionKeys.ViewRoles, PermissionKeys.EditRole)]
        [HttpGet("find-by-id/{id}")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _roleService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get permission by role id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PermissionFilter(PermissionKeys.ViewRoles)]
        [HttpGet("permissions/{id}")]
        public async Task<ApiResponse> GetRolePermissions(int id)
        {
            try
            {
                return new(await _roleService.GetRolePermissions(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Create role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [PermissionFilter(PermissionKeys.CreateRole)]
        public async Task<ApiResponse> Create([FromBody] CreateRoleDto input)
        {
            try
            {
                await _roleService.Create(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Update role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [PermissionFilter(PermissionKeys.EditRole)]
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdateRoleDto input)
        {
            try
            {
                await _roleService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        [PermissionFilter(PermissionKeys.DeleteRole)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _roleService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
