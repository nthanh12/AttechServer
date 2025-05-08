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
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AttechServer.Applications.UserModules.Implements
{
    public class PostService : IPostService
    {
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
        public async Task<PostDto> Create(CreatePostDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
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
                    if (input.Description.Length > 160)
                    {
                        input.Description = input.Description.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;
                    var sanitizer = new HtmlSanitizer();
                    var safeContent = sanitizer.Sanitize(input.Content);

                    // Kiểm tra danh mục
                    var category = await _dbContext.PostCategories
                        .Where(c => c.Id == input.PostCategoryId && !c.Deleted)
                        .Select(c => new { c.Id, c.Name, c.Slug })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
                    }

                    // Tạo slug
                    var slug = SlugHelper.GenerateSlug(input.Title);
                    var slugExists = await _dbContext.Posts.AnyAsync(p => p.Slug == slug && !p.Deleted);
                    if (slugExists)
                    {
                        slug = $"{slug}-{Guid.NewGuid().ToString("N").Substring(0, 4)}";
                    }

                    var newPost = new Post
                    {
                        Slug = slug,
                        Title = input.Title,
                        Description = input.Description,
                        Content = safeContent,
                        TimePosted = input.TimePosted,
                        Status = CommonStatus.ACTIVE,
                        PostCategoryId = input.PostCategoryId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false
                    };

                    _dbContext.Posts.Add(newPost);
                    await _dbContext.SaveChangesAsync();

                    var (processedContent, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContent, EntityType.Post, newPost.Id);
                    newPost.Content = processedContent;
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
            }
        }
        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");
            var postCategory = _dbContext.Posts.FirstOrDefault(pc => pc.Id == id) ?? throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
            postCategory.Deleted = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PagingResult<PostDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.Posts.AsNoTracking()
                .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE
                    && (string.IsNullOrEmpty(input.Keyword) || p.Title.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.Id)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Title = p.Title,
                    Description = p.Description,
                    Status = p.Status,
                    PostCategoryId = p.PostCategoryId
                })
                .ToListAsync();

            return new PagingResult<PostDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<PagingResult<PostDto>> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId)
        {
            _logger.LogInformation($"{nameof(FindAllByCategoryId)}: input = {JsonSerializer.Serialize(input)}, categoryId = {categoryId}");

            var baseQuery = _dbContext.Posts.AsNoTracking()
                .Where(p => p.PostCategoryId == categoryId && !p.Deleted && p.Status == CommonStatus.ACTIVE
                    && (string.IsNullOrEmpty(input.Keyword) || p.Title.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderByDescending(p => p.TimePosted)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
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

        public async Task<DetailPostDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var post = await _dbContext.Posts
                .Where(p => !p.Deleted && p.Id == id && p.Status == CommonStatus.ACTIVE)
                .Select(p => new DetailPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
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
                throw new UserFriendlyException(ErrorCode.PostNotFound);

            return post;
        }

        public async Task<PostDto> Update(UpdatePostDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == input.Id && !p.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.PostNotFound);

                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.Title) || string.IsNullOrWhiteSpace(input.Content))
                    {
                        throw new ArgumentException("Tiêu đề và nội dung là bắt buộc.");
                    }
                    if (input.TimePosted > DateTime.UtcNow)
                    {
                        throw new ArgumentException("Thời gian đăng bài không thể trong tương lai.");
                    }
                    if (input.Description.Length > 160)
                    {
                        input.Description = input.Description.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;
                    var sanitizer = new HtmlSanitizer();
                    var safeContent = sanitizer.Sanitize(input.Content);

                    // Kiểm tra danh mục
                    var category = await _dbContext.PostCategories
                        .Where(c => c.Id == input.PostCategoryId && !c.Deleted)
                        .Select(c => new { c.Id, c.Name, c.Slug })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
                    }

                    // Cập nhật slug nếu tiêu đề thay đổi
                    var slug = SlugHelper.GenerateSlug(input.Title);
                    if (slug != post.Slug)
                    {
                        var slugExists = await _dbContext.Posts.AnyAsync(p => p.Slug == slug && !p.Deleted && p.Id != input.Id);
                        if (slugExists)
                        {
                            slug = $"{slug}-{DateTime.Now.Ticks}";
                        }
                        post.Slug = slug;
                    }

                    post.Title = input.Title;
                    post.Description = input.Description;
                    post.Content = safeContent;
                    post.TimePosted = input.TimePosted;
                    post.PostCategoryId = input.PostCategoryId;
                    post.ModifiedDate = DateTime.UtcNow;
                    post.ModifiedBy = userId;

                    var (processedContent, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContent, EntityType.Post, post.Id);
                    post.Content = processedContent;

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
                    _logger.LogError(ex, "Error updating post");
                    throw;
                }
            }
        }

        public async Task UpdateStatusPost(int id, int status)
        {
            _logger.LogInformation($"{nameof(UpdateStatusPost)}: Id = {id}, status = {status}");
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id && !p.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.PostNotFound);
            post.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }
}
