using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Domains.Entities.Main;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IPostService
    {
        /// <summary>
        /// Lấy danh sách tất cả bài viết với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<PagingResult<PostDto>> FindAll(PagingRequestBaseDto input, PostType type);

        /// <summary>
        /// Lấy danh sách tất cả bài viết theo danh mục với phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <param name="categoryId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<PagingResult<PostDto>> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId, PostType type);

        /// <summary>
        /// Lấy thông tin chi tiết bài viết theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<DetailPostDto> FindById(int id, PostType type);

        /// <summary>
        /// Thêm mới bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<PostDto> Create(CreatePostDto input, PostType type);

        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<PostDto> Update(UpdatePostDto input, PostType type);

        /// <summary>
        /// Xóa bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task Delete(int id, PostType type);

        /// <summary>
        /// Khóa/Mở khóa bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">Trạng thái mới</param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task UpdateStatusPost(int id, int status, PostType type);
    }
}