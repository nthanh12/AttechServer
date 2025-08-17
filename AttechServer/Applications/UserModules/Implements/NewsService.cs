using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.News;
using AttechServer.Applications.UserModules.Dtos.Attachment;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Abstractions;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace AttechServer.Applications.UserModules.Implements
{
    public class NewsService : INewsService
    {
        private const int MAX_CONTENT_LENGTH = 100000; // 100KB
        private readonly ILogger<NewsService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWysiwygFileProcessor _wysiwygFileProcessor;
        private readonly IActivityLogService _activityLogService;
        private readonly IAttachmentService _attachmentService;

        public NewsService(ApplicationDbContext dbContext, ILogger<NewsService> logger, IHttpContextAccessor httpContextAccessor, IWysiwygFileProcessor wysiwygFileProcessor, IActivityLogService activityLogService, IAttachmentService attachmentService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _wysiwygFileProcessor = wysiwygFileProcessor;
            _activityLogService = activityLogService;
            _attachmentService = attachmentService;
        }

        private string TruncateDescription(string description, int maxLength = 160)
        {
            if (string.IsNullOrEmpty(description)) return string.Empty;
            return description.Length <= maxLength ? description : description.Substring(0, maxLength - 3) + "...";
        }

        public async Task<NewsDto> Create(CreateNewsDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: Creating news with all data in one atomic operation");
            
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Step 1: Validate input
                    if (string.IsNullOrWhiteSpace(input.TitleVi) || string.IsNullOrWhiteSpace(input.ContentVi))
                    {
                        throw new ArgumentException("Tiêu đề và nội dung là bắt buộc.");
                    }

                    if (!string.IsNullOrEmpty(input.DescriptionVi) && input.DescriptionVi.Length > 700)
                    {
                        input.DescriptionVi = input.DescriptionVi.Substring(0, 697) + "...";
                    }
                    if (!string.IsNullOrEmpty(input.DescriptionEn) && input.DescriptionEn.Length > 700)
                    {
                        input.DescriptionEn = input.DescriptionEn.Substring(0, 697) + "...";
                    }

                    if (input.TimePosted > DateTime.Now)
                    {
                        throw new ArgumentException("Thời gian đăng bài không phù hợp.");
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value, out var id) ? id : 0;

                    // Step 2: Validate category exists
                    var category = await _dbContext.NewsCategories
                        .Where(c => c.Id == input.NewsCategoryId && !c.Deleted)
                        .Select(c => new { c.Id, c.TitleVi, c.TitleEn, c.SlugVi, c.SlugEn })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.NewsCategoryNotFound);
                    }

                    // Step 3: Check for duplicate titles
                    var titleViExists = await _dbContext.News.AnyAsync(n => n.TitleVi == input.TitleVi && !n.Deleted);
                    if (titleViExists)
                    {
                        throw new ArgumentException("Tiêu đề tiếng Việt đã tồn tại.");
                    }
                    
                    if (!string.IsNullOrEmpty(input.TitleEn))
                    {
                        var titleEnExists = await _dbContext.News.AnyAsync(n => n.TitleEn == input.TitleEn && !n.Deleted);
                        if (titleEnExists)
                        {
                            throw new ArgumentException("Tiêu đề tiếng Anh đã tồn tại.");
                        }
                    }

                    // Step 4: Sanitize content
                    var sanitizedContentVi = SanitizeContent(input.ContentVi);
                    var sanitizedContentEn = SanitizeContent(input.ContentEn ?? string.Empty);

                    // Step 5: Create news entity
                    var newNews = new News
                    {
                        SlugVi = input.SlugVi,
                        SlugEn = input.SlugEn,
                        TitleVi = input.TitleVi.Trim(),
                        TitleEn = input.TitleEn?.Trim() ?? string.Empty,
                        DescriptionVi = input.DescriptionVi?.Trim() ?? string.Empty,
                        DescriptionEn = input.DescriptionEn?.Trim() ?? string.Empty,
                        ContentVi = sanitizedContentVi,
                        ContentEn = sanitizedContentEn,
                        TimePosted = input.TimePosted,
                        Status = input.Status,
                        IsOutstanding = input.IsOutstanding,
                        NewsCategoryId = input.NewsCategoryId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        Deleted = false
                    };

                    _dbContext.News.Add(newNews);
                    await _dbContext.SaveChangesAsync();

                    // Step 6: Smart content processing - extract unique attachment IDs first
                    var contentAttachmentIds = await _wysiwygFileProcessor.ExtractUniqueAttachmentIdsAsync(newNews.ContentVi, newNews.ContentEn);
                    
                    // Associate content attachments first
                    if (contentAttachmentIds.Any())
                    {
                        await _attachmentService.AssociateAttachmentsAsync(contentAttachmentIds, ObjectType.News, newNews.Id, isFeaturedImage: false, isContentImage: true);
                    }
                    
                    // Process both content - now attachments are permanent
                    var (processedContentVi, fileEntriesVi) = await _wysiwygFileProcessor.ProcessContentAsync(newNews.ContentVi, ObjectType.News, newNews.Id);
                    var (processedContentEn, fileEntriesEn) = await _wysiwygFileProcessor.ProcessContentAsync(newNews.ContentEn, ObjectType.News, newNews.Id);
                    
                    // Update content with processed paths
                    newNews.ContentVi = processedContentVi;
                    newNews.ContentEn = processedContentEn;

                    // Step 7: Finalize gallery attachments (IsPrimary = false) - exclude content attachments
                    if (input.AttachmentIds != null && input.AttachmentIds.Any())
                    {
                        try
                        {
                            // Gallery attachments = attachmentIds - contentAttachmentIds (to avoid duplicates)
                            var galleryAttachmentIds = input.AttachmentIds.Except(contentAttachmentIds).ToList();
                            if (galleryAttachmentIds.Any())
                            {
                                await _attachmentService.AssociateAttachmentsAsync(galleryAttachmentIds, ObjectType.News, newNews.Id, isFeaturedImage: false, isContentImage: false);
                                _logger.LogInformation($"Finalized {galleryAttachmentIds.Count} gallery attachments for news ID: {newNews.Id}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error finalizing gallery attachments for news ID: {newNews.Id}");
                            throw new UserFriendlyException(ErrorCode.InvalidClientRequest);
                        }
                    }

                    // Step 8: Handle featured image (IsPrimary = true)
                    if (input.FeaturedImageId.HasValue)
                    {
                        try
                        {
                            await _attachmentService.AssociateAttachmentsAsync(new List<int> { input.FeaturedImageId.Value }, ObjectType.News, newNews.Id, isFeaturedImage: true, isContentImage: false);
                            
                            // ImageUrl will be set automatically by AttachmentService to /uploads/images/yyyy/MM/filename.ext
                            // No need to override it here
                            
                            _logger.LogInformation($"Finalized featured image {input.FeaturedImageId.Value} for news ID: {newNews.Id}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error finalizing featured image for news ID: {newNews.Id}");
                            throw new UserFriendlyException(ErrorCode.InvalidClientRequest);
                        }
                    }

                    // Step 9: Save all changes and commit transaction
                    await _dbContext.SaveChangesAsync();
                    
                    // Log activity
                    await _activityLogService.LogAsync("NEWS_CREATE", "Tạo tin tức với file đính kèm", newNews.TitleVi, "Info");
                    
                    await transaction.CommitAsync();

                    // Step 10: Return NewsDto
                    var response = new NewsDto
                    {
                        Id = newNews.Id,
                        SlugVi = newNews.SlugVi,
                        SlugEn = newNews.SlugEn,
                        TitleVi = newNews.TitleVi,
                        TitleEn = newNews.TitleEn,
                        DescriptionVi = newNews.DescriptionVi,
                        DescriptionEn = newNews.DescriptionEn,
                        NewsCategoryId = newNews.NewsCategoryId,
                        TimePosted = newNews.TimePosted,
                        Status = newNews.Status,
                        IsOutstanding = newNews.IsOutstanding
                    };

                    _logger.LogInformation($"Successfully created news. NewsId: {newNews.Id}");
                    return response;
                }
                catch (UserFriendlyException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error creating news with attachments: {ex.Message}");
                    throw new UserFriendlyException(ErrorCode.InvalidClientRequest);
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
                    var news = await _dbContext.News
                        .FirstOrDefaultAsync(n => n.Id == id && !n.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.NewsNotFound);

                    // Delete all associated files (featured, album, attachments)
                    await DeleteAssociatedFilesAsync(news.Id);

                    // Delete WYSIWYG files
                    await _wysiwygFileProcessor.DeleteFilesAsync(ObjectType.News, news.Id);

                    news.Deleted = true;
                    await _dbContext.SaveChangesAsync();
                    
                    // Log activity
                    await _activityLogService.LogAsync("NEWS_DELETE", "Xóa tin tức", news.TitleVi, "Info");
                    
                    await transaction.CommitAsync();
                    
                    _logger.LogInformation($"Successfully deleted news ID: {id} and all associated files");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error deleting news with id = {id}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete all files associated with a news article
        /// </summary>
        private async Task DeleteAssociatedFilesAsync(int newsId)
        {
            try
            {
                // Get all files associated with this news
                var associatedFiles = await _dbContext.Attachments
                    .Where(f => f.ObjectType == ObjectType.News && f.ObjectId == newsId && !f.Deleted)
                    .ToListAsync();

                if (associatedFiles.Any())
                {
                    _logger.LogInformation($"Found {associatedFiles.Count} files to delete for news ID: {newsId}");

                    foreach (var file in associatedFiles)
                    {
                        try
                        {
                            // Delete physical file
                            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "AttechServer", "Uploads", file.FilePath);
                            if (File.Exists(fullPath))
                            {
                                File.Delete(fullPath);
                                _logger.LogInformation($"Deleted physical file: {file.FilePath}");
                            }

                            // Mark file as deleted in database
                            file.Deleted = true;
                            file.ModifiedDate = DateTime.Now;
                            file.ModifiedBy = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value, out var userIdValue) ? userIdValue : 0;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Could not delete file {file.FilePath}. Continuing with other files.");
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting associated files for news ID: {newsId}");
                throw;
            }
        }

        public async Task<PagingResult<NewsDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.News.AsNoTracking()
                .Where(n => !n.Deleted && n.IsAlbum == false); // CHỈ lấy News thực, không lấy Albums

            // Tìm kiếm
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                baseQuery = baseQuery.Where(n =>
                    n.TitleVi.Contains(input.Keyword) ||
                    n.DescriptionVi.Contains(input.Keyword) ||
                    n.ContentVi.Contains(input.Keyword) ||
                    n.TitleEn.Contains(input.Keyword) ||
                    n.DescriptionEn.Contains(input.Keyword) ||
                    n.ContentEn.Contains(input.Keyword));
            }

            var totalItems = await baseQuery.CountAsync();

            // Sắp xếp
            var query = baseQuery.OrderByDescending(n => n.TimePosted);
            if (input.Sort.Any())
            {
                // TODO: Implement dynamic sorting based on input.Sort
            }

            var pagedItems = await query
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    SlugVi = n.SlugVi,
                    SlugEn = n.SlugEn,
                    TitleVi = n.TitleVi,
                    TitleEn = n.TitleEn,
                    DescriptionVi = n.DescriptionVi,
                    DescriptionEn = n.DescriptionEn,
                    TimePosted = n.TimePosted,
                    Status = n.Status,
                    NewsCategoryId = n.NewsCategoryId,
                    NewsCategoryTitleVi = n.NewsCategory.TitleVi,
                    NewsCategoryTitleEn = n.NewsCategory.TitleEn,
                    NewsCategorySlugVi = n.NewsCategory.SlugVi,
                    NewsCategorySlugEn = n.NewsCategory.SlugEn,
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl
                })
                .ToListAsync();

            return new PagingResult<NewsDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<PagingResult<NewsDto>> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId)
        {
            _logger.LogInformation($"{nameof(FindAllByCategoryId)}: input = {JsonSerializer.Serialize(input)}, categoryId = {categoryId}");

            // Kiểm tra danh mục tồn tại
            var exists = await _dbContext.NewsCategories
                .AnyAsync(c => c.Id == categoryId && !c.Deleted);
            if (!exists)
                throw new UserFriendlyException(ErrorCode.NewsCategoryNotFound);

            // Build query
            var baseQuery = _dbContext.News.AsNoTracking()
                .Where(n => !n.Deleted && n.IsAlbum == false && n.NewsCategoryId == categoryId); // CHỈ lấy News thực

            // Tìm kiếm
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                baseQuery = baseQuery.Where(n =>
                    n.TitleVi.Contains(input.Keyword) ||
                    n.DescriptionVi.Contains(input.Keyword) ||
                    n.ContentVi.Contains(input.Keyword) ||
                    n.TitleEn.Contains(input.Keyword) ||
                    n.DescriptionEn.Contains(input.Keyword) ||
                    n.ContentEn.Contains(input.Keyword));
            }

            var totalItems = await baseQuery.CountAsync();

            // Phân trang và sắp xếp
            var items = await baseQuery
                .OrderByDescending(n => n.TimePosted)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    SlugVi = n.SlugVi,
                    SlugEn = n.SlugEn,
                    TitleVi = n.TitleVi,
                    TitleEn = n.TitleEn,
                    DescriptionVi = n.DescriptionVi,
                    DescriptionEn = n.DescriptionEn,
                    TimePosted = n.TimePosted,
                    Status = n.Status,
                    NewsCategoryId = n.NewsCategoryId,
                    NewsCategoryTitleVi = n.NewsCategory.TitleVi,
                    NewsCategoryTitleEn = n.NewsCategory.TitleEn,
                    NewsCategorySlugVi = n.NewsCategory.SlugVi,
                    NewsCategorySlugEn = n.NewsCategory.SlugEn,
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl
                })
                .ToListAsync();

            return new PagingResult<NewsDto>
            {
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<PagingResult<NewsDto>> FindAllByCategorySlug(PagingRequestBaseDto input, string slug)
        {
            // 1. Tìm category theo slug
            var category = await _dbContext.NewsCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.SlugVi == slug && !c.Deleted);
            if (category == null)
                throw new UserFriendlyException(ErrorCode.NewsCategoryNotFound);

            // 2. Delegate về hàm cũ để xử lý paging
            return await FindAllByCategoryId(input, category.Id);
        }

        public async Task<DetailNewsDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var news = await _dbContext.News
                .Where(n => n.Id == id && !n.Deleted)
                .Select(n => new DetailNewsDto
                {
                    Id = n.Id,
                    SlugVi = n.SlugVi,
                    TitleVi = n.TitleVi,
                    DescriptionVi = n.DescriptionVi,
                    ContentVi = n.ContentVi,
                    SlugEn = n.SlugEn,
                    TitleEn = n.TitleEn,
                    DescriptionEn = n.DescriptionEn,
                    ContentEn = n.ContentEn,
                    TimePosted = n.TimePosted,
                    Status = n.Status,
                    NewsCategoryId = n.NewsCategoryId,
                    NewsCategoryTitleVi = n.NewsCategory.TitleVi,
                    NewsCategoryTitleEn = n.NewsCategory.TitleEn,
                    NewsCategorySlugVi = n.NewsCategory.SlugVi,
                    NewsCategorySlugEn = n.NewsCategory.SlugEn,
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl,
                    FeaturedImageId = n.FeaturedImageId
                })
                .FirstOrDefaultAsync();

            if (news == null)
            {
                throw new UserFriendlyException(ErrorCode.NewsNotFound);
            }

            // Load attachments
            var attachments = await _dbContext.Attachments
                .Where(a => a.ObjectType == ObjectType.News && a.ObjectId == id && !a.Deleted)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FilePath = a.FilePath,
                    Url = a.Url,
                    OriginalFileName = a.OriginalFileName,
                    FileSize = a.FileSize,
                    ContentType = a.ContentType,
                    ObjectType = a.ObjectType,
                    ObjectId = a.ObjectId,
                    RelationType = a.RelationType,
                    IsPrimary = a.IsPrimary,
                    IsContentImage = a.IsContentImage,
                    IsTemporary = a.IsTemporary,
                    CreatedDate = a.CreatedDate
                })
                .ToListAsync();

            // Group attachments by type
            news.Attachments = new AttachmentsGroupDto
            {
                Images = attachments.Where(a => a.ContentType.StartsWith("image/") && !a.IsPrimary && !a.IsContentImage).ToList(),
                Documents = attachments.Where(a => !a.ContentType.StartsWith("image/")).ToList()
            };

            return news;
        }

        public async Task<DetailNewsDto> FindBySlug(string slug)
        {
            _logger.LogInformation($"{nameof(FindBySlug)}: slug = {slug}");
            var news = await _dbContext.News
                .Where(n => (n.SlugVi == slug || n.SlugEn == slug) && !n.Deleted && n.IsAlbum == false) // CHỈ lấy News thực
                .Select(n => new DetailNewsDto
                {
                    Id = n.Id,
                    TitleVi = n.TitleVi,
                    TitleEn = n.TitleEn,
                    SlugVi = n.SlugVi,
                    SlugEn = n.SlugEn,
                    DescriptionVi = n.DescriptionVi,
                    DescriptionEn = n.DescriptionEn,
                    ContentVi = n.ContentVi,
                    ContentEn = n.ContentEn,
                    TimePosted = n.TimePosted,
                    Status = n.Status,
                    NewsCategoryId = n.NewsCategoryId,
                    NewsCategoryTitleVi = n.NewsCategory.TitleVi,
                    NewsCategoryTitleEn = n.NewsCategory.TitleEn,
                    NewsCategorySlugVi = n.NewsCategory.SlugVi,
                    NewsCategorySlugEn = n.NewsCategory.SlugEn,
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (news == null)
                throw new UserFriendlyException(ErrorCode.NewsNotFound);

            // Load attachments
            var attachments = await _dbContext.Attachments
                .Where(a => a.ObjectType == ObjectType.News && a.ObjectId == news.Id && !a.Deleted)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FilePath = a.FilePath,
                    Url = a.Url,
                    OriginalFileName = a.OriginalFileName,
                    FileSize = a.FileSize,
                    ContentType = a.ContentType,
                    ObjectType = a.ObjectType,
                    ObjectId = a.ObjectId,
                    RelationType = a.RelationType,
                    IsPrimary = a.IsPrimary,
                    IsContentImage = a.IsContentImage,
                    IsTemporary = a.IsTemporary,
                    CreatedDate = a.CreatedDate
                })
                .ToListAsync();

            // Group attachments by type
            news.Attachments = new AttachmentsGroupDto
            {
                Images = attachments.Where(a => a.ContentType.StartsWith("image/") && !a.IsPrimary && !a.IsContentImage).ToList(),
                Documents = attachments.Where(a => !a.ContentType.StartsWith("image/")).ToList()
            };

            return news;
        }

        public async Task<NewsDto> Update(UpdateNewsDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.TitleVi) || string.IsNullOrWhiteSpace(input.ContentVi))
                    {
                        throw new ArgumentException("Tiêu đề và nội dung là bắt buộc.");
                    }
                    if (input.TimePosted > DateTime.Now)
                    {
                        throw new ArgumentException("Thời gian đăng bài không phù hợp.");
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value, out var id) ? id : 0;

                    // Kiểm tra news tồn tại
                    var news = await _dbContext.News
                        .FirstOrDefaultAsync(n => n.Id == input.Id && !n.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.NewsNotFound);

                    // Check for duplicate titles (excluding current record)
                    var titleViExists = await _dbContext.News.AnyAsync(n => n.TitleVi == input.TitleVi && n.Id != input.Id && !n.Deleted);
                    if (titleViExists)
                    {
                        throw new ArgumentException("Tiêu đề tiếng Việt đã tồn tại.");
                    }
                    
                    if (!string.IsNullOrEmpty(input.TitleEn))
                    {
                        var titleEnExists = await _dbContext.News.AnyAsync(n => n.TitleEn == input.TitleEn && n.Id != input.Id && !n.Deleted);
                        if (titleEnExists)
                        {
                            throw new ArgumentException("Tiêu đề tiếng Anh đã tồn tại.");
                        }
                    }

                    // Kiểm tra danh mục
                    var category = await _dbContext.NewsCategories
                        .Where(c => c.Id == input.NewsCategoryId && !c.Deleted)
                        .Select(c => new { c.Id, c.TitleVi, c.TitleEn, c.SlugVi, c.SlugEn })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.NewsCategoryNotFound);
                    }

                    // Update basic fields
                    news.TitleVi = input.TitleVi;
                    news.TitleEn = input.TitleEn;
                    news.SlugVi = input.SlugVi;
                    news.SlugEn = input.SlugEn;
                    news.DescriptionVi = input.DescriptionVi;
                    news.DescriptionEn = input.DescriptionEn;
                    news.ContentVi = input.ContentVi;
                    news.ContentEn = input.ContentEn;
                    news.NewsCategoryId = input.NewsCategoryId;
                    news.TimePosted = input.TimePosted;
                    news.Status = input.Status;
                    news.IsOutstanding = input.IsOutstanding;
                    news.ModifiedBy = userId;

                    // Simple attachment update logic - compare current vs desired state
                    var currentGalleryAttachmentIds = await _attachmentService.GetCurrentGalleryAttachmentIdsAsync(ObjectType.News, news.Id);
                    var currentFeaturedImageId = await _attachmentService.GetCurrentFeaturedImageIdAsync(ObjectType.News, news.Id);
                    
                    var desiredGalleryAttachmentIds = (input.AttachmentIds ?? new List<int>()).OrderBy(x => x).ToList();
                    var desiredFeaturedImageId = input.FeaturedImageId;
                    
                    // Check if gallery attachments changed
                    bool galleryChanged = !currentGalleryAttachmentIds.SequenceEqual(desiredGalleryAttachmentIds);
                    bool featuredImageChanged = currentFeaturedImageId != desiredFeaturedImageId;
                    
                    // Check if content has new temp files
                    bool hasNewContentFiles = _wysiwygFileProcessor.HasTempFilesInContent(input.ContentVi) || 
                                             _wysiwygFileProcessor.HasTempFilesInContent(input.ContentEn ?? string.Empty);
                    
                    if (galleryChanged || featuredImageChanged || hasNewContentFiles)
                    {
                        // Something changed → Soft delete ALL old attachments and recreate
                        await _attachmentService.SoftDeleteEntityAttachmentsAsync(ObjectType.News, news.Id);
                        
                        // Process content attachments first - extract unique IDs
                        var contentAttachmentIds = await _wysiwygFileProcessor.ExtractUniqueAttachmentIdsAsync(news.ContentVi, news.ContentEn);
                        if (contentAttachmentIds.Any())
                        {
                            await _attachmentService.AssociateAttachmentsAsync(contentAttachmentIds, ObjectType.News, news.Id, isFeaturedImage: false, isContentImage: true);
                        }
                        
                        // Process both content - now attachments are permanent
                        var (processedContentVi, fileEntriesVi) = await _wysiwygFileProcessor.ProcessContentAsync(news.ContentVi, ObjectType.News, news.Id);
                        var (processedContentEn, fileEntriesEn) = await _wysiwygFileProcessor.ProcessContentAsync(news.ContentEn, ObjectType.News, news.Id);
                        
                        // Update content with processed paths
                        news.ContentVi = processedContentVi;
                        news.ContentEn = processedContentEn;
                        
                        // Handle gallery attachments - exclude content attachments
                        if (desiredGalleryAttachmentIds.Any())
                        {
                            // Gallery attachments = desiredGalleryAttachmentIds - contentAttachmentIds (to avoid duplicates)
                            var pureGalleryAttachmentIds = desiredGalleryAttachmentIds.Except(contentAttachmentIds).ToList();
                            if (pureGalleryAttachmentIds.Any())
                            {
                                await _attachmentService.AssociateAttachmentsAsync(pureGalleryAttachmentIds, ObjectType.News, news.Id, isFeaturedImage: false, isContentImage: false);
                            }
                        }
                        
                        // Handle featured image
                        if (desiredFeaturedImageId.HasValue)
                        {
                            await _attachmentService.AssociateAttachmentsAsync(new List<int> { desiredFeaturedImageId.Value }, ObjectType.News, news.Id, isFeaturedImage: true, isContentImage: false);
                        }
                        else
                        {
                            // Clear featured image if not provided
                            news.FeaturedImageId = null;
                            news.ImageUrl = string.Empty;
                        }
                    }
                    // If nothing changed → keep everything as is


                    await _dbContext.SaveChangesAsync();
                    
                    // Log activity
                    await _activityLogService.LogAsync("NEWS_UPDATE", "Cập nhật tin tức", news.TitleVi, "Info");
                    
                    await transaction.CommitAsync();

                    return new NewsDto
                    {
                        Id = news.Id,
                        TitleVi = news.TitleVi,
                        TitleEn = news.TitleEn,
                        SlugVi = news.SlugVi,
                        SlugEn = news.SlugEn,
                        DescriptionVi = news.DescriptionVi,
                        DescriptionEn = news.DescriptionEn,
                        TimePosted = news.TimePosted,
                        Status = news.Status,
                        NewsCategoryId = news.NewsCategoryId,
                        NewsCategoryTitleVi = category.TitleVi,
                        NewsCategoryTitleEn = category.TitleEn,
                        NewsCategorySlugVi = category.SlugVi,
                        NewsCategorySlugEn = category.SlugEn,
                        IsOutstanding = news.IsOutstanding,
                        ImageUrl = news.ImageUrl
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error updating news with id = {input.Id}");
                    throw;
                }
                finally
                {
                    // Temp files will be cleaned by background service
                }
            }
        }

        public async Task<NewsDto> CreateAlbum(CreateAlbumDto input)
        {
            _logger.LogInformation($"{nameof(CreateAlbum)}: Creating album {input.TitleVi}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("user_id")?.Value;
                var userId = int.TryParse(userIdClaim, out int parsedUserId) ? parsedUserId : 0;
                
                // Validate category exists for album
                var category = await _dbContext.NewsCategories
                    .Where(c => c.Id == input.NewsCategoryId && !c.Deleted)
                    .FirstOrDefaultAsync();
                if (category == null)
                {
                    throw new UserFriendlyException(ErrorCode.NewsCategoryNotFound);
                }
                
                // Create news with IsAlbum = true
                var newNews = new News
                {
                    SlugVi = SlugHelper.GenerateSlug(input.TitleVi),
                    SlugEn = SlugHelper.GenerateSlug(input.TitleEn),
                    TitleVi = input.TitleVi,
                    TitleEn = input.TitleEn,
                    DescriptionVi = "", // Empty for albums
                    DescriptionEn = "", // Empty for albums
                    ContentVi = "", // Empty for albums
                    ContentEn = "", // Empty for albums
                    NewsCategoryId = input.NewsCategoryId,
                    TimePosted = DateTime.UtcNow,
                    Status = 1, // Active
                    IsOutstanding = false,
                    IsAlbum = true, // Mark as album
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _dbContext.News.Add(newNews);
                await _dbContext.SaveChangesAsync();

                // Finalize attachments and set featured image
                if (input.AttachmentIds != null && input.AttachmentIds.Any())
                {
                    await _attachmentService.AssociateAttachmentsAsync(input.AttachmentIds, ObjectType.News, newNews.Id, isFeaturedImage: false, isContentImage: false);
                    _logger.LogInformation($"Finalized {input.AttachmentIds.Count} gallery attachments for album ID: {newNews.Id}");

                    // Auto-set first image as featured image
                    var featuredImageId = input.AttachmentIds.First();
                    await _attachmentService.AssociateAttachmentsAsync(new List<int> { featuredImageId }, ObjectType.News, newNews.Id, isFeaturedImage: true, isContentImage: false);
                    _logger.LogInformation($"Auto-set featured image {featuredImageId} (first attachment) for album ID: {newNews.Id}");
                }

                await transaction.CommitAsync();

                // Return NewsDto
                return new NewsDto
                {
                    Id = newNews.Id,
                    SlugVi = newNews.SlugVi,
                    SlugEn = newNews.SlugEn,
                    TitleVi = newNews.TitleVi,
                    TitleEn = newNews.TitleEn,
                    DescriptionVi = newNews.DescriptionVi,
                    DescriptionEn = newNews.DescriptionEn,
                    NewsCategoryId = newNews.NewsCategoryId,
                    TimePosted = newNews.TimePosted,
                    Status = newNews.Status,
                    IsOutstanding = newNews.IsOutstanding
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error creating album: {input.TitleVi}");
                throw;
            }
        }

        public async Task<PagingResult<NewsDto>> GetAlbums(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(GetAlbums)}: Getting albums with pagination");

            var query = _dbContext.News
                .Where(n => !n.Deleted && n.IsAlbum == true)
                .Include(n => n.NewsCategory)
                .OrderByDescending(n => n.TimePosted);

            var totalItems = await query.CountAsync();
            
            var albums = await query
                .Skip((input.PageNumber - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    SlugVi = n.SlugVi,
                    SlugEn = n.SlugEn,
                    TitleVi = n.TitleVi,
                    TitleEn = n.TitleEn,
                    DescriptionVi = n.DescriptionVi,
                    DescriptionEn = n.DescriptionEn,
                    NewsCategoryId = n.NewsCategoryId,
                    NewsCategoryTitleVi = n.NewsCategory.TitleVi,
                    NewsCategoryTitleEn = n.NewsCategory.TitleEn,
                    TimePosted = n.TimePosted,
                    Status = n.Status,
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl
                })
                .ToListAsync();

            return new PagingResult<NewsDto>
            {
                Items = albums,
                TotalItems = totalItems,
            };
        }

        public async Task<DetailNewsDto> FindAlbumBySlug(string slug)
        {
            _logger.LogInformation($"{nameof(FindAlbumBySlug)}: slug = {slug}");
            var album = await _dbContext.News
                .Where(n => (n.SlugVi == slug || n.SlugEn == slug) && !n.Deleted && n.IsAlbum == true) // CHỈ lấy Albums
                .Select(n => new DetailNewsDto
                {
                    Id = n.Id,
                    TitleVi = n.TitleVi,
                    TitleEn = n.TitleEn,
                    SlugVi = n.SlugVi,
                    SlugEn = n.SlugEn,
                    DescriptionVi = n.DescriptionVi,
                    DescriptionEn = n.DescriptionEn,
                    ContentVi = n.ContentVi,
                    ContentEn = n.ContentEn,
                    NewsCategoryId = n.NewsCategoryId,
                    NewsCategoryTitleVi = n.NewsCategory.TitleVi,
                    NewsCategoryTitleEn = n.NewsCategory.TitleEn,
                    TimePosted = n.TimePosted,
                    Status = n.Status,
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (album == null)
                throw new UserFriendlyException(ErrorCode.NewsNotFound);

            return album;
        }

        public async Task<NewsGalleryDto> GetGalleryBySlug(string slug)
        {
            _logger.LogInformation($"{nameof(GetGalleryBySlug)}: slug = {slug}");
            
            // Tìm News theo slug
            var news = await _dbContext.News
                .Where(n => (n.SlugVi == slug || n.SlugEn == slug) && !n.Deleted)
                .Select(n => new 
                {
                    n.Id,
                    n.TitleVi,
                    n.TitleEn,
                    n.SlugVi,
                    n.SlugEn
                })
                .FirstOrDefaultAsync();

            if (news == null)
                throw new UserFriendlyException(ErrorCode.NewsNotFound);

            // Lấy TẤT CẢ attachments của News này
            var attachments = await _dbContext.Attachments
                .Where(a => a.ObjectType == ObjectType.News 
                          && a.ObjectId == news.Id 
                          && !a.Deleted 
                          && !a.IsTemporary)
                .OrderBy(a => a.IsPrimary ? 0 : 1)
                .ThenBy(a => a.OrderIndex)
                .ThenBy(a => a.CreatedDate)
                .ToListAsync();

            return new NewsGalleryDto
            {
                Id = news.Id,
                TitleVi = news.TitleVi,
                TitleEn = news.TitleEn,
                SlugVi = news.SlugVi,
                SlugEn = news.SlugEn,
                FeaturedImage = null, // Không phân biệt featured
                GalleryImages = attachments.Select(a => new NewsImageDto
                {
                    Id = a.Id,
                    FilePath = a.Url,
                    OriginalFileName = a.OriginalFileName,
                    FileSize = a.FileSize,
                    ContentType = a.ContentType
                }).ToList()
            };
        }

        private string SanitizeContent(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Add("img");
            sanitizer.AllowedAttributes.Add("src");
            sanitizer.AllowedAttributes.Add("alt");
            sanitizer.AllowedAttributes.Add("class");

            return sanitizer.Sanitize(content);
        }


    }
} 
