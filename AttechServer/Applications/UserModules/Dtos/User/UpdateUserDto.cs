using AttechServer.Shared.Consts;

namespace AttechServer.Applications.UserModules.Dtos.User
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public int Status { get; set; }
        public int UserType { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}