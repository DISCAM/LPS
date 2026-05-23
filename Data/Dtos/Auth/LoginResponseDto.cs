
using System.Text.Json.Serialization;
namespace Data.Dtos.Auth
{
    public class LoginResponseDto
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = null!;
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
        [JsonPropertyName("expiration")]
        public DateTime Expiration { get; set; }
    }
}

