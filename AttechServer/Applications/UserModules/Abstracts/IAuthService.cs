using AttechServer.Applications.UserModules.Dtos;
using AttechServer.Applications.UserModules.Dtos.User;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IAuthService
    {
        public Task<LoginResponseDto> Login(UserLoginDto user);
        public Task<TokenApiDto> RefreshToken(TokenApiDto input);
        public void RegisterUser(CreateUserDto user);
        public Task<UserDto?> GetUserWithPermissionsAsync(int userId);
    }
}
