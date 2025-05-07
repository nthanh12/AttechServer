using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Service;
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
    public class ServiceService : IServiceService
    {
        private readonly ILogger<ServiceService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWysiwygFileProcessor _wysiwygFileProcessor;

        public ServiceService(ApplicationDbContext dbContext, ILogger<ServiceService> logger, IHttpContextAccessor httpContextAccessor, IWysiwygFileProcessor wysiwygFileProcessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _wysiwygFileProcessor = wysiwygFileProcessor;
        }
        public async Task<ServiceDto> Create(CreateServiceDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Content))
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

                    // Tạo slug
                    var slug = GenerateSlug(input.Name);
                    var slugExists = await _dbContext.Services.AnyAsync(p => p.Slug == slug && !p.Deleted);
                    if (slugExists)
                    {
                        slug = $"{slug}-{Guid.NewGuid().ToString("N").Substring(0, 4)}";
                    }

                    var newService = new Service
                    {
                        Slug = slug,
                        Name = input.Name,
                        Description = input.Description,
                        Content = safeContent,
                        TimePosted = input.TimePosted,
                        Status = CommonStatus.ACTIVE,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false
                    };

                    _dbContext.Services.Add(newService);
                    await _dbContext.SaveChangesAsync();

                    var (processedContent, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContent, EntityType.Service, newService.Id);
                    newService.Content = processedContent;
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ServiceDto
                    {
                        Id = newService.Id,
                        Name = newService.Name,
                        Slug = newService.Slug,
                        Description = newService.Description,
                        TimePosted = newService.TimePosted,
                        Status = newService.Status
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating Service");
                    throw;
                }
            }
        }
        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");
            var service = _dbContext.Services.FirstOrDefault(pc => pc.Id == id) ?? throw new UserFriendlyException(ErrorCode.ServiceNotFound);
            service.Deleted = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PagingResult<ServiceDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.Services.AsNoTracking()
                .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE
                    && (string.IsNullOrEmpty(input.Keyword) || p.Name.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.Id)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new ServiceDto
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status,
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
                .Where(p => !p.Deleted && p.Id == id && p.Status == CommonStatus.ACTIVE)
                .Select(p => new DetailServiceDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Description = p.Description,
                    Content = p.Content,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                })
                .FirstOrDefaultAsync();

            if (service == null)
                throw new UserFriendlyException(ErrorCode.ServiceNotFound);

            return service;
        }

        public async Task<ServiceDto> Update(UpdateServiceDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var service = await _dbContext.Services.FirstOrDefaultAsync(p => p.Id == input.Id && !p.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.ServiceNotFound);

                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Content))
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

                    // Cập nhật slug nếu tiêu đề thay đổi
                    var slug = GenerateSlug(input.Name);
                    if (slug != service.Slug)
                    {
                        var slugExists = await _dbContext.Services.AnyAsync(p => p.Slug == slug && !p.Deleted && p.Id != input.Id);
                        if (slugExists)
                        {
                            slug = $"{slug}-{DateTime.Now.Ticks}";
                        }
                        service.Slug = slug;
                    }

                    service.Name = input.Name;
                    service.Description = input.Description;
                    service.Content = safeContent;
                    service.TimePosted = input.TimePosted;
                    service.ModifiedDate = DateTime.UtcNow;
                    service.ModifiedBy = userId;

                    var (processedContent, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContent, EntityType.Service, service.Id);
                    service.Content = processedContent;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ServiceDto
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Slug = service.Slug,
                        Description = service.Description,
                        TimePosted = service.TimePosted,
                        Status = service.Status
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error updating Service");
                    throw;
                }
            }
        }

        public async Task UpdateStatusService(int id, int status)
        {
            _logger.LogInformation($"{nameof(UpdateStatusService)}: Id = {id}, status = {status}");
            var service = await _dbContext.Services.FirstOrDefaultAsync(p => p.Id == id && !p.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.ServiceNotFound);
            service.Status = status;
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
