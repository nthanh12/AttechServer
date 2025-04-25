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
using System.Text.Json;

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
                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?
                        .Value, out var id) ? id : 0;
                    var newPostCategory = new PostCategory()
                    {
                        Name = input.Name,
                        Status = CommonStatus.ACTIVE,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userId
                    };
                    _dbContext.PostCategories.Add(newPostCategory);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new PostCategoryDto
                    {
                        Id = newPostCategory.Id,
                        Name = newPostCategory.Name,
                        Status = newPostCategory.Status
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");
            var postCategory = _dbContext.PostCategories.FirstOrDefault(pc => pc.Id == id) ?? throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
            postCategory.Deleted = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PagingResult<PostCategoryDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.PostCategories.AsNoTracking()
                .Where(pc => !pc.Deleted
                    && (string.IsNullOrEmpty(input.Keyword) || pc.Name.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.Id)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(pc => new PostCategoryDto
                {
                    Id = pc.Id,
                    Name = pc.Name,
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
                    Posts = pc.Posts
                        .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE)
                        .Select(p => new PostDto
                        {
                            Id = p.Id,
                            Slug = p.Slug,
                            Title = p.Title,
                            Description = p.Description,
                            Status = p.Status,
                            PostCategoryId = p.PostCategoryId
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
            var postCategory = await _dbContext.PostCategories.FirstOrDefaultAsync(pc => pc.Id == input.Id) ?? throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    postCategory.Name = input.Name;
                    postCategory.ModifiedBy = userId;
                    postCategory.ModifiedDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new PostCategoryDto
                    {
                        Id = postCategory.Id,
                        Name = postCategory.Name,
                        Status = postCategory.Status
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task UpdateStatusPostCategory(int id, int status)
        {
            _logger.LogInformation($"{nameof(UpdateStatusPostCategory)}: Id = {id}, status = {status}");
            var postCategory = await _dbContext.PostCategories.FirstOrDefaultAsync(pc => pc.Id == id && !pc.Deleted && pc.Status == CommonStatus.ACTIVE)
                ?? throw new UserFriendlyException(ErrorCode.PostCategoryNotFound);
            postCategory.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }
}
