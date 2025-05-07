using AttechServer.Applications.UserModules.Dtos.ProductCategory;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IProductCategoryService
    {
        /// <summary>
        /// Lấy danh sách danh mục bài viết với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagingResult<ProductCategoryDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết danh mục bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DetailProductCategoryDto> FindById(int id);

        /// <summary>
        /// Thêm mới danh mục bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProductCategoryDto> Create(CreateProductCategoryDto input);

        /// <summary>
        /// Cập nhật danh mục bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProductCategoryDto> Update(UpdateProductCategoryDto input);

        /// <summary>
        /// Xóa nhóm bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// Khóa/Mở khóa nhóm bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns></returns>
        Task UpdateStatusProductCategory(int id, int status);
    }
}
