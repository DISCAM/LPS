using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Dtos.Auth
{
    public class RegisterUserDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = null!;

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
