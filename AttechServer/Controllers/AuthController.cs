using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(ILogger<AuthController> logger, IAuthService authService) : base(logger)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng nhập 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ApiResponse> Login([FromBody] UserLoginDto input)
        {
            try
            {
                return new(await _authService.Login(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Đăng ký tài khoản
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public ApiResponse Register([FromBody] CreateUserDto input)
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
        /// Refresh token
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public ApiResponse RefreshToken([FromBody] TokenApiDto input)
        {
            try
            {
                return new(_authService.RefreshToken(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
