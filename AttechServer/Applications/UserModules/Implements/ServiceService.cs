using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Service;
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
    public class ServiceService : IServiceService
    {
        private const int MAX_CONTENT_LENGTH = 100000; // 100KB
        private readonly ILogger<ServiceService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWysiwygFileProcessor _wysiwygFileProcessor;
        private readonly IActivityLogService _activityLogService;
        private readonly IAttachmentService _attachmentService;

        public ServiceService(ApplicationDbContext dbContext, ILogger<ServiceService> logger, IHttpContextAccessor httpContextAccessor, IWysiwygFileProcessor wysiwygFileProcessor, IActivityLogService activityLogService, IAttachmentService attachmentService)
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

        public async Task<ServiceDto> Create(CreateServiceDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: Creating service with all data in one atomic operation");

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

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value, out var parseId) ? parseId : 0;


                    // Step 3: Check for duplicate titles
                    var titleViExists = await _dbContext.Services.AnyAsync(n => n.TitleVi == input.TitleVi && !n.Deleted);
                    if (titleViExists)
                    {
                        throw new ArgumentException("Tiêu đề tiếng Việt đã tồn tại.");
                    }

                    if (!string.IsNullOrEmpty(input.TitleEn))
                    {
                        var titleEnExists = await _dbContext.Services.AnyAsync(n => n.TitleEn == input.TitleEn && !n.Deleted);
                        if (titleEnExists)
                        {
                            throw new ArgumentException("Tiêu đề tiếng Anh đã tồn tại.");
                        }
                    }

                    // Step 4: Sanitize content
                    var sanitizedContentVi = SanitizeContent(input.ContentVi);
                    var sanitizedContentEn = SanitizeContent(input.ContentEn ?? string.Empty);

                    // Step 5: Create service entity
                    var newService = new Service
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
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        Deleted = false
                    };

                    _dbContext.Services.Add(newService);
                    await _dbContext.SaveChangesAsync();

                    // Step 6: Smart content processing - extract unique attachment IDs first
                    var contentAttachmentIds = await _wysiwygFileProcessor.ExtractUniqueAttachmentIdsAsync(newService.ContentVi, newService.ContentEn);
                    
                    // Associate content attachments first
                    if (contentAttachmentIds.Any())
                    {
                        await _attachmentService.AssociateAttachmentsAsync(contentAttachmentIds, ObjectType.Service, newService.Id, isFeaturedImage: false, isContentImage: true);
                    }
                    
                    // Process both content - now attachments are permanent
                    var (processedContentVi, fileEntriesVi) = await _wysiwygFileProcessor.ProcessContentAsync(newService.ContentVi, ObjectType.Service, newService.Id);
                    var (processedContentEn, fileEntriesEn) = await _wysiwygFileProcessor.ProcessContentAsync(newService.ContentEn, ObjectType.Service, newService.Id);
                    
                    // Update content with processed paths
                    newService.ContentVi = processedContentVi;
                    newService.ContentEn = processedContentEn;

                    // Step 7: Finalize gallery attachments (IsPrimary = false) - exclude content attachments
                    if (input.AttachmentIds != null && input.AttachmentIds.Any())
                    {
                        try
                        {
                            
                            // Gallery attachments = attachmentIds - contentAttachmentIds (to avoid duplicates)
                            var galleryAttachmentIds = input.AttachmentIds.Except(contentAttachmentIds).ToList();
                            if (galleryAttachmentIds.Any())
                            {
                                await _attachmentService.AssociateAttachmentsAsync(galleryAttachmentIds, ObjectType.Service, newService.Id, isFeaturedImage: false, isContentImage: false);
                                _logger.LogInformation($"Finalized {galleryAttachmentIds.Count} gallery attachments for service ID: {newService.Id}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error finalizing gallery attachments for service ID: {newService.Id}");
                            throw new UserFriendlyException(ErrorCode.InvalidClientRequest);
                        }
                    }

                    // Step 7.5: Handle featured image (IsPrimary = true)
                    if (input.FeaturedImageId.HasValue)
                    {
                        try
                        {
                            await _attachmentService.AssociateAttachmentsAsync(new List<int> { input.FeaturedImageId.Value }, ObjectType.Service, newService.Id, isFeaturedImage: true, isContentImage: false);
                            
                            // ImageUrl will be set automatically by AttachmentService to /uploads/images/yyyy/MM/filename.ext
                            // No need to override it here
                            
                            _logger.LogInformation($"Finalized featured image {input.FeaturedImageId.Value} for service ID: {newService.Id}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error finalizing featured image for service ID: {newService.Id}");
                            throw new UserFriendlyException(ErrorCode.InvalidClientRequest);
                        }
                    }

                    // Step 9: Save all changes and commit transaction
                    await _dbContext.SaveChangesAsync();
                    
                    // Log activity
                    await _activityLogService.LogAsync("SERVICE_CREATE", "Tạo dịch vụ với file đính kèm", newService.TitleVi, "Info");
                    
                    await transaction.CommitAsync();

                    // Step 10: Return ServiceDto
                    var response = new ServiceDto
                    {
                        Id = newService.Id,
                        SlugVi = newService.SlugVi,
                        SlugEn = newService.SlugEn,
                        TitleVi = newService.TitleVi,
                        TitleEn = newService.TitleEn,
                        DescriptionVi = newService.DescriptionVi,
                        DescriptionEn = newService.DescriptionEn,
                        TimePosted = newService.TimePosted,
                        Status = newService.Status,
                        IsOutstanding = newService.IsOutstanding
                    };

                    _logger.LogInformation($"Successfully created service. ServiceId: {newService.Id}");
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
                    _logger.LogError(ex, $"Error creating service with attachments: {ex.Message}");
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
                    var service = await _dbContext.Services
                        .FirstOrDefaultAsync(n => n.Id == id && !n.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.ServiceNotFound);

                    // Delete all associated files (featured, album, attachments)
                    await DeleteAssociatedFilesAsync(service.Id);

                    // Delete WYSIWYG files
                    await _wysiwygFileProcessor.DeleteFilesAsync(ObjectType.Service, service.Id);

                    service.Deleted = true;
                    await _dbContext.SaveChangesAsync();

                    // Log activity
                    await _activityLogService.LogAsync("SERVICE_DELETE", "Xóa dịch vụ", service.TitleVi, "Info");

                    await transaction.CommitAsync();

                    _logger.LogInformation($"Successfully deleted service ID: {id} and all associated files");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error deleting service with id = {id}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete all files associated with a service article
        /// </summary>
        private async Task DeleteAssociatedFilesAsync(int serviceId)
        {
            try
            {
                // Get all files associated with this service
                var associatedFiles = await _dbContext.Attachments
                    .Where(f => f.ObjectType == ObjectType.Service && f.ObjectId == serviceId && !f.Deleted)
                    .ToListAsync();

                if (associatedFiles.Any())
                {
                    _logger.LogInformation($"Found {associatedFiles.Count} files to delete for service ID: {serviceId}");

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
                _logger.LogError(ex, $"Error deleting associated files for service ID: {serviceId}");
                throw;
            }
        }

        public async Task<PagingResult<ServiceDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.Services.AsNoTracking()
                .Where(n => !n.Deleted);

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
                .Select(n => new ServiceDto
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
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl
                })
                .ToListAsync();

            return new PagingResult<ServiceDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<DetailServiceDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var service = await _dbContext.Services
                .Where(n => n.Id == id && !n.Deleted)
                .Select(n => new DetailServiceDto
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
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl,
                    FeaturedImageId = n.FeaturedImageId
                })
                .FirstOrDefaultAsync();

            if (service == null)
            {
                throw new UserFriendlyException(ErrorCode.ServiceNotFound);
            }

            // Load attachments
            var attachments = await _dbContext.Attachments
                .Where(a => a.ObjectType == ObjectType.Service && a.ObjectId == id && !a.Deleted)
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
            service.Attachments = new AttachmentsGroupDto
            {
                Images = attachments.Where(a => a.ContentType.StartsWith("image/") && !a.IsPrimary && !a.IsContentImage).ToList(),
                Documents = attachments.Where(a => !a.ContentType.StartsWith("image/")).ToList()
            };

            return service;
        }

        public async Task<DetailServiceDto> FindBySlug(string slug)
        {
            _logger.LogInformation($"{nameof(FindBySlug)}: slug = {slug}");
            var service = await _dbContext.Services
                .Where(n => (n.SlugVi == slug || n.SlugEn == slug) && !n.Deleted)
                .Select(n => new DetailServiceDto
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
                    IsOutstanding = n.IsOutstanding,
                    ImageUrl = n.ImageUrl,
                    FeaturedImageId = n.FeaturedImageId
                })
                .FirstOrDefaultAsync();

            if (service == null)
                throw new UserFriendlyException(ErrorCode.ServiceNotFound);

            // Load attachments
            var attachments = await _dbContext.Attachments
                .Where(a => a.ObjectType == ObjectType.Service && a.ObjectId == service.Id && !a.Deleted)
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
            service.Attachments = new AttachmentsGroupDto
            {
                Images = attachments.Where(a => a.ContentType.StartsWith("image/") && !a.IsPrimary && !a.IsContentImage).ToList(),
                Documents = attachments.Where(a => !a.ContentType.StartsWith("image/")).ToList()
            };

            return service;
        }

        public async Task<ServiceDto> Update(int id, UpdateServiceDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: ID = {id}, input = {JsonSerializer.Serialize(input)}");
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

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value, out var parseId) ? parseId : 0;

                    // Kiểm tra service tồn tại
                    var service = await _dbContext.Services
                        .FirstOrDefaultAsync(n => n.Id == id && !n.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.ServiceNotFound);

                    // Check for duplicate titles (excluding current record)
                    var titleViExists = await _dbContext.Services.AnyAsync(n => n.TitleVi == input.TitleVi && n.Id != id && !n.Deleted);
                    if (titleViExists)
                    {
                        throw new ArgumentException("Tiêu đề tiếng Việt đã tồn tại.");
                    }

                    if (!string.IsNullOrEmpty(input.TitleEn))
                    {
                        var titleEnExists = await _dbContext.Services.AnyAsync(n => n.TitleEn == input.TitleEn && n.Id != id && !n.Deleted);
                        if (titleEnExists)
                        {
                            throw new ArgumentException("Tiêu đề tiếng Anh đã tồn tại.");
                        }
                    }

                    // Xóa các file cũ trong content nếu content thay đổi
                    if (service.ContentVi != input.ContentVi)
                    {
                        await _wysiwygFileProcessor.DeleteFilesAsync(ObjectType.Service, service.Id);
                    }

                    service.TitleVi = input.TitleVi;
                    service.TitleEn = input.TitleEn;
                    service.SlugVi = input.SlugVi;
                    service.SlugEn = input.SlugEn;
                    service.DescriptionVi = input.DescriptionVi;
                    service.DescriptionEn = input.DescriptionEn;
                    service.ContentVi = input.ContentVi;
                    service.ContentEn = input.ContentEn;
                    service.TimePosted = input.TimePosted;
                    service.Status = input.Status;
                    service.IsOutstanding = input.IsOutstanding;

                    service.ModifiedBy = userId;

                    // Simple attachment update logic - compare current vs desired state
                    var currentGalleryAttachmentIds = await _attachmentService.GetCurrentGalleryAttachmentIdsAsync(ObjectType.Service, service.Id);
                    var currentFeaturedImageId = await _attachmentService.GetCurrentFeaturedImageIdAsync(ObjectType.Service, service.Id);
                    
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
                        await _attachmentService.SoftDeleteEntityAttachmentsAsync(ObjectType.Service, service.Id);
                        
                        // Process content attachments first - extract unique IDs
                        var contentAttachmentIds = await _wysiwygFileProcessor.ExtractUniqueAttachmentIdsAsync(service.ContentVi, service.ContentEn);
                        if (contentAttachmentIds.Any())
                        {
                            await _attachmentService.AssociateAttachmentsAsync(contentAttachmentIds, ObjectType.Service, service.Id, isFeaturedImage: false, isContentImage: true);
                        }
                        
                        // Process both content - now attachments are permanent
                        var (processedContentVi, fileEntriesVi) = await _wysiwygFileProcessor.ProcessContentAsync(service.ContentVi, ObjectType.Service, service.Id);
                        var (processedContentEn, fileEntriesEn) = await _wysiwygFileProcessor.ProcessContentAsync(service.ContentEn, ObjectType.Service, service.Id);
                        
                        // Update content with processed paths
                        service.ContentVi = processedContentVi;
                        service.ContentEn = processedContentEn;
                        
                        // Handle gallery attachments - exclude content attachments
                        if (desiredGalleryAttachmentIds.Any())
                        {
                            // Gallery attachments = desiredGalleryAttachmentIds - contentAttachmentIds (to avoid duplicates)
                            var pureGalleryAttachmentIds = desiredGalleryAttachmentIds.Except(contentAttachmentIds).ToList();
                            if (pureGalleryAttachmentIds.Any())
                            {
                                await _attachmentService.AssociateAttachmentsAsync(pureGalleryAttachmentIds, ObjectType.Service, service.Id, isFeaturedImage: false, isContentImage: false);
                                _logger.LogInformation($"Updated {pureGalleryAttachmentIds.Count} gallery attachments for service ID: {service.Id}");
                            }
                        }
                        
                        // Handle featured image
                        if (desiredFeaturedImageId.HasValue)
                        {
                            try
                            {
                                await _attachmentService.AssociateAttachmentsAsync(new List<int> { desiredFeaturedImageId.Value }, ObjectType.Service, service.Id, isFeaturedImage: true, isContentImage: false);
                                _logger.LogInformation($"Updated featured image {desiredFeaturedImageId.Value} for service ID: {service.Id}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error finalizing featured image for service ID: {service.Id}");
                                throw new UserFriendlyException(ErrorCode.InvalidClientRequest);
                            }
                        }
                        else
                        {
                            // Clear featured image if not provided
                            service.FeaturedImageId = null;
                            service.ImageUrl = string.Empty;
                        }
                    }
                    // If nothing changed → keep everything as is

                    await _dbContext.SaveChangesAsync();

                    // Log activity
                    await _activityLogService.LogAsync("SERVICE_UPDATE", "Cập nhật dịch vụ", service.TitleVi, "Info");

                    await transaction.CommitAsync();

                    return new ServiceDto
                    {
                        Id = service.Id,
                        TitleVi = service.TitleVi,
                        TitleEn = service.TitleEn,
                        SlugVi = service.SlugVi,
                        SlugEn = service.SlugEn,
                        DescriptionVi = service.DescriptionVi,
                        DescriptionEn = service.DescriptionEn,
                        TimePosted = service.TimePosted,
                        Status = service.Status,
                        IsOutstanding = service.IsOutstanding,
                        ImageUrl = service.ImageUrl
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error updating service with id = {id}");
                    throw;
                }
                finally
                {
                    // Temp files will be cleaned by background service
                }
            }
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

        public async Task<PagingResult<ServiceDto>> FindAllForClient(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAllForClient)}: Getting published services for client");
            
            var query = _dbContext.Services
                .Where(s => !s.Deleted && s.Status == CommonStatus.ACTIVE);

            // Search by keyword if provided
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(s => s.TitleVi.Contains(input.Keyword) || 
                                       s.TitleEn.Contains(input.Keyword) ||
                                       s.DescriptionVi.Contains(input.Keyword) ||
                                       s.DescriptionEn.Contains(input.Keyword));
            }

            var totalCount = await query.CountAsync();

            var services = await query
                .OrderByDescending(s => s.IsOutstanding)
                .ThenByDescending(s => s.TimePosted)
                .Skip(input.GetSkip())
                .Take(input.PageSize == -1 ? totalCount : input.PageSize)
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    SlugVi = s.SlugVi,
                    SlugEn = s.SlugEn,
                    TitleVi = s.TitleVi,
                    TitleEn = s.TitleEn,
                    DescriptionVi = s.DescriptionVi,
                    DescriptionEn = s.DescriptionEn,
                    TimePosted = s.TimePosted,
                    Status = s.Status,
                    IsOutstanding = s.IsOutstanding,
                    ImageUrl = s.ImageUrl,
                    FeaturedImageId = s.FeaturedImageId
                })
                .ToListAsync();

            return new PagingResult<ServiceDto>
            {
                Items = services,
                TotalItems = totalCount,
                PageSize = input.PageSize == -1 ? totalCount : input.PageSize,
                Page = input.PageNumber
            };
        }

        public async Task<DetailServiceDto> FindBySlugForClient(string slug)
        {
            _logger.LogInformation($"{nameof(FindBySlugForClient)}: slug = {slug}");
            var service = await _dbContext.Services
                .Where(s => (s.SlugVi == slug || s.SlugEn == slug) && !s.Deleted && s.Status == CommonStatus.ACTIVE)
                .Select(s => new DetailServiceDto
                {
                    Id = s.Id,
                    TitleVi = s.TitleVi,
                    TitleEn = s.TitleEn,
                    SlugVi = s.SlugVi,
                    SlugEn = s.SlugEn,
                    DescriptionVi = s.DescriptionVi,
                    DescriptionEn = s.DescriptionEn,
                    ContentVi = s.ContentVi,
                    ContentEn = s.ContentEn,
                    TimePosted = s.TimePosted,
                    Status = s.Status,
                    IsOutstanding = s.IsOutstanding,
                    ImageUrl = s.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (service == null)
                throw new UserFriendlyException(ErrorCode.ServiceNotFound);

            // Load attachments
            var attachments = await _dbContext.Attachments
                .Where(a => a.ObjectType == ObjectType.Service && a.ObjectId == service.Id && !a.Deleted)
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

            service.Attachments = new AttachmentsGroupDto
            {
                Images = attachments.Where(a => a.ContentType.StartsWith("image/") && !a.IsPrimary && !a.IsContentImage).ToList(),
                Documents = attachments.Where(a => !a.ContentType.StartsWith("image/")).ToList()
            };

            return service;
        }

        public async Task<PagingResult<ServiceDto>> SearchForClient(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(SearchForClient)}: keyword = {input.Keyword}");
            
            var query = _dbContext.Services
                .Where(s => !s.Deleted && s.Status == CommonStatus.ACTIVE);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(s => s.TitleVi.Contains(input.Keyword) || 
                                       s.TitleEn.Contains(input.Keyword) ||
                                       s.DescriptionVi.Contains(input.Keyword) ||
                                       s.DescriptionEn.Contains(input.Keyword) ||
                                       s.ContentVi.Contains(input.Keyword) ||
                                       s.ContentEn.Contains(input.Keyword));
            }

            var totalCount = await query.CountAsync();

            var services = await query
                .OrderByDescending(s => s.IsOutstanding)
                .ThenByDescending(s => s.TimePosted)
                .Skip(input.GetSkip())
                .Take(input.PageSize == -1 ? totalCount : input.PageSize)
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    SlugVi = s.SlugVi,
                    SlugEn = s.SlugEn,
                    TitleVi = s.TitleVi,
                    TitleEn = s.TitleEn,
                    DescriptionVi = s.DescriptionVi,
                    DescriptionEn = s.DescriptionEn,
                    TimePosted = s.TimePosted,
                    Status = s.Status,
                    IsOutstanding = s.IsOutstanding,
                    ImageUrl = s.ImageUrl,
                    FeaturedImageId = s.FeaturedImageId
                })
                .ToListAsync();

            return new PagingResult<ServiceDto>
            {
                Items = services,
                TotalItems = totalCount,
                PageSize = input.PageSize == -1 ? totalCount : input.PageSize,
                Page = input.PageNumber
            };
        }
    }
}
