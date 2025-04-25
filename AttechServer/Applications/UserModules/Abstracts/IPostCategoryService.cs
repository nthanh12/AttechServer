using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Applications.UserModules.Dtos.PostCategory;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IPostCategoryService
    {
        /// <summary>
        /// Lấy danh sách danh mục bài viết với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagingResult<PostCategoryDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy thông tin chi tiết danh mục bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DetailPostCategoryDto> FindById(int id);

        /// <summary>
        /// Thêm mới danh mục bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PostCategoryDto> Create(CreatePostCategoryDto input);

        /// <summary>
        /// Cập nhật danh mục bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PostCategoryDto> Update(UpdatePostCategoryDto input);

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
        Task UpdateStatusPostCategory(int id, int status);
    }
}
