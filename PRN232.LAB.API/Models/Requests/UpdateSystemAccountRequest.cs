using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// API Request Model for updating a system account
    /// Used exclusively by the API layer to receive client input
    /// </summary>
    public class UpdateSystemAccountRequest
    {
        [Required(ErrorMessage = "Account name is required")]
        [MaxLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
        public string AccountEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [Range(0, 2, ErrorMessage = "Role must be 0 (Admin), 1 (Staff), or 2 (Lecturer)")]
        public int AccountRole { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(70, ErrorMessage = "Password cannot exceed 70 characters")]
        public string? AccountPassword { get; set; }
    }
}
