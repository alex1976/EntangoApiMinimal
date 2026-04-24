using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EntangoApi.Models
{
    public class DocExtractRequest
    {
        [Required]
        public IFormFile Pdf { get; set; } = default!;

        [Required]
        public string SchemaJson { get; set; } = string.Empty;

        // Optional fields for model and generation config
        public string Model { get; set; } = "text-bison-001";
        public double Temperature { get; set; } = 0.0;
        public int MaxOutputTokens { get; set; } = 2048;
    }
}
