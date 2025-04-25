namespace AttechServer.Applications.UserModules.Dtos.Role
{
    public class UpdateRoleDto : CreateRoleDto
    {
        public int Id { get; set; }
        public IEnumerable<string> PermissionKeys { get; internal set; }
    }
}
