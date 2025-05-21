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
            var query = _dbContext.Posts.Where(p => p.Slug == slug && !p.Deleted && p.Type == type);

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
                    if (string.IsNullOrWhiteSpace(input.Title) || string.IsNullOrWhiteSpace(input.Content))
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
                        .Select(c => new { c.Id, c.Name, c.Slug })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
                    }

                    // Tạo slug
                    var slug = await GenerateUniqueSlug(input.Title, type);

                    var newPost = new Post
                    {
                        Slug = slug,
                        Title = input.Title,
                        Description = TruncateDescription(input.Description),
                        Content = input.Content, // Tạm thời lưu content gốc
                        TimePosted = input.TimePosted,
                        Status = CommonStatus.ACTIVE,
                        PostCategoryId = input.PostCategoryId,
                        Type = type,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false
                    };

                    _dbContext.Posts.Add(newPost);
                    await _dbContext.SaveChangesAsync();

                    // Xử lý content và file sau khi có Id
                    newPost.Content = await ProcessContent(input.Content, newPost.Id);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new PostDto
                    {
                        Id = newPost.Id,
                        Title = newPost.Title,
                        Slug = newPost.Slug,
                        Description = newPost.Description,
                        TimePosted = newPost.TimePosted,
                        Status = newPost.Status,
                        PostCategoryId = newPost.PostCategoryId,
                        PostCategoryName = category.Name,
                        PostCategorySlug = category.Slug
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
                    p.Title.Contains(input.Keyword) ||
                    p.Description.Contains(input.Keyword) ||
                    p.Content.Contains(input.Keyword));
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
                    Slug = p.Slug,
                    Title = p.Title,
                    Description = p.Description,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    PostCategoryId = p.PostCategoryId,
                    PostCategoryName = p.PostCategory.Name,
                    PostCategorySlug = p.PostCategory.Slug
                })
                .ToListAsync();

            return new PagingResult<PostDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<PagingResult<PostDto>> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId, PostType type)
        {
            _logger.LogInformation($"{nameof(FindAllByCategoryId)}: input = {JsonSerializer.Serialize(input)}, categoryId = {categoryId}, type = {type}");
            ValidatePostType(type);

            // Kiểm tra danh mục tồn tại và đúng Type
            var categoryExists = await _dbContext.PostCategories
                .AnyAsync(c => c.Id == categoryId && !c.Deleted && c.Type == type);
            if (!categoryExists)
            {
                throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
            }

            var baseQuery = _dbContext.Posts.AsNoTracking()
                .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE && p.Type == type && p.PostCategoryId == categoryId);

            // Tìm kiếm
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                baseQuery = baseQuery.Where(p =>
                    p.Title.Contains(input.Keyword) ||
                    p.Description.Contains(input.Keyword) ||
                    p.Content.Contains(input.Keyword));
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
                    Slug = p.Slug,
                    Title = p.Title,
                    Description = p.Description,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    PostCategoryId = p.PostCategoryId,
                    PostCategoryName = p.PostCategory.Name,
                    PostCategorySlug = p.PostCategory.Slug
                })
                .ToListAsync();

            return new PagingResult<PostDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
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
                    Slug = p.Slug,
                    Title = p.Title,
                    Description = p.Description,
                    Content = p.Content,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    PostCategoryId = p.PostCategoryId,
                    PostCategoryName = p.PostCategory.Name,
                    PostCategorySlug = p.PostCategory.Slug
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                throw new UserFriendlyException(ErrorCode.PostNotFound);
            }

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
                    if (string.IsNullOrWhiteSpace(input.Title) || string.IsNullOrWhiteSpace(input.Content))
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
                        .Select(c => new { c.Id, c.Name, c.Slug })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
                    }

                    // Tạo slug mới nếu title thay đổi
                    if (post.Title != input.Title)
                    {
                        post.Slug = await GenerateUniqueSlug(input.Title, type, post.Id);
                    }

                    // Xóa các file cũ trong content nếu content thay đổi
                    if (post.Content != input.Content)
                    {
                        await _wysiwygFileProcessor.DeleteFilesAsync(EntityType.Post, post.Id);
                    }

                    post.Title = input.Title;
                    post.Description = TruncateDescription(input.Description);
                    post.Content = await ProcessContent(input.Content, post.Id);
                    post.TimePosted = input.TimePosted;
                    post.PostCategoryId = input.PostCategoryId;
                    post.ModifiedDate = DateTime.UtcNow;
                    post.ModifiedBy = userId;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new PostDto
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Slug = post.Slug,
                        Description = post.Description,
                        TimePosted = post.TimePosted,
                        Status = post.Status,
                        PostCategoryId = post.PostCategoryId,
                        PostCategoryName = category.Name,
                        PostCategorySlug = category.Slug
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
    }
}