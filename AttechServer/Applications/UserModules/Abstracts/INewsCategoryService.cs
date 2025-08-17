using AttechServer.Applications.UserModules.Dtos.NewsCategory;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface INewsCategoryService
    {
        /// <summary>
        /// Lấy danh sách tất cả danh mục tin tức với phân trang
        /// </summary>
        Task<PagingResult<NewsCategoryDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết danh mục tin tức theo Id
        /// </summary>
        Task<DetailNewsCategoryDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết danh mục tin tức theo slug
        /// </summary>
        Task<DetailNewsCategoryDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới danh mục tin tức
        /// </summary>
        Task<NewsCategoryDto> Create(CreateNewsCategoryDto input);

        /// <summary>
        /// Cập nhật danh mục tin tức
        /// </summary>
        Task<NewsCategoryDto> Update(UpdateNewsCategoryDto input);

        /// <summary>
        /// Xóa danh mục tin tức
        /// </summary>
        Task Delete(int id);

        /// <summary>
        /// Cập nhật trạng thái danh mục tin tức
        /// </summary>
        Task UpdateStatus(int id, int status);
    }
} 
