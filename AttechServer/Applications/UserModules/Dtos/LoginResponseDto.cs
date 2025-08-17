using AttechServer.Applications.UserModules.Dtos.User;

namespace AttechServer.Applications.UserModules.Dtos
{
    public class LoginResponseDto
    {
        public bool success { get; set; } = true;
        public string token { get; set; } = null!;
        public UserDto user { get; set; } = null!;
    }
}
