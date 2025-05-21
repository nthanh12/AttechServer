using AttechServer.Domains.Entities;

namespace AttechServer.Applications.UserModules.Dtos.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public int Status { get; set; }
        public int UserType { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}