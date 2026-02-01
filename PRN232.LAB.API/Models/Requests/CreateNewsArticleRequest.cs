using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// API Request Model for creating a news article
    /// Used exclusively by the API layer to receive client input
    /// </summary>
    public class CreateNewsArticleRequest
    {
        [Required(ErrorMessage = "News article ID is required")]
        [MaxLength(20, ErrorMessage = "News article ID cannot exceed 20 characters")]
        public string NewsArticleId { get; set; } = string.Empty;

        [Required(ErrorMessage = "News title is required")]
        [MaxLength(400, ErrorMessage = "News title cannot exceed 400 characters")]
        public string NewsTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Headline is required")]
        [MaxLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        public string Headline { get; set; } = string.Empty;

        [MaxLength(4000, ErrorMessage = "News content cannot exceed 4000 characters")]
        public string? NewsContent { get; set; }

        [MaxLength(400, ErrorMessage = "News source cannot exceed 400 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short CategoryId { get; set; }

        public bool NewsStatus { get; set; } = true;

        [Required(ErrorMessage = "Created by account ID is required")]
        public short CreatedById { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();
    }
}
