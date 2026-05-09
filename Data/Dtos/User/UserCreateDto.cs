using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.User
{
    public class UserCreateDto
    {
       
        [JsonPropertyName("login")]
        required public string Login { get; set; } = null!;

        [JsonPropertyName("fullName")]
        required public string FullName { get; set; } = null!;

        [JsonPropertyName("roleId")]
        required public int RoleId { get; set; }

       
    }
}
