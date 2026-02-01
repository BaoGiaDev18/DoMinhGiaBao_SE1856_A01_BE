using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// API Request Model for creating a category
    /// Used exclusively by the API layer to receive client input
    /// </summary>
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? CategoryDescription { get; set; }

        public short? ParentCategoryId { get; set; }
    }
}
