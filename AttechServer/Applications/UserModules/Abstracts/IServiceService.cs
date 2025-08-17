using AttechServer.Applications.UserModules.Dtos.Service;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IServiceService
    {
        /// <summary>
        /// Lấy danh sách tất cả tin tức với phân trang
        /// </summary>
        Task<PagingResult<ServiceDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết tin tức theo Id
        /// </summary>
        Task<DetailServiceDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết tin tức theo slug (song ngữ)
        /// </summary>
        Task<DetailServiceDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới tin tức với tất cả dữ liệu (text + files) trong một request
        /// </summary>
        Task<ServiceDto> Create(CreateServiceDto input);


        /// <summary>
        /// Cập nhật tin tức
        /// </summary>
        Task<ServiceDto> Update(UpdateServiceDto input);


        /// <summary>
        /// Xóa tin tức
        /// </summary>
        Task Delete(int id);


    }
}
