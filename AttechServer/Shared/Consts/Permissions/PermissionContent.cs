namespace AttechServer.Shared.Consts.Permissions
{
    public class PermissionContent
    {
        /// <summary>
        /// Key cha
        /// </summary>
        public string? ParentKey { get; set; }
        /// <summary>
        /// Key permission hi?n t?i
        /// </summary>
        public string PermissionKey { get; set; }
        public string PermissionLabel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionKey"></param>
        /// <param name="parentKey"></param>
        public PermissionContent(string permissionKey, string permissionLabel, string? parentKey = null)
        {
            PermissionKey = permissionKey;
            PermissionLabel = permissionLabel;
            ParentKey = parentKey;
        }
    }
}
