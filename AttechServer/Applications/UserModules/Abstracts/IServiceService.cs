using AttechServer.Applications.UserModules.Dtos.Service;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IServiceService
    {
        /// <summary>
        /// Lấy danh sách tất cả dịch vụ với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagingResult<ServiceDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết dịch vụ theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DetailServiceDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết dịch vụ theo slug (song ngữ)
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        Task<DetailServiceDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới dịch vụ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ServiceDto> Create(CreateServiceDto input);

        /// <summary>
        /// Cập nhật dịch vụ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ServiceDto> Update(UpdateServiceDto input);

        /// <summary>
        /// Xóa nhóm dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// Khóa/Mở khóa dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns></returns>
        Task UpdateStatusService(int id, int status);
    }
}
