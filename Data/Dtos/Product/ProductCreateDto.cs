using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.Product
{
    public class ProductCreateDto
    {
        [Required]
        [MaxLength(50)]
        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [MaxLength(14)]
        [JsonPropertyName("ean")]
        public string? Ean { get; set; }

        [MaxLength(14)]
        [JsonPropertyName("gtin")]
        public string? Gtin { get; set; }
    }
}
