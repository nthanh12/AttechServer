using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Shared.WebAPIBase
{
    /// <summary>
    /// Base controller for CRUD operations with standardized patterns
    /// </summary>
    /// <typeparam name="TService">Service interface type</typeparam>
    /// <typeparam name="TDto">List DTO type</typeparam>
    /// <typeparam name="TDetailDto">Detail DTO type</typeparam>
    /// <typeparam name="TCreateDto">Create DTO type</typeparam>
    /// <typeparam name="TUpdateDto">Update DTO type</typeparam>
    [ApiController]
    [Authorize]
    public abstract class BaseCrudController<TService, TDto, TDetailDto, TCreateDto, TUpdateDto> : ApiControllerBase
        where TService : class
        where TDto : class
        where TDetailDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly TService _service;

        protected BaseCrudController(TService service, ILogger logger) : base(logger)
        {
            _service = service;
        }

        /// <summary>
        /// Get paginated list of items
        /// </summary>
        [HttpGet("find-all")]
        public virtual async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await GetFindAllAsync(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        /// <summary>
        /// Get item by ID
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        public virtual async Task<ApiResponse> FindById(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await GetFindByIdAsync(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        /// <summary>
        /// Get item by slug (if supported)
        /// </summary>
        [HttpGet("detail/{slug}")]
        public virtual async Task<ApiResponse> FindBySlug(string slug)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await GetFindBySlugAsync(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        /// <summary>
        /// Create new item
        /// </summary>
        [HttpPost("create")]
        public virtual async Task<ApiResponse> Create([FromBody] TCreateDto input)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await GetCreateAsync(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        /// <summary>
        /// Update existing item
        /// </summary>
        [HttpPut("update")]
        public virtual async Task<ApiResponse> Update([FromBody] TUpdateDto input)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await GetUpdateAsync(input);
                return result != null 
                    ? new ApiResponse(ApiStatusCode.Success, result, 200, "Ok")
                    : new ApiResponse(ApiStatusCode.Success, null, 200, "Ok");
            });
        }

        /// <summary>
        /// Delete item by ID
        /// </summary>
        [HttpDelete("delete/{id}")]
        public virtual async Task<ApiResponse> Delete(int id)
        {
            return await ExecuteAsync(async () =>
            {
                await GetDeleteAsync(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Ok");
            });
        }

        /// <summary>
        /// Update item status
        /// </summary>
        [HttpPut("update-status")]
        public virtual async Task<ApiResponse> UpdateStatus([FromBody] AttechServer.Applications.UserModules.Dtos.UpdateStatusDto input)
        {
            return await ExecuteVoidAsync(async () => await GetUpdateStatusAsync(input));
        }

        #region Protected Virtual Methods - Override in derived classes

        /// <summary>
        /// Override this method to implement FindAll logic
        /// </summary>
        protected abstract Task<object> GetFindAllAsync(PagingRequestBaseDto input);

        /// <summary>
        /// Override this method to implement FindById logic
        /// </summary>
        protected abstract Task<TDetailDto> GetFindByIdAsync(int id);

        /// <summary>
        /// Override this method to implement FindBySlug logic (optional)
        /// </summary>
        protected virtual async Task<TDetailDto> GetFindBySlugAsync(string slug)
        {
            throw new NotImplementedException("FindBySlug is not implemented for this controller");
        }

        /// <summary>
        /// Override this method to implement Create logic
        /// </summary>
        protected abstract Task<object> GetCreateAsync(TCreateDto input);

        /// <summary>
        /// Override this method to implement Update logic
        /// </summary>
        protected abstract Task<object?> GetUpdateAsync(TUpdateDto input);

        /// <summary>
        /// Override this method to implement Delete logic
        /// </summary>
        protected abstract Task GetDeleteAsync(int id);

        /// <summary>
        /// Override this method to implement UpdateStatus logic
        /// </summary>
        protected virtual async Task GetUpdateStatusAsync(AttechServer.Applications.UserModules.Dtos.UpdateStatusDto input)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Execute operation with standardized error handling
        /// </summary>
        protected async Task<ApiResponse> ExecuteAsync(Func<Task<ApiResponse>> operation)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Execute operation with standardized error handling and return data
        /// </summary>
        protected async Task<ApiResponse> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            try
            {
                var result = await operation();
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Execute void operation with standardized error handling
        /// </summary>
        protected async Task<ApiResponse> ExecuteVoidAsync(Func<Task> operation)
        {
            try
            {
                await operation();
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// Base controller for content with categories (News, Notifications)
    /// </summary>
    public abstract class BaseContentController<TService, TDto, TDetailDto, TCreateDto, TUpdateDto> 
        : BaseCrudController<TService, TDto, TDetailDto, TCreateDto, TUpdateDto>
        where TService : class
        where TDto : class
        where TDetailDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected BaseContentController(TService service, ILogger logger) : base(service, logger)
        {
        }

        /// <summary>
        /// Get items by category slug
        /// </summary>
        [HttpGet("category/{slug}")]
        [AllowAnonymous]
        public virtual async Task<ApiResponse> FindAllByCategorySlug([FromQuery] PagingRequestBaseDto input, string slug)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await GetFindAllByCategorySlugAsync(input, slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        /// <summary>
        /// Override this method to implement FindAllByCategorySlug logic
        /// </summary>
        protected abstract Task<object> GetFindAllByCategorySlugAsync(PagingRequestBaseDto input, string slug);
    }
}