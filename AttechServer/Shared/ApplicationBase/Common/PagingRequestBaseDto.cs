using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Shared.ApplicationBase.Common
{
    /// <summary>
    /// Base class for pagination request DTOs
    /// </summary>
    public class PagingRequestBaseDto
    {
        /// <summary>
        /// Default number of items per page
        /// </summary>
        public const int DEFAULT_PAGE_SIZE = 20;

        /// <summary>
        /// Minimum number of items per page
        /// </summary>
        public const int MIN_PAGE_SIZE = 1;

        /// <summary>
        /// Maximum number of items per page
        /// </summary>
        public const int MAX_PAGE_SIZE = 100;

        /// <summary>
        /// Special value to get all items
        /// </summary>
        public const int ALL_ITEMS = -1;

        /// <summary>
        /// Number of items per page. Use -1 to get all items
        /// </summary>
        [FromQuery(Name = "pageSize")]
        [Range(ALL_ITEMS, MAX_PAGE_SIZE, ErrorMessage = "Page size must be -1 (for all items) or between {1} and {2}")]
        public int PageSize { get; set; } = DEFAULT_PAGE_SIZE;

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        [FromQuery(Name = "pageNumber")]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        private string? _keyword { get; set; }

        /// <summary>
        /// Search keyword
        /// </summary>
        [FromQuery(Name = "keyword")]
        public string? Keyword
        {
            get => _keyword;
            set => _keyword = value?.Trim();
        }

        /// <summary>
        /// Calculate number of items to skip based on page size and number
        /// </summary>
        /// <returns>Number of items to skip</returns>
        public int GetSkip()
        {
            if (PageSize == ALL_ITEMS)
                return 0;

            int skip = (PageNumber - 1) * PageSize;
            return skip < 0 ? 0 : skip;
        }

        /// <summary>
        /// Sort parameters
        /// </summary>
        public List<string> Sort { get; set; } = new();
    }
}
