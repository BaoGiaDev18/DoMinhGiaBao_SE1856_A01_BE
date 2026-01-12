using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao_SE1856_A01_Service.DTOs
{
    public class SystemAccountDto
    {
        public short AccountId { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        [MaxLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
        public string? AccountName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
        public string? AccountEmail { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public int? AccountRole { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(70, ErrorMessage = "Password cannot exceed 70 characters")]
        public string? AccountPassword { get; set; }
    }

    public class CreateSystemAccountDto
    {
        [Required(ErrorMessage = "Account name is required")]
        [MaxLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
        public string AccountEmail { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        public int AccountRole { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(70, ErrorMessage = "Password cannot exceed 70 characters")]
        public string AccountPassword { get; set; } = null!;
    }

    public class UpdateSystemAccountDto
    {
        [Required(ErrorMessage = "Account name is required")]
        [MaxLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
        public string AccountEmail { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        public int AccountRole { get; set; }

        [MaxLength(70, ErrorMessage = "Password cannot exceed 70 characters")]
        public string? AccountPassword { get; set; }
    }
}
