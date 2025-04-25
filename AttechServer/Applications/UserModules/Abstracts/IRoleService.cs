using AttechServer.Applications.UserModules.Dtos.Role;
using AttechServer.Shared.AppicationBase.Common;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IRoleService
    {
        /// <summary>
        /// Danh sách nhóm quyền
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PagingResult<RoleDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Thông tin chi tiết nhóm quyền
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<DetailRoleDto> FindById(int id);

        /// <summary>
        /// Thêm mới nhóm quyền
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task Create(CreateRoleDto input);

        /// <summary>
        /// Cập nhật nhóm quyền
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task Update(UpdateRoleDto input);

        /// <summary>
        /// Xóa nhóm quyền
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task Delete(int id);

        /// <summary>
        /// Khóa/Mở khóa nhóm quyền
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Task UpdateStatusRole(int id, int status);
    }
}
