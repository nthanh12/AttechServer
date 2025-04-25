namespace AttechServer.Applications.UserModules.Dtos.Permission
{
    public class PermissionDto
    {
        //public int Id { get; set; }
        public string PermisisonKey { get; set; } = null!;
        public string PermissionLabel { get; set; } = null!;
        public string ParentKey { get; set; } = null!;
    }
}
