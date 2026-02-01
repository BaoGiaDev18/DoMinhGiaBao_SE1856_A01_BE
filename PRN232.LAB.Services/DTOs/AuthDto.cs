using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB.Services.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }

    public class LoginResponseDto
    {
        public short AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
        public string? AccessToken { get; set; }  // ? JWT Access Token
        public int? ExpiresIn { get; set; }        // ? Token expiration in seconds
        public string Message { get; set; } = null!;
        public bool Success { get; set; }
    }
}

