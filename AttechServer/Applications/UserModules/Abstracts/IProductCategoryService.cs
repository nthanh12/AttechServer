using AttechServer.Applications.UserModules.Dtos.ProductCategory;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IProductCategoryService
    {
        /// <summary>
        /// L?y danh sách danh m?c bài vi?t v?i phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagingResult<ProductCategoryDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// L?y thông tin chi ti?t danh m?c bài vi?t
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DetailProductCategoryDto> FindById(int id);

        /// <summary>
        /// Thêm m?i danh m?c bài vi?t
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProductCategoryDto> Create(CreateProductCategoryDto input);

        /// <summary>
        /// C?p nh?t danh m?c bài vi?t
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProductCategoryDto> Update(UpdateProductCategoryDto input);

        /// <summary>
        /// Xóa nhóm bài vi?t
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// Khóa/M? khóa nhóm bài vi?t
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">Tr?ng thái m?i</param>
        /// <returns></returns>
        Task UpdateStatusProductCategory(int id, int status);
    }
}
