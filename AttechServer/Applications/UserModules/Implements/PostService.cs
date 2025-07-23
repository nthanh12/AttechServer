using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Abstractions;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AttechServer.Applications.UserModules.Implements
{
    public class PostService : IPostService
    {
        private const int MAX_CONTENT_LENGTH = 100000; // 100KB
        private readonly ILogger<PostService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWysiwygFileProcessor _wysiwygFileProcessor;

        public PostService(ApplicationDbContext dbContext, ILogger<PostService> logger, IHttpContextAccessor httpContextAccessor, IWysiwygFileProcessor wysiwygFileProcessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _wysiwygFileProcessor = wysiwygFileProcessor;
        }

        private void ValidatePostType(PostType type)
        {
            if (type != PostType.News && type != PostType.Notification)
            {
                throw new ArgumentException("Loại bài viết không hợp lệ. Chỉ chấp nhận News hoặc Notification.");
            }
        }

        private string TruncateDescription(string description, int maxLength = 160)
        {
            if (string.IsNullOrEmpty(description)) return string.Empty;
            return description.Length <= maxLength ? description : description.Substring(0, maxLength - 3) + "...";
        }

        private async Task<string> GenerateUniqueSlug(string title, PostType type, int? excludeId = null)
        {
            var slug = SlugHelper.GenerateSlug(title);
            var query = _dbContext.Posts.Where(p => p.SlugVi == slug && !p.Deleted && p.Type == type);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            var exists = await query.AnyAsync();
            if (!exists) return slug;

            return $"{slug}-{Guid.NewGuid().ToString("N").Substring(0, 4)}";
        }

        private async Task<string> ProcessContent(string content, int postId)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            if (content.Length > MAX_CONTENT_LENGTH)
            {
                throw new ArgumentException($"Nội dung không được vượt quá {MAX_CONTENT_LENGTH} ký tự.");
            }

            try
            {
                // Sanitize HTML content
                var sanitizer = new HtmlSanitizer();
                var safeContent = sanitizer.Sanitize(content);

                // Xử lý file trong content
                var (processedContent, fileEntries) = await _wysiwygFileProcessor.ProcessContentAsync(safeContent, EntityType.Post, postId);

                // Log thông tin về các file đã xử lý
                if (fileEntries?.Any() == true)
                {
                    _logger.LogInformation($"Processed {fileEntries.Count} files for post {postId}: {JsonSerializer.Serialize(fileEntries)}");
                }

                return processedContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing content for post {postId}");
                throw new UserFriendlyException(ErrorCode.InvalidClientRequest);
            }
        }

        public async Task<PostDto> Create(CreatePostDto input, PostType type)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}, type = {type}");
            ValidatePostType(type);
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.TitleVi) || string.IsNullOrWhiteSpace(input.ContentVi))
                    {
                        throw new ArgumentException("Tiêu đề và nội dung là bắt buộc.");
                    }
                    if (input.TimePosted > DateTime.UtcNow)
                    {
                        throw new ArgumentException("Thời gian đăng bài không phù hợp.");
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    // Kiểm tra danh mục
                    var category = await _dbContext.PostCategories
                        .Where(c => c.Id == input.PostCategoryId && !c.Deleted && c.Type == type)
                        .Select(c => new { c.Id, c.NameVi, c.SlugVi })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
                    }

                    // Tạo slug
                    var slug = await GenerateUniqueSlug(input.TitleVi, type);

                    var newPost = new Post
                    {
                        SlugVi = slug,
                        TitleVi = input.TitleVi,
                        DescriptionVi = TruncateDescription(input.DescriptionVi),
                        ContentVi = input.ContentVi,
                        TimePosted = input.TimePosted,
                        Status = CommonStatus.ACTIVE,
                        PostCategoryId = input.PostCategoryId,
                        Type = type,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false,
                        isOutstanding = input.isOutstanding,
                        ImageUrl = input.ImageUrl ?? string.Empty
                    };

                    _dbContext.Posts.Add(newPost);
                    await _dbContext.SaveChangesAsync();

                    // Xử lý content và file sau khi có Id
                    newPost.ContentVi = await ProcessContent(input.ContentVi, newPost.Id);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new PostDto
                    {
                        Id = newPost.Id,
                        TitleVi = newPost.TitleVi,
                        TitleEn = newPost.TitleEn,
                        SlugVi = newPost.SlugVi,
                        DescriptionVi = newPost.DescriptionVi,
                        TimePosted = newPost.TimePosted,
                        Status = newPost.Status,
                        PostCategoryId = newPost.PostCategoryId,
                        PostCategoryName = category.NameVi,
                        PostCategorySlug = category.SlugVi,
                        isOutstanding = newPost.isOutstanding,
                        ImageUrl = newPost.ImageUrl
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating post");
                    throw;
                }
                finally
                {
                    // Cleanup temp files
                    await _wysiwygFileProcessor.CleanTempFilesAsync();
                }
            }
        }

        public async Task Delete(int id, PostType type)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}, type = {type}");
            ValidatePostType(type);
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var post = await _dbContext.Posts
                        .FirstOrDefaultAsync(p => p.Id == id && !p.Deleted && p.Type == type)
                        ?? throw new UserFriendlyException(ErrorCode.PostNotFound);

                    // Xóa các file trong content
                    await _wysiwygFileProcessor.DeleteFilesAsync(EntityType.Post, post.Id);

                    post.Deleted = true;
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error deleting post with id = {id}, type = {type}");
                    throw;
                }
            }
        }

        public async Task<PagingResult<PostDto>> FindAll(PagingRequestBaseDto input, PostType type)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}, type = {type}");
            ValidatePostType(type);

            var baseQuery = _dbContext.Posts.AsNoTracking()
                .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE && p.Type == type);

            // Tìm kiếm
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                baseQuery = baseQuery.Where(p =>
                    p.TitleVi.Contains(input.Keyword) ||
                    p.DescriptionVi.Contains(input.Keyword) ||
                    p.ContentVi.Contains(input.Keyword) ||
                    p.TitleEn.Contains(input.Keyword) ||
                    p.DescriptionEn.Contains(input.Keyword) ||
                    p.ContentEn.Contains(input.Keyword));
            }

            var totalItems = await baseQuery.CountAsync();

            // Sắp xếp
            var query = baseQuery.OrderByDescending(p => p.TimePosted);
            if (input.Sort.Any())
            {
                // TODO: Implement dynamic sorting based on input.Sort
            }

            var pagedItems = await query
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    SlugVi = p.SlugVi,
                    SlugEn = p.SlugEn,
                    TitleVi = p.TitleVi,
                    TitleEn = p.TitleEn,
                    DescriptionVi = p.DescriptionVi,
                    DescriptionEn = p.DescriptionEn,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    PostCategoryId = p.PostCategoryId,
                    PostCategoryName = p.PostCategory.NameVi,
                    PostCategorySlug = p.PostCategory.SlugVi,
                    isOutstanding = p.isOutstanding,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return new PagingResult<PostDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<PagingResult<PostDto>> FindAllByCategoryId(
            PagingRequestBaseDto input,
            int categoryId,
            PostType type)
        {
            _logger.LogInformation($"{nameof(FindAllByCategoryId)}: input = {JsonSerializer.Serialize(input)}, categoryId = {categoryId}, type = {type}");
            ValidatePostType(type);

            // Kiểm tra danh mục tồn tại và đúng Type
            var exists = await _dbContext.PostCategories
                .AnyAsync(c => c.Id == categoryId && !c.Deleted && c.Type == type);
            if (!exists)
                throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);

            // Lấy tất cả categoryId của cha và sub-categories
            var allCategoryIds = await GetAllDescendantIdsAsync(categoryId);

            // Build query với tập IDs
            var baseQuery = _dbContext.Posts.AsNoTracking()
                .Where(p => !p.Deleted
                            && p.Status == CommonStatus.ACTIVE
                            && p.Type == type
                            && allCategoryIds.Contains(p.PostCategoryId));

            // Tìm kiếm
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                baseQuery = baseQuery.Where(p =>
                    p.TitleVi.Contains(input.Keyword) ||
                    p.DescriptionVi.Contains(input.Keyword) ||
                    p.ContentVi.Contains(input.Keyword) ||
                    p.TitleEn.Contains(input.Keyword) ||
                    p.DescriptionEn.Contains(input.Keyword) ||
                    p.ContentEn.Contains(input.Keyword));
            }

            var totalItems = await baseQuery.CountAsync();

            // Phân trang và sắp xếp
            var items = await baseQuery
                .OrderByDescending(p => p.TimePosted)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    SlugVi = p.SlugVi,
                    SlugEn = p.SlugEn,
                    DescriptionVi = p.DescriptionVi,
                    DescriptionEn = p.DescriptionEn,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    PostCategoryId = p.PostCategoryId,
                    PostCategoryName = p.PostCategory.NameVi,
                    PostCategorySlug = p.PostCategory.SlugVi,
                    isOutstanding = p.isOutstanding,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return new PagingResult<PostDto>
            {
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<PagingResult<PostDto>> FindAllByCategorySlug(
            PagingRequestBaseDto input,
            string slug,
            PostType type)
        {
            // Validate loại bài
            ValidatePostType(type);

            // 1. Tìm category theo slug
            var category = await _dbContext.PostCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.SlugVi == slug && !c.Deleted && c.Type == type);
            if (category == null)
                throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);

            // 2. Delegate về hàm cũ để xử lý tree + paging
            return await FindAllByCategoryId(input, category.Id, type);
        }
        public async Task<DetailPostDto> FindById(int id, PostType type)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}, type = {type}");
            ValidatePostType(type);

            var post = await _dbContext.Posts
                .Where(p => p.Id == id && !p.Deleted && p.Type == type)
                .Select(p => new DetailPostDto
                {
                    Id = p.Id,
                    SlugVi = p.SlugVi,
                    TitleVi = p.TitleVi,
                    DescriptionVi = p.DescriptionVi,
                    ContentVi = p.ContentVi,
                    SlugEn = p.SlugEn,
                    TitleEn = p.TitleEn,
                    DescriptionEn = p.DescriptionEn,
                    ContentEn = p.ContentEn,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    PostCategoryId = p.PostCategoryId,
                    PostCategoryName = p.PostCategory.NameVi,
                    PostCategorySlug = p.PostCategory.SlugVi,
                    isOutstanding = p.isOutstanding,
                    ImageUrl = p.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                throw new UserFriendlyException(ErrorCode.PostNotFound);
            }

            return post;
        }

        public async Task<DetailPostDto> FindBySlug(string slug, PostType type)
        {
            _logger.LogInformation($"{nameof(FindBySlug)}: slug = {slug}, type = {type}");
            ValidatePostType(type);
            var post = await _dbContext.Posts
                .Where(p => (p.SlugVi == slug || p.SlugEn == slug) && !p.Deleted && p.Type == type)
                .Select(p => new DetailPostDto
                {
                    Id = p.Id,
                    TitleVi = p.TitleVi,
                    TitleEn = p.TitleEn,
                    SlugVi = p.SlugVi,
                    SlugEn = p.SlugEn,
                    DescriptionVi = p.DescriptionVi,
                    DescriptionEn = p.DescriptionEn,
                    ContentVi = p.ContentVi,
                    ContentEn = p.ContentEn,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    PostCategoryId = p.PostCategoryId,
                    PostCategoryName = p.PostCategory.NameVi,
                    PostCategorySlug = p.PostCategory.SlugVi,
                    isOutstanding = p.isOutstanding,
                    ImageUrl = p.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (post == null)
                throw new UserFriendlyException(ErrorCode.PostNotFound);

            return post;
        }

        public async Task<PostDto> Update(UpdatePostDto input, PostType type)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}, type = {type}");
            ValidatePostType(type);
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.TitleVi) || string.IsNullOrWhiteSpace(input.ContentVi))
                    {
                        throw new ArgumentException("Tiêu đề và nội dung là bắt buộc.");
                    }
                    if (input.TimePosted > DateTime.UtcNow)
                    {
                        throw new ArgumentException("Thời gian đăng bài không phù hợp.");
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    // Kiểm tra bài viết tồn tại
                    var post = await _dbContext.Posts
                        .FirstOrDefaultAsync(p => p.Id == input.Id && !p.Deleted && p.Type == type)
                        ?? throw new UserFriendlyException(ErrorCode.PostNotFound);

                    // Kiểm tra danh mục
                    var category = await _dbContext.PostCategories
                        .Where(c => c.Id == input.PostCategoryId && !c.Deleted && c.Type == type)
                        .Select(c => new { c.Id, c.NameVi, c.SlugVi })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
                    }

                    // Tạo slug mới nếu title thay đổi
                    if (post.TitleVi != input.TitleVi)
                    {
                        post.SlugVi = await GenerateUniqueSlug(input.TitleVi, type, post.Id);
                    }

                    // Xóa các file cũ trong content nếu content thay đổi
                    if (post.ContentVi != input.ContentVi)
                    {
                        await _wysiwygFileProcessor.DeleteFilesAsync(EntityType.Post, post.Id);
                    }

                    post.TitleVi = input.TitleVi;
                    post.DescriptionVi = TruncateDescription(input.DescriptionVi);
                    post.ContentVi = await ProcessContent(input.ContentVi, post.Id);
                    post.TimePosted = input.TimePosted;
                    post.PostCategoryId = input.PostCategoryId;
                    post.ModifiedDate = DateTime.UtcNow;
                    post.ModifiedBy = userId;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new PostDto
                    {
                        Id = post.Id,
                        TitleVi = post.TitleVi,
                        SlugVi = post.SlugVi,
                        DescriptionVi = post.DescriptionVi,
                        TimePosted = post.TimePosted,
                        Status = post.Status,
                        PostCategoryId = post.PostCategoryId,
                        PostCategoryName = category.NameVi,
                        PostCategorySlug = category.SlugVi,
                        isOutstanding = post.isOutstanding,
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error updating post with id = {input.Id}, type = {type}");
                    throw;
                }
                finally
                {
                    // Cleanup temp files
                    await _wysiwygFileProcessor.CleanTempFilesAsync();
                }
            }
        }

        public async Task UpdateStatusPost(int id, int status, PostType type)
        {
            _logger.LogInformation($"{nameof(UpdateStatusPost)}: id = {id}, status = {status}, type = {type}");
            ValidatePostType(type);

            var post = await _dbContext.Posts
                .FirstOrDefaultAsync(p => p.Id == id && !p.Deleted && p.Type == type)
                ?? throw new UserFriendlyException(ErrorCode.PostNotFound);

            var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var userIdValue) ? userIdValue : 0;

            post.Status = status;
            post.ModifiedDate = DateTime.UtcNow;
            post.ModifiedBy = userId;

            await _dbContext.SaveChangesAsync();
        }

        // Lấy tất cả categoryId con cháu
        private async Task<List<int>> GetAllDescendantIdsAsync(int parentId)
        {
            var ids = new List<int> { parentId };
            var children = await _dbContext.PostCategories
                .Where(c => c.ParentId == parentId && !c.Deleted)
                .Select(c => c.Id)
                .ToListAsync();
            foreach (var child in children)
                ids.AddRange(await GetAllDescendantIdsAsync(child));
            return ids;
        }
    }
}