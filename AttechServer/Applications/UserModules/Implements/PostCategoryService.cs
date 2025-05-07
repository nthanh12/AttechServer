using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Applications.UserModules.Dtos.PostCategory;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AttechServer.Applications.UserModules.Implements
{
    public class PostCategoryService : IPostCategoryService
    {
        private readonly ILogger<PostCategoryService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostCategoryService(ApplicationDbContext dbContext, ILogger<PostCategoryService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PostCategoryDto> Create(CreatePostCategoryDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.Name))
                    {
                        throw new ArgumentException("Tên danh mục là bắt buộc.");
                    }
                    if (input.Description.Length > 160)
                    {
                        input.Description = input.Description.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    // Tạo slug tự động
                    var slug = GenerateSlug(input.Name);
                    var slugExists = await _dbContext.PostCategories.AnyAsync(c => c.Slug == slug && !c.Deleted);
                    if (slugExists)
                    {
                        slug = $"{slug}-{DateTime.Now.Ticks}";
                    }

                    var newPostCategory = new PostCategory
                    {
                        Name = input.Name,
                        Slug = slug,
                        Description = input.Description,
                        Status = input.Status,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false
                    };

                    _dbContext.PostCategories.Add(newPostCategory);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new PostCategoryDto
                    {
                        Id = newPostCategory.Id,
                        Name = newPostCategory.Name,
                        Slug = newPostCategory.Slug,
                        Description = newPostCategory.Description,
                        Status = newPostCategory.Status
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating post category");
                    throw;
                }
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var postCategory = await _dbContext.PostCategories
                        .FirstOrDefaultAsync(pc => pc.Id == id && !pc.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);

                    // Xóa mềm các Post liên quan
                    var posts = await _dbContext.Posts
                        .Where(p => p.PostCategoryId == id && !p.Deleted)
                        .ToListAsync();

                    foreach (var post in posts)
                    {
                        post.Deleted = true;
                    }

                    postCategory.Deleted = true;
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error deleting post category with id = {id}");
                    throw;
                }
            }
        }

        public async Task<PagingResult<PostCategoryDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.PostCategories.AsNoTracking()
                .Where(pc => !pc.Deleted
                    && (string.IsNullOrEmpty(input.Keyword) || pc.Name.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.Name)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(pc => new PostCategoryDto
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    Slug = pc.Slug,
                    Description = pc.Description,
                    Status = pc.Status
                })
                .ToListAsync();

            return new PagingResult<PostCategoryDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<DetailPostCategoryDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var postCategory = await _dbContext.PostCategories
                .Where(pc => !pc.Deleted && pc.Id == id && pc.Status == CommonStatus.ACTIVE)
                .Select(pc => new DetailPostCategoryDto
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    Slug = pc.Slug,
                    Description = pc.Description,
                    Status = pc.Status,
                    Posts = pc.Posts
                        .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE)
                        .Select(p => new PostDto
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Slug = p.Slug,
                            Description = p.Description,
                            TimePosted = p.TimePosted,
                            Status = p.Status,
                            PostCategoryId = p.PostCategoryId,
                            PostCategoryName = pc.Name,
                            PostCategorySlug = pc.Slug
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (postCategory == null)
                throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);

            return postCategory;
        }

        public async Task<PostCategoryDto> Update(UpdatePostCategoryDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            var postCategory = await _dbContext.PostCategories.FirstOrDefaultAsync(pc => pc.Id == input.Id && !pc.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.Name))
                    {
                        throw new ArgumentException("Tên danh mục là bắt buộc.");
                    }
                    if (input.Description.Length > 160)
                    {
                        input.Description = input.Description.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    // Cập nhật slug nếu tên thay đổi
                    var slug = GenerateSlug(input.Name);
                    if (slug != postCategory.Slug)
                    {
                        var slugExists = await _dbContext.PostCategories.AnyAsync(c => c.Slug == slug && !c.Deleted && c.Id != input.Id);
                        if (slugExists)
                        {
                            slug = $"{slug}-{DateTime.Now.Ticks}";
                        }
                        postCategory.Slug = slug;
                    }

                    postCategory.Name = input.Name;
                    postCategory.Description = input.Description;
                    postCategory.ModifiedBy = userId;
                    postCategory.ModifiedDate = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new PostCategoryDto
                    {
                        Id = postCategory.Id,
                        Name = postCategory.Name,
                        Slug = postCategory.Slug,
                        Description = postCategory.Description,
                        Status = postCategory.Status
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error updating post category");
                    throw;
                }
            }
        }

        public async Task UpdateStatusPostCategory(int id, int status)
        {
            _logger.LogInformation($"{nameof(UpdateStatusPostCategory)}: Id = {id}, status = {status}");
            var postCategory = await _dbContext.PostCategories.FirstOrDefaultAsync(pc => pc.Id == id && !pc.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
            postCategory.Status = status;
            await _dbContext.SaveChangesAsync();
        }

        private string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Chuyển về chữ thường, loại bỏ dấu tiếng Việt
            var slug = input.ToLowerInvariant()
                .Normalize(NormalizationForm.FormD)
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                .ToString()
                .Normalize(NormalizationForm.FormC);

            // Thay khoảng trắng bằng dấu gạch ngang, loại bỏ ký tự không hợp lệ
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9-]", "");
            slug = slug.Trim('-');

            return slug;
        }
    }
}