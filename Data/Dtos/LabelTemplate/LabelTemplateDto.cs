using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.LabelTemplate
{
    public class LabelTemplateDto
    {
        [JsonPropertyName("id")]
        public int LabelTemplateId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("labelType")]
        public string LabelType { get; set; } = null!;

        [JsonPropertyName("templateEngine")]
        public string TemplateEngine { get; set; } = null!;

        [JsonPropertyName("templateReference")]
        public string TemplateReference { get; set; } = null!;

        [JsonPropertyName("versionNo")]
        public int VersionNo { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("createdByUserId")]
        public int CreatedByUserId { get; set; }

        [JsonPropertyName("modifiedByUserId")]
        public int? ModifiedByUserId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("modifiedAt")]
        public DateTime? ModifiedAt { get; set; }
    }

}
