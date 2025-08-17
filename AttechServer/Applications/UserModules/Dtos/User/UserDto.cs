using AttechServer.Domains.Entities;

namespace AttechServer.Applications.UserModules.Dtos.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string UserLevel { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? LastLogin { get; set; }
        public List<int> RoleIds { get; set; } = new();
        public List<string> RoleNames { get; set; } = new();
        public List<UserRoleDto> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }

    public class UserRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Status { get; set; }
    }
}
