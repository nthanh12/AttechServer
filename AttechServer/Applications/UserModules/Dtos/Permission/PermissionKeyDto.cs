using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Permission
{
    public class PermissionKeyDto
    {
        public int Id { get; set; }
        public string KeyPermissionName { get; set; } = null!;
        public string KeyPermissionLabel { get; set; } = null!;
        public string? Description { get; set; }
        /// <summary>
        /// Loại permisison
        /// <see cref="PermissionType"/>
        /// </summary>
        public int PermissionType { get; set; }
        public string? ParentKey { get; set; }
        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int OrderPriority { get; set; }
    }
}
