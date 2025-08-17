using AttechServer.Applications.UserModules.Dtos.User;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IUserService
    {
        /// <summary>
        /// Gán nhóm quy?n cho tài kho?n
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        void AddRoleToUser(int roleId, int userId);

        /// <summary>
        /// Xóa quy?n t? tài kho?n
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        void RemoveRoleFromUser(int roleId, int userId);

        /// <summary>
        /// L?y danh sách ngu?i dùng
        /// </summary>
        Task<PagingResult<UserDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// L?y thông tin ngu?i dùng theo ID
        /// </summary>
        Task<UserDto> FindById(int id);

        /// <summary>
        /// C?p nh?t thông tin ngu?i dùng
        /// </summary>
        Task Update(UpdateUserDto input);

        /// <summary>
        /// Xóa ngu?i dùng
        /// </summary>
        Task Delete(int id);
    }
}
