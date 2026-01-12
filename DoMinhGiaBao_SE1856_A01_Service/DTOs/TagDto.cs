using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao_SE1856_A01_Service.DTOs
{
    public class TagDto
    {
        public int TagId { get; set; }

        [MaxLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
        public string? TagName { get; set; }

        [MaxLength(400, ErrorMessage = "Note cannot exceed 400 characters")]
        public string? Note { get; set; }
    }

    public class CreateTagDto
    {
        [Required(ErrorMessage = "Tag name is required")]
        [MaxLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
        public string TagName { get; set; } = null!;

        [MaxLength(400, ErrorMessage = "Note cannot exceed 400 characters")]
        public string? Note { get; set; }
    }

    public class UpdateTagDto
    {
        [Required(ErrorMessage = "Tag name is required")]
        [MaxLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
        public string TagName { get; set; } = null!;

        [MaxLength(400, ErrorMessage = "Note cannot exceed 400 characters")]
        public string? Note { get; set; }
    }
}
