using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.User
{
    public class UserCreateDto
    {
        [Required]
        [MaxLength(100)]
        [JsonPropertyName("login")]
        required public string Login { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [JsonPropertyName("fullName")]
        required public string FullName { get; set; } = null!;

        [Required]
        [JsonPropertyName("roleId")]
        required public int RoleId { get; set; }

       
    }
}
