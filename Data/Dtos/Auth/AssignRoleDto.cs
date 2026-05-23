
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
namespace Data.Dtos.Auth
{
    public class AssignRoleDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
        [Required]
        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;
    }
}


