using AttechServer.Applications.UserModules.Dtos.NotificationCategory;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface INotificationCategoryService
    {
        /// <summary>
        /// Lấy danh sách tất cả danh mục thông báo với phân trang
        /// </summary>
        Task<PagingResult<NotificationCategoryDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết danh mục thông báo theo Id
        /// </summary>
        Task<DetailNotificationCategoryDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết danh mục thông báo theo slug
        /// </summary>
        Task<DetailNotificationCategoryDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới danh mục thông báo
        /// </summary>
        Task<NotificationCategoryDto> Create(CreateNotificationCategoryDto input);

        /// <summary>
        /// Cập nhật danh mục thông báo
        /// </summary>
        Task<NotificationCategoryDto> Update(UpdateNotificationCategoryDto input);

        /// <summary>
        /// Xóa danh mục thông báo
        /// </summary>
        Task Delete(int id);

        /// <summary>
        /// Cập nhật trạng thái danh mục thông báo
        /// </summary>
        Task UpdateStatus(int id, int status);
    }
} 
