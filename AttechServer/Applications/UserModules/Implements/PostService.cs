using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Applications.UserModules.Dtos.PostCategory;
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
                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;
                    var sanitizer = new HtmlSanitizer();
                    var safeContent = sanitizer.Sanitize(input.Content);

                    var newPost = new Post
                    {
                        Slug = GenerateSlug(input.Title),
                        Title = input.Title,
                        Description = input.Description,
                        Content = safeContent,
                        Status = CommonStatus.ACTIVE,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userId
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
                        Slug = newPost.Slug,
                        Description = newPost.Description,
                        Status = newPost.Status
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
            _logger.LogInformation($"{nameof(FindAllByCategoryId)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.Posts.AsNoTracking()
                .Where(p => p.Id == categoryId && !p.Deleted && p.Status == CommonStatus.ACTIVE
                    && (string.IsNullOrEmpty(input.Keyword) || p.Title.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(p => p.Id)
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

        //public async Task<DetailPostDto> FindById(int id)
        //{
        //    _logger.LogInformation($"{nameof(FindById)}: id = {id}");

        //    var postCategory = await _dbContext.Posts
        //        .Where(p => !p.Deleted && p.Id == id && p.Status == CommonStatus.ACTIVE)
        //        .Select(p => new DetailPostDto
        //        {
        //            Id = p.Id,
        //            Slug = p.Slug,
        //            Title = p.Title,
        //            Description = p.Description,
        //            Content = p.Content
        //                .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE)
        //                .Select(p => new PostDto
        //                {
        //                    Id = p.Id,
        //                    Slug = p.Slug,
        //                    Title = p.Title,
        //                    Description = p.Description,
        //                    Status = p.Status,
        //                    PostCategoryId = p.PostCategoryId
        //                })
        //                .ToList()
        //        })
        //        .FirstOrDefaultAsync();

        //    if (postCategory == null)
        //        throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);

        //    return postCategory;
        //}

        public Task<PostDto> Update(UpdatePostDto input)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStatusPost(int id, int status)
        {
            throw new NotImplementedException();
        }
        private string GenerateSlug(string title)
        {
            var slug = title.ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

            var randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 4);
            slug = $"{slug}-{randomSuffix}";

            return slug;
        }

    }
}
