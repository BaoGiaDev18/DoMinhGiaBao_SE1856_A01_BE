using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// API Request Model for querying news articles with filters
    /// Used exclusively by the API layer to receive query parameters
    /// </summary>
    public class NewsArticleQueryRequest
    {
        /// <summary>
        /// Filter by status: "active" or "inactive"
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filter by creator account ID
        /// </summary>
        public short? CreatedBy { get; set; }

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
        /// Search term for searching in title, headline, or content
        /// </summary>
        [MaxLength(200, ErrorMessage = "Search term cannot exceed 200 characters")]
        public string? SearchTerm { get; set; }
    }
}
