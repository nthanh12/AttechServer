namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IUserService
    {
        /// <summary>
        /// Gán nhóm quyền cho tài khoản
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        void AddRoleToUser(int roleId, int userId);

        /// <summary>
        /// Xóa quyền từ tài khoản
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        void RemoveRoleFromUser(int roleId, int userId);
    }
}
