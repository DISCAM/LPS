using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.Customer
{
    public class CustomerCreateDto
    {
        [Required]
        [MaxLength(50)]
        [JsonPropertyName("customerCode")]
        public string CustomerCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [MaxLength(30)]
        [JsonPropertyName("taxNumber")]
        public string? TaxNumber { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [MaxLength(30)]
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [MaxLength(150)]
        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [MaxLength(20)]
        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}
