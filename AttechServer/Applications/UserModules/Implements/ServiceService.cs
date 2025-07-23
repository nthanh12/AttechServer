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
                    if (string.IsNullOrWhiteSpace(input.NameVi) || string.IsNullOrWhiteSpace(input.SlugVi) || string.IsNullOrWhiteSpace(input.ContentVi))
                    {
                        throw new ArgumentException("Tên, Slug (VI) và nội dung là bắt buộc.");
                    }
                    if (string.IsNullOrWhiteSpace(input.NameEn) || string.IsNullOrWhiteSpace(input.SlugEn) || string.IsNullOrWhiteSpace(input.ContentEn))
                    {
                        throw new ArgumentException("Tên, Slug (EN) và nội dung là bắt buộc.");
                    }
                    if (input.DescriptionVi.Length > 160)
                    {
                        input.DescriptionVi = input.DescriptionVi.Substring(0, 157) + "...";
                    }
                    if (input.DescriptionEn.Length > 160)
                    {
                        input.DescriptionEn = input.DescriptionEn.Substring(0, 157) + "...";
                    }
                    if (input.TimePosted > DateTime.UtcNow)
                    {
                        throw new ArgumentException("Thời gian đăng bài không phù hợp.");
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;
                    var sanitizer = new HtmlSanitizer();
                    var safeContentVi = sanitizer.Sanitize(input.ContentVi);
                    var safeContentEn = sanitizer.Sanitize(input.ContentEn);

                    // Kiểm tra trùng slug
                    var slugViExists = await _dbContext.Services.AnyAsync(p => p.SlugVi == input.SlugVi && !p.Deleted);
                    if (slugViExists)
                    {
                        throw new ArgumentException("Slug VI đã tồn tại.");
                    }
                    var slugEnExists = await _dbContext.Services.AnyAsync(p => p.SlugEn == input.SlugEn && !p.Deleted);
                    if (slugEnExists)
                    {
                        throw new ArgumentException("Slug EN đã tồn tại.");
                    }

                    var newService = new Service
                    {
                        SlugVi = input.SlugVi,
                        SlugEn = input.SlugEn,
                        NameVi = input.NameVi,
                        NameEn = input.NameEn,
                        DescriptionVi = input.DescriptionVi,
                        DescriptionEn = input.DescriptionEn,
                        ContentVi = safeContentVi,
                        ContentEn = safeContentEn,
                        TimePosted = input.TimePosted,
                        Status = CommonStatus.ACTIVE,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false,
                        ImageUrl = input.ImageUrl ?? string.Empty
                    };

                    _dbContext.Services.Add(newService);
                    await _dbContext.SaveChangesAsync();

                    var (processedContentVi, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContentVi, EntityType.Service, newService.Id);
                    var (processedContentEn, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContentEn, EntityType.Service, newService.Id);
                    newService.ContentVi = processedContentVi;
                    newService.ContentEn = processedContentEn;
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ServiceDto
                    {
                        Id = newService.Id,
                        NameVi = newService.NameVi,
                        NameEn = newService.NameEn,
                        SlugVi = newService.SlugVi,
                        SlugEn = newService.SlugEn,
                        DescriptionVi = newService.DescriptionVi,
                        DescriptionEn = newService.DescriptionEn,
                        TimePosted = newService.TimePosted,
                        Status = newService.Status,
                        ImageUrl = newService.ImageUrl
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
                    && (string.IsNullOrEmpty(input.Keyword) || p.NameVi.Contains(input.Keyword) || p.NameEn.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.Id)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new ServiceDto
                {
                    Id = p.Id,
                    SlugVi = p.SlugVi,
                    SlugEn = p.SlugEn,
                    NameVi = p.NameVi,
                    NameEn = p.NameEn,
                    DescriptionVi = p.DescriptionVi,
                    DescriptionEn = p.DescriptionEn,
                    Status = p.Status,
                    TimePosted = p.TimePosted,
                    ImageUrl = p.ImageUrl
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
                    NameVi = p.NameVi,
                    NameEn = p.NameEn,
                    SlugVi = p.SlugVi,
                    SlugEn = p.SlugEn,
                    DescriptionVi = p.DescriptionVi,
                    DescriptionEn = p.DescriptionEn,
                    ContentVi = p.ContentVi,
                    ContentEn = p.ContentEn,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    ImageUrl = p.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (service == null)
                throw new UserFriendlyException(ErrorCode.ServiceNotFound);

            return service;
        }

        public async Task<DetailServiceDto> FindBySlug(string slug)
        {
            _logger.LogInformation($"{nameof(FindBySlug)}: slug = {slug}");
            var service = await _dbContext.Services
                .Where(p => (p.SlugVi == slug || p.SlugEn == slug) && !p.Deleted)
                .Select(p => new DetailServiceDto
                {
                    Id = p.Id,
                    NameVi = p.NameVi,
                    NameEn = p.NameEn,
                    SlugVi = p.SlugVi,
                    SlugEn = p.SlugEn,
                    DescriptionVi = p.DescriptionVi,
                    DescriptionEn = p.DescriptionEn,
                    ContentVi = p.ContentVi,
                    ContentEn = p.ContentEn,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    ImageUrl = p.ImageUrl
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
                    if (string.IsNullOrWhiteSpace(input.NameVi) || string.IsNullOrWhiteSpace(input.SlugVi) || string.IsNullOrWhiteSpace(input.ContentVi))
                    {
                        throw new ArgumentException("Tên, Slug (VI) và nội dung là bắt buộc.");
                    }
                    if (string.IsNullOrWhiteSpace(input.NameEn) || string.IsNullOrWhiteSpace(input.SlugEn) || string.IsNullOrWhiteSpace(input.ContentEn))
                    {
                        throw new ArgumentException("Tên, Slug (EN) và nội dung là bắt buộc.");
                    }
                    if (input.DescriptionVi.Length > 160)
                    {
                        input.DescriptionVi = input.DescriptionVi.Substring(0, 157) + "...";
                    }
                    if (input.DescriptionEn.Length > 160)
                    {
                        input.DescriptionEn = input.DescriptionEn.Substring(0, 157) + "...";
                    }
                    if (input.TimePosted > DateTime.UtcNow)
                    {
                        throw new ArgumentException("Thời gian đăng bài không phù hợp.");
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;
                    var sanitizer = new HtmlSanitizer();
                    var safeContentVi = sanitizer.Sanitize(input.ContentVi);
                    var safeContentEn = sanitizer.Sanitize(input.ContentEn);

                    // Kiểm tra trùng slug (trừ chính nó)
                    var slugViExists = await _dbContext.Services.AnyAsync(p => p.SlugVi == input.SlugVi && !p.Deleted && p.Id != input.Id);
                    if (slugViExists)
                    {
                        throw new ArgumentException("Slug VI đã tồn tại.");
                    }
                    var slugEnExists = await _dbContext.Services.AnyAsync(p => p.SlugEn == input.SlugEn && !p.Deleted && p.Id != input.Id);
                    if (slugEnExists)
                    {
                        throw new ArgumentException("Slug EN đã tồn tại.");
                    }

                    service.NameVi = input.NameVi;
                    service.NameEn = input.NameEn;
                    service.SlugVi = input.SlugVi;
                    service.SlugEn = input.SlugEn;
                    service.DescriptionVi = input.DescriptionVi;
                    service.DescriptionEn = input.DescriptionEn;
                    service.ContentVi = safeContentVi;
                    service.ContentEn = safeContentEn;
                    service.TimePosted = input.TimePosted;
                    service.ImageUrl = input.ImageUrl ?? string.Empty;
                    service.ModifiedDate = DateTime.UtcNow;
                    service.ModifiedBy = userId;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ServiceDto
                    {
                        Id = service.Id,
                        NameVi = service.NameVi,
                        NameEn = service.NameEn,
                        SlugVi = service.SlugVi,
                        SlugEn = service.SlugEn,
                        DescriptionVi = service.DescriptionVi,
                        DescriptionEn = service.DescriptionEn,
                        TimePosted = service.TimePosted,
                        Status = service.Status,
                        ImageUrl = service.ImageUrl
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
