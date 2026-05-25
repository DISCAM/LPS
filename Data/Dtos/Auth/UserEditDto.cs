using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.Auth
{
    public class UserEditDto
    {
        [JsonPropertyName("id")]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = null!;

        [JsonPropertyName("roleNames")]
        public List<string> RoleNames { get; set; } = new();
    }
}
