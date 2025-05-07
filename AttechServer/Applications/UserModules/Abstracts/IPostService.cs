using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IPostService
    {
        /// <summary>
        /// Lấy danh sách tất cả bài viết với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagingResult<PostDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Lấy danh sách tất cả bài viết theo danh mục với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<PagingResult<PostDto>> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId);

        /// <summary>
        /// Lấy thông tin chi tiết bài viết theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //Task<DetailPostDto> FindById(int id);

        /// <summary>
        /// Thêm mới bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PostDto> Create(CreatePostDto input);

        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PostDto> Update(UpdatePostDto input);

        /// <summary>
        /// Xóa nhóm bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// Khóa/Mở khóa bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns></returns>
        Task UpdateStatusPost(int id, int status);
    }
}
