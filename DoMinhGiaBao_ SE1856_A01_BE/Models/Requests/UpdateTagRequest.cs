using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// API Request Model for updating a tag
    /// Used exclusively by the API layer to receive client input
    /// </summary>
    public class UpdateTagRequest
    {
        [Required(ErrorMessage = "Tag name is required")]
        [MaxLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
        public string TagName { get; set; } = string.Empty;

        [MaxLength(400, ErrorMessage = "Note cannot exceed 400 characters")]
        public string? Note { get; set; }
    }
}
