using AttechServer.Applications.UserModules.Dtos.ProductCategory;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IProductCategoryService
    {
        /// <summary>
        /// L?y danh s�ch danh m?c b�i vi?t v?i ph�n trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagingResult<ProductCategoryDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// L?y th�ng tin chi ti?t danh m?c b�i vi?t
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DetailProductCategoryDto> FindById(int id);

        /// <summary>
        /// L?y th�ng tin chi ti?t danh m?c b�i vi?t theo slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        Task<DetailProductCategoryDto> FindBySlug(string slug);

        /// <summary>
        /// Th�m m?i danh m?c b�i vi?t
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProductCategoryDto> Create(CreateProductCategoryDto input);

        /// <summary>
        /// C?p nh?t danh m?c b�i vi?t
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProductCategoryDto> Update(UpdateProductCategoryDto input);

        /// <summary>
        /// X�a nh�m b�i vi?t
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// Kh�a/M? kh�a nh�m b�i vi?t
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">Tr?ng th�i m?i</param>
        /// <returns></returns>
        Task UpdateStatusProductCategory(int id, int status);
    }
}
