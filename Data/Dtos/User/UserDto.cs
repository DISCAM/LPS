using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.User
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public int UserId { get; set; }

        [JsonPropertyName("identityUserId")]
        public string? IdentityUserId { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = null!;

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("modifiedAt")]
        public DateTime? ModifiedAt { get; set; }
    }
}
