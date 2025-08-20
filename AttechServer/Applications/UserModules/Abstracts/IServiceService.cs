using AttechServer.Applications.UserModules.Dtos.Service;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IServiceService
    {
        /// <summary>
        /// Lấy danh sách tất cả dịch vụ với phân trang
        /// </summary>
        Task<PagingResult<ServiceDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết dịch vụ theo Id
        /// </summary>
        Task<DetailServiceDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết dịch vụ theo slug (song ngữ)
        /// </summary>
        Task<DetailServiceDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới dịch vụ với tất cả dữ liệu (text + files) trong một request
        /// </summary>
        Task<ServiceDto> Create(CreateServiceDto input);

        /// <summary>
        /// Cập nhật dịch vụ
        /// ID được truyền riêng, không cần trong DTO
        /// </summary>
        Task<ServiceDto> Update(int id, UpdateServiceDto input);

        /// <summary>
        /// Xóa dịch vụ
        /// </summary>
        Task Delete(int id);

        /// <summary>
        /// Lấy danh sách dịch vụ đã xuất bản (status = 1) cho client
        /// </summary>
        Task<PagingResult<ServiceDto>> FindAllForClient(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết dịch vụ theo slug (chỉ status = 1) cho client
        /// </summary>
        Task<DetailServiceDto> FindBySlugForClient(string slug);

        /// <summary>
        /// Tìm kiếm dịch vụ đã xuất bản (chỉ status = 1) cho client
        /// </summary>
        Task<PagingResult<ServiceDto>> SearchForClient(PagingRequestBaseDto input);
    }
}
