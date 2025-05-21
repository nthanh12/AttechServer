using AttechServer.Applications.UserModules.Dtos.User;
using AttechServer.Shared.ApplicationBase.Common;

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

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        Task<PagingResult<UserDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        Task<UserDto> FindById(int id);

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        Task Update(UpdateUserDto input);

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        Task Delete(int id);
    }
}
