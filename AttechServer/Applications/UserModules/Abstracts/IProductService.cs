using AttechServer.Applications.UserModules.Dtos.Product;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IProductService
    {
        /// <summary>
        /// Lấy danh sách tất cả sản phẩm với phân trang
        /// </summary>
        Task<PagingResult<ProductDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy danh sách sản phẩm theo slug danh mục, bao gồm cả category chính và sub-categories
        /// </summary>
        Task<PagingResult<ProductDto>> FindAllByCategorySlug(PagingRequestBaseDto input, string slug);

        /// <summary>
        /// Lấy thông tin chi tiết sản phẩm theo Id
        /// </summary>
        Task<DetailProductDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết sản phẩm theo slug (song ngữ)
        /// </summary>
        Task<DetailProductDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới sản phẩm với tất cả dữ liệu (text + files) trong một request
        /// </summary>
        Task<ProductDto> Create(CreateProductDto input);

        /// <summary>
        /// Cập nhật sản phẩm
        /// ID được truyền riêng, không cần trong DTO
        /// </summary>
        Task<ProductDto> Update(int id, UpdateProductDto input);

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        Task Delete(int id);

        /// <summary>
        /// Lấy danh sách sản phẩm đã xuất bản (status = 1) cho client
        /// </summary>
        Task<PagingResult<ProductDto>> FindAllForClient(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết sản phẩm theo slug (chỉ status = 1) cho client
        /// </summary>
        Task<DetailProductDto> FindBySlugForClient(string slug);

        /// <summary>
        /// Lấy danh sách sản phẩm theo slug danh mục (chỉ status = 1) cho client
        /// </summary>
        Task<PagingResult<ProductDto>> FindAllByCategorySlugForClient(PagingRequestBaseDto input, string slug);

        /// <summary>
        /// Tìm kiếm sản phẩm đã xuất bản (chỉ status = 1) cho client
        /// </summary>
        Task<PagingResult<ProductDto>> SearchForClient(PagingRequestBaseDto input);
    }
}
