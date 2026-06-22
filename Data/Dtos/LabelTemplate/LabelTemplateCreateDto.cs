using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Data.Dtos.LabelTemplate
{
    public class LabelTemplateCreateDto
    {
        [Required]
        [MaxLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [JsonPropertyName("labelType")]
        public string LabelType { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [JsonPropertyName("templateEngine")]
        public string TemplateEngine { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [JsonPropertyName("templateReference")]
        public string TemplateReference { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        [JsonPropertyName("versionNo")]
        public int VersionNo { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }
}
