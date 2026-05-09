using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.User
{
    public class UserEditDto
    {
        [JsonPropertyName("id")]
        required public int UserId { get; set; }

        [JsonPropertyName("login")]
        required public string Login { get; set; } = null!;

        [JsonPropertyName("fullName")]
        required public string FullName { get; set; } = null!;

        [JsonPropertyName("roleId")]
        required public int RoleId { get; set; }

        

    }
}
