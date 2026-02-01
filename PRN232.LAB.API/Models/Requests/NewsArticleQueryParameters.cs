using DoMinhGiaBao__SE1856_A01_BE.Models.Common;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// Query parameters for NewsArticles List API
    /// Extends base QueryParameters with news article-specific filters
    /// </summary>
    public class NewsArticleQueryParameters : QueryParameters
    {
        /// <summary>
        /// Filter by status: "active" or "inactive"
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filter by creator account ID
        /// </summary>
        public short? CreatedById { get; set; }

        /// <summary>
        /// Filter by category ID
        /// </summary>
        public short? CategoryId { get; set; }

        /// <summary>
        /// Start date for date range filtering
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date for date range filtering
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Filter by tag ID
        /// </summary>
        public int? TagId { get; set; }
    }
}
