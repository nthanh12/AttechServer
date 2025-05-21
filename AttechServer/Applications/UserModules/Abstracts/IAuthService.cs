using AttechServer.Applications.UserModules.Dtos;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IAuthService
    {
        public Task<TokenApiDto> Login(UserLoginDto user);
        public TokenApiDto RefreshToken(TokenApiDto input);
        public void RegisterUser(CreateUserDto user);
    }
}
