using DoMinhGiaBao__SE1856_A01_BE.Models.Common;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// Query parameters for Categories List API
    /// Extends base QueryParameters with category-specific filters
    /// </summary>
    public class CategoryQueryParameters : QueryParameters
    {
        /// <summary>
        /// Filter by status: "active" or "inactive"
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filter by parent category ID
        /// </summary>
        public short? ParentCategoryId { get; set; }
    }
}
