using AttechServer.Applications.UserModules.Dtos.Product;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IProductService
    {
        /// <summary>
        /// Lấy danh sách tất cả sản phẩm với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagingResult<ProductDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm theo danh mục với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<PagingResult<ProductDto>> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId);

        /// <summary>
        /// Lấy danh sách sản phẩm theo slug danh mục sản phẩm, bao gồm cả category chính và sub-categories
        /// </summary>
        Task<PagingResult<ProductDto>> FindAllByCategorySlug(PagingRequestBaseDto input, string slug);

        /// <summary>
        /// Lấy thông tin chi tiết sản phẩm theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DetailProductDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết sản phẩm theo slug (song ngữ)
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        Task<DetailProductDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới sản phẩm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProductDto> Create(CreateProductDto input);

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProductDto> Update(UpdateProductDto input);

        /// <summary>
        /// Xóa nhóm sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// Khóa/Mở khóa sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns></returns>
        Task UpdateStatusProduct(int id, int status);
    }
}
