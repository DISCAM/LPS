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
        required public int UserId { get; set; }
        
        [JsonPropertyName("login")]
        required public string Login { get; set; } = null!;

        [JsonPropertyName("fullName")]
        required public string FullName { get; set; } = null!;

        [JsonPropertyName("roleName")]
        required public string RoleName { get; set; } = null!;

        [JsonPropertyName("createdAt")]
        required public DateTime CreatedAt { get; set; }

        [JsonPropertyName("modifiedAt")]
        required public DateTime? ModifiedAt { get; set; }
    }
}
