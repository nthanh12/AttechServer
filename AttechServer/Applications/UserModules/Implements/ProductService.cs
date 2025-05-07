using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Product;
using AttechServer.Applications.UserModules.Dtos.ProductCategory;
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
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWysiwygFileProcessor _wysiwygFileProcessor;

        public ProductService(ApplicationDbContext dbContext, ILogger<ProductService> logger, IHttpContextAccessor httpContextAccessor, IWysiwygFileProcessor wysiwygFileProcessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _wysiwygFileProcessor = wysiwygFileProcessor;
        }
        public async Task<ProductDto> Create(CreateProductDto input)
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

                    // Kiểm tra danh mục
                    var category = await _dbContext.ProductCategories
                        .Where(c => c.Id == input.ProductCategoryId && !c.Deleted)
                        .Select(c => new { c.Id, c.Name, c.Slug })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);
                    }

                    // Tạo slug
                    var slug = GenerateSlug(input.Name);
                    var slugExists = await _dbContext.Products.AnyAsync(p => p.Slug == slug && !p.Deleted);
                    if (slugExists)
                    {
                        slug = $"{slug}-{Guid.NewGuid().ToString("N").Substring(0, 4)}";
                    }

                    var newProduct = new Product
                    {
                        Slug = slug,
                        Name = input.Name,
                        Description = input.Description,
                        Content = safeContent,
                        TimePosted = input.TimePosted,
                        Status = CommonStatus.ACTIVE,
                        ProductCategoryId = input.ProductCategoryId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false
                    };

                    _dbContext.Products.Add(newProduct);
                    await _dbContext.SaveChangesAsync();

                    var (processedContent, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContent, EntityType.Product, newProduct.Id);
                    newProduct.Content = processedContent;
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ProductDto
                    {
                        Id = newProduct.Id,
                        Name = newProduct.Name,
                        Slug = newProduct.Slug,
                        Description = newProduct.Description,
                        TimePosted = newProduct.TimePosted,
                        Status = newProduct.Status,
                        ProductCategoryId = newProduct.ProductCategoryId,
                        ProductCategoryName = category.Name,
                        ProductCategorySlug = category.Slug
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating Product");
                    throw;
                }
            }
        }
        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");
            var product = _dbContext.Products.FirstOrDefault(pc => pc.Id == id) ?? throw new UserFriendlyException(ErrorCode.ProductNotFound);
            product.Deleted = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PagingResult<ProductDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.Products.AsNoTracking()
                .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE
                    && (string.IsNullOrEmpty(input.Keyword) || p.Name.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.Id)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status,
                    ProductCategoryId = p.ProductCategoryId
                })
                .ToListAsync();

            return new PagingResult<ProductDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<PagingResult<ProductDto>> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId)
        {
            _logger.LogInformation($"{nameof(FindAllByCategoryId)}: input = {JsonSerializer.Serialize(input)}, categoryId = {categoryId}");

            var baseQuery = _dbContext.Products.AsNoTracking()
                .Where(p => p.ProductCategoryId == categoryId && !p.Deleted && p.Status == CommonStatus.ACTIVE
                    && (string.IsNullOrEmpty(input.Keyword) || p.Name.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderByDescending(p => p.TimePosted)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Description = p.Description,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    ProductCategoryId = p.ProductCategoryId,
                    ProductCategoryName = p.ProductCategory.Name,
                    ProductCategorySlug = p.ProductCategory.Slug
                })
                .ToListAsync();

            return new PagingResult<ProductDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<DetailProductDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var product = await _dbContext.Products
                .Where(p => !p.Deleted && p.Id == id && p.Status == CommonStatus.ACTIVE)
                .Select(p => new DetailProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Description = p.Description,
                    Content = p.Content,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    ProductCategoryId = p.ProductCategoryId,
                    ProductCategoryName = p.ProductCategory.Name,
                    ProductCategorySlug = p.ProductCategory.Slug
                })
                .FirstOrDefaultAsync();

            if (product == null)
                throw new UserFriendlyException(ErrorCode.ProductNotFound);

            return product;
        }

        public async Task<ProductDto> Update(UpdateProductDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == input.Id && !p.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.ProductNotFound);

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

                    // Kiểm tra danh mục
                    var category = await _dbContext.ProductCategories
                        .Where(c => c.Id == input.ProductCategoryId && !c.Deleted)
                        .Select(c => new { c.Id, c.Name, c.Slug })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);
                    }

                    // Cập nhật slug nếu tiêu đề thay đổi
                    var slug = GenerateSlug(input.Name);
                    if (slug != product.Slug)
                    {
                        var slugExists = await _dbContext.Products.AnyAsync(p => p.Slug == slug && !p.Deleted && p.Id != input.Id);
                        if (slugExists)
                        {
                            slug = $"{slug}-{DateTime.Now.Ticks}";
                        }
                        product.Slug = slug;
                    }

                    product.Name = input.Name;
                    product.Description = input.Description;
                    product.Content = safeContent;
                    product.TimePosted = input.TimePosted;
                    product.ProductCategoryId = input.ProductCategoryId;
                    product.ModifiedDate = DateTime.UtcNow;
                    product.ModifiedBy = userId;

                    var (processedContent, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContent, EntityType.Product, product.Id);
                    product.Content = processedContent;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Slug = product.Slug,
                        Description = product.Description,
                        TimePosted = product.TimePosted,
                        Status = product.Status,
                        ProductCategoryId = product.ProductCategoryId,
                        ProductCategoryName = category.Name,
                        ProductCategorySlug = category.Slug
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error updating Product");
                    throw;
                }
            }
        }

        public async Task UpdateStatusProduct(int id, int status)
        {
            _logger.LogInformation($"{nameof(UpdateStatusProduct)}: Id = {id}, status = {status}");
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id && !p.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.ProductNotFound);
            product.Status = status;
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
