using AttechServer.Shared.Consts;

namespace AttechServer.Applications.UserModules.Dtos.Role
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        /// <summary>
        /// Trạng thái role
        /// <see cref="CommonStatus"/>
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Số người sử dụng
        /// </summary>
        //public int CountUserUsing {  get; set; }

    }
}
