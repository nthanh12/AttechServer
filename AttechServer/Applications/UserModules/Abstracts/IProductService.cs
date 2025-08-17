using AttechServer.Applications.UserModules.Dtos.Product;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IProductService
    {
        /// <summary>
        /// Lấy danh sách tất cả tin tức với phân trang
        /// </summary>
        Task<PagingResult<ProductDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy danh sách tin tức theo slug danh mục, bao gồm cả category chính và sub-categories
        /// </summary>
        Task<PagingResult<ProductDto>> FindAllByCategorySlug(PagingRequestBaseDto input, string slug);

        /// <summary>
        /// Lấy thông tin chi tiết tin tức theo Id
        /// </summary>
        Task<DetailProductDto> FindById(int id);

        /// <summary>
        /// Lấy thông tin chi tiết tin tức theo slug (song ngữ)
        /// </summary>
        Task<DetailProductDto> FindBySlug(string slug);

        /// <summary>
        /// Thêm mới tin tức với tất cả dữ liệu (text + files) trong một request
        /// </summary>
        Task<ProductDto> Create(CreateProductDto input);


        /// <summary>
        /// Cập nhật tin tức
        /// </summary>
        Task<ProductDto> Update(UpdateProductDto input);


        /// <summary>
        /// Xóa tin tức
        /// </summary>
        Task Delete(int id);


    }
}
