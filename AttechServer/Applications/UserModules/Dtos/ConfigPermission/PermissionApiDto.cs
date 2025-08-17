namespace AttechServer.Applications.UserModules.Dtos.ConfigPermission
{
    public class PermissionApiDto
    {
        public int Id { get; set; }
        public string Path { get; set; } = null!;
        public string? Description { get; set; }
    }
}
