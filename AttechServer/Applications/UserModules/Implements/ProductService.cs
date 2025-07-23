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

                    // Kiểm tra danh mục
                    var category = await _dbContext.ProductCategories
                        .Where(c => c.Id == input.ProductCategoryId && !c.Deleted)
                        .Select(c => new { c.Id, c.NameVi, c.SlugVi })
                        .FirstOrDefaultAsync();
                    if (category == null)
                    {
                        throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);
                    }

                    // Kiểm tra trùng slug
                    var slugViExists = await _dbContext.Products.AnyAsync(p => p.SlugVi == input.SlugVi && !p.Deleted);
                    if (slugViExists)
                    {
                        throw new ArgumentException("Slug VI đã tồn tại.");
                    }
                    var slugEnExists = await _dbContext.Products.AnyAsync(p => p.SlugEn == input.SlugEn && !p.Deleted);
                    if (slugEnExists)
                    {
                        throw new ArgumentException("Slug EN đã tồn tại.");
                    }

                    var newProduct = new Product
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
                        ProductCategoryId = input.ProductCategoryId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false,
                        ImageUrl = input.ImageUrl ?? string.Empty
                    };

                    _dbContext.Products.Add(newProduct);
                    await _dbContext.SaveChangesAsync();

                    var (processedContentVi, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContentVi, EntityType.Product, newProduct.Id);
                    var (processedContentEn, _) = await _wysiwygFileProcessor.ProcessContentAsync(safeContentEn, EntityType.Product, newProduct.Id);
                    newProduct.ContentVi = processedContentVi;
                    newProduct.ContentEn = processedContentEn;
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ProductDto
                    {
                        Id = newProduct.Id,
                        NameVi = newProduct.NameVi,
                        NameEn = newProduct.NameEn,
                        SlugVi = newProduct.SlugVi,
                        SlugEn = newProduct.SlugEn,
                        DescriptionVi = newProduct.DescriptionVi,
                        DescriptionEn = newProduct.DescriptionEn,
                        TimePosted = newProduct.TimePosted,
                        Status = newProduct.Status,
                        ProductCategoryId = newProduct.ProductCategoryId,
                        ProductCategoryName = category.NameVi,
                        ProductCategorySlug = category.SlugVi,
                        ImageUrl = newProduct.ImageUrl
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
                    && (string.IsNullOrEmpty(input.Keyword) || p.NameVi.Contains(input.Keyword) || p.NameEn.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.Id)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    SlugVi = p.SlugVi,
                    SlugEn = p.SlugEn,
                    NameVi = p.NameVi,
                    NameEn = p.NameEn,
                    DescriptionVi = p.DescriptionVi,
                    DescriptionEn = p.DescriptionEn,
                    Status = p.Status,
                    ProductCategoryId = p.ProductCategoryId,
                    ProductCategoryName = p.ProductCategory.NameVi,
                    ProductCategorySlug = p.ProductCategory.SlugVi,
                    ImageUrl = p.ImageUrl
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
                    && (string.IsNullOrEmpty(input.Keyword) || p.NameVi.Contains(input.Keyword) || p.NameEn.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderByDescending(p => p.TimePosted)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    NameVi = p.NameVi,
                    NameEn = p.NameEn,
                    SlugVi = p.SlugVi,
                    SlugEn = p.SlugEn,
                    DescriptionVi = p.DescriptionVi,
                    DescriptionEn = p.DescriptionEn,
                    TimePosted = p.TimePosted,
                    Status = p.Status,
                    ProductCategoryId = p.ProductCategoryId,
                    ProductCategoryName = p.ProductCategory.NameVi,
                    ProductCategorySlug = p.ProductCategory.SlugVi,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return new PagingResult<ProductDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo SlugVi danh mục, bao gồm cả category chính và sub-categories
        /// </summary>
        public async Task<PagingResult<ProductDto>> FindAllByCategorySlug(
            PagingRequestBaseDto input,
            string slugVi)
        {
            var category = await _dbContext.ProductCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.SlugVi == slugVi && !c.Deleted);
            if (category == null)
                throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);
            return await FindAllByCategoryId(input, category.Id);
        }
        public async Task<DetailProductDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var product = await _dbContext.Products
                .Where(p => p.Id == id && !p.Deleted)
                .Select(p => new DetailProductDto
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
                    ProductCategoryId = p.ProductCategoryId,
                    ProductCategoryName = p.ProductCategory.NameVi,
                    ProductCategorySlug = p.ProductCategory.SlugVi,
                    ImageUrl = p.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (product == null)
                throw new UserFriendlyException(ErrorCode.ProductNotFound);

            return product;
        }

        public async Task<DetailProductDto> FindBySlug(string slug)
        {
            _logger.LogInformation($"{nameof(FindBySlug)}: slug = {slug}");
            var product = await _dbContext.Products
                .Where(p => (p.SlugVi == slug || p.SlugEn == slug) && !p.Deleted)
                .Select(p => new DetailProductDto
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
                    ProductCategoryId = p.ProductCategoryId,
                    ProductCategoryName = p.ProductCategory.NameVi,
                    ProductCategorySlug = p.ProductCategory.SlugVi,
                    ImageUrl = p.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (product == null)
                throw new UserFriendlyException(ErrorCode.ProductNotFound);

            return product;
        }

        public async Task<ProductDto> Update(UpdateProductDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == input.Id && !p.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.ProductNotFound);
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

                    // Kiểm tra trùng slug (trừ chính nó)
                    var slugViExists = await _dbContext.Products.AnyAsync(p => p.SlugVi == input.SlugVi && !p.Deleted && p.Id != input.Id);
                    if (slugViExists)
                    {
                        throw new ArgumentException("Slug VI đã tồn tại.");
                    }
                    var slugEnExists = await _dbContext.Products.AnyAsync(p => p.SlugEn == input.SlugEn && !p.Deleted && p.Id != input.Id);
                    if (slugEnExists)
                    {
                        throw new ArgumentException("Slug EN đã tồn tại.");
                    }

                    product.NameVi = input.NameVi;
                    product.NameEn = input.NameEn;
                    product.SlugVi = input.SlugVi;
                    product.SlugEn = input.SlugEn;
                    product.DescriptionVi = input.DescriptionVi;
                    product.DescriptionEn = input.DescriptionEn;
                    product.ContentVi = safeContentVi;
                    product.ContentEn = safeContentEn;
                    product.TimePosted = input.TimePosted;
                    product.ProductCategoryId = input.ProductCategoryId;
                    product.ImageUrl = input.ImageUrl ?? string.Empty;
                    product.ModifiedDate = DateTime.UtcNow;
                    product.ModifiedBy = userId;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ProductDto
                    {
                        Id = product.Id,
                        NameVi = product.NameVi,
                        NameEn = product.NameEn,
                        SlugVi = product.SlugVi,
                        SlugEn = product.SlugEn,
                        DescriptionVi = product.DescriptionVi,
                        DescriptionEn = product.DescriptionEn,
                        TimePosted = product.TimePosted,
                        Status = product.Status,
                        ProductCategoryId = product.ProductCategoryId,
                        ProductCategoryName = product.ProductCategory.NameVi,
                        ProductCategorySlug = product.ProductCategory.SlugVi,
                        ImageUrl = product.ImageUrl
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
    }
}
