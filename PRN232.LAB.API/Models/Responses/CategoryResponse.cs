namespace DoMinhGiaBao__SE1856_A01_BE.Models.Responses
{
    /// <summary>
    /// API Response Model for category operations
    /// Used exclusively by the API layer to return category data to clients
    /// </summary>
    public class CategoryResponse
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryDescription { get; set; }
        public short? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool IsActive { get; set; }
        public List<CategoryResponse> SubCategories { get; set; } = new List<CategoryResponse>();
    }

    /// <summary>
    /// Simplified category response for list/dropdown views
    /// </summary>
    public class CategoryListResponse
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryDescription { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
