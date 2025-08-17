using AttechServer.Applications.UserModules.Dtos.Notification;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface INotificationService
    {
        /// <summary>
        /// Lấy danh sách tất cả tin tức với phân trang
        /// </summary>
        Task<PagingResult<NotificationDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy danh sách tin tức theo slug danh mục, bao gồm cả category chính và sub-categories
        /// </summary>
        Task<PagingResult<NotificationDto>> FindAllByCategorySlug(PagingRequestBaseDto input, string slug);

        /// <summary>
        /// Lấy thông tin chi tiết tin tức theo Id
        /// </summary>
        Task<DetailNotificationDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết tin tức theo slug (song ngữ)
        /// </summary>
        Task<DetailNotificationDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới tin tức với tất cả dữ liệu (text + files) trong một request
        /// </summary>
        Task<NotificationDto> Create(CreateNotificationDto input);


        /// <summary>
        /// Cập nhật tin tức
        /// </summary>
        Task<NotificationDto> Update(UpdateNotificationDto input);


        /// <summary>
        /// Xóa tin tức
        /// </summary>
        Task Delete(int id);


    }
}
