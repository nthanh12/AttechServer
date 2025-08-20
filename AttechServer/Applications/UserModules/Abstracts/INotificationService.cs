using AttechServer.Applications.UserModules.Dtos.Notification;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface INotificationService
    {
        /// <summary>
        /// Lấy danh sách tất cả thông báo với phân trang
        /// </summary>
        Task<PagingResult<NotificationDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy danh sách thông báo theo slug danh mục, bao gồm cả category chính và sub-categories
        /// </summary>
        Task<PagingResult<NotificationDto>> FindAllByCategorySlug(PagingRequestBaseDto input, string slug);

        /// <summary>
        /// Lấy thông tin chi tiết thông báo theo Id
        /// </summary>
        Task<DetailNotificationDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết thông báo theo slug (song ngữ)
        /// </summary>
        Task<DetailNotificationDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới thông báo với tất cả dữ liệu (text + files) trong một request
        /// </summary>
        Task<NotificationDto> Create(CreateNotificationDto input);

        /// <summary>
        /// Cập nhật thông báo
        /// ID được truyền riêng, không cần trong DTO
        /// </summary>
        Task<NotificationDto> Update(int id, UpdateNotificationDto input);

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        Task Delete(int id);

        /// <summary>
        /// Lấy danh sách thông báo đã xuất bản (status = 1) cho client
        /// </summary>
        Task<PagingResult<NotificationDto>> FindAllForClient(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết thông báo theo slug (chỉ status = 1) cho client
        /// </summary>
        Task<DetailNotificationDto> FindBySlugForClient(string slug);

        /// <summary>
        /// Lấy danh sách thông báo theo slug danh mục (chỉ status = 1) cho client
        /// </summary>
        Task<PagingResult<NotificationDto>> FindAllByCategorySlugForClient(PagingRequestBaseDto input, string slug);

        /// <summary>
        /// Tìm kiếm thông báo đã xuất bản (chỉ status = 1) cho client
        /// </summary>
        Task<PagingResult<NotificationDto>> SearchForClient(PagingRequestBaseDto input);
    }
}
