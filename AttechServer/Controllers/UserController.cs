using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userServices;

        public UserController(ILogger<UserController> logger, IUserService userServices) : base(logger)
        {
            _userServices = userServices;
        }

        /// <summary>
        /// Thêm quyền
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("add-role-to-user")]
        public ApiResponse AddRoleToUser(int roleId, int userId)
        {
            try
            {
                _userServices.AddRoleToUser(roleId, userId);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        /// <summary>
        /// Gỡ quyền
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("remove-role-to-user")]
        public ApiResponse RemoveRoleToUser(int roleId, int userId)
        {
            try
            {
                _userServices.RemoveRoleFromUser(roleId, userId);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
