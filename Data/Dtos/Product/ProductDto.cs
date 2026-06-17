using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.Product
{
    public class ProductDto
    {
        [JsonPropertyName("id")]
        public int ProductId { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("ean")]
        public string? Ean { get; set; }

        [JsonPropertyName("gtin")]
        public string? Gtin { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("modifiedAt")]
        public DateTime? ModifiedAt { get; set; }
    }
}
