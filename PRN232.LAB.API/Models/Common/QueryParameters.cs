using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Common
{
    /// <summary>
    /// Base query parameters for paginated, sorted, and filtered requests
    /// All List APIs should inherit from this class
    /// </summary>
    public class QueryParameters
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        /// <summary>
        /// Page number (1-based, default: 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default: 10, max: 100)
        /// </summary>
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Property name to sort by (e.g., "categoryName", "createdDate")
        /// </summary>
        [MaxLength(50, ErrorMessage = "SortBy cannot exceed 50 characters")]
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort order: "asc" or "desc" (default: "asc")
        /// </summary>
        [RegularExpression("^(asc|desc)$", ErrorMessage = "SortOrder must be 'asc' or 'desc'")]
        public string SortOrder { get; set; } = "asc";

        /// <summary>
        /// Comma-separated list of fields to include in response
        /// Example: "categoryId,categoryName"
        /// </summary>
        [MaxLength(200, ErrorMessage = "Fields cannot exceed 200 characters")]
        public string? Fields { get; set; }

        /// <summary>
        /// Search term for full-text search
        /// </summary>
        [MaxLength(200, ErrorMessage = "Search term cannot exceed 200 characters")]
        public string? Search { get; set; }
    }
}
