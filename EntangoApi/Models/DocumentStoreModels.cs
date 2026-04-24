using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EntangoApi.Models
{
    public class DocumentStore
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [StringLength(255)]
        public string DocumentDescription { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DocumentType { get; set; }

        [StringLength(50)]
        public string? DocumentCategory { get; set; }

        [Required]
        [StringLength(10)]
        public string DocumentExtension { get; set; } = string.Empty;

        [Required]
        public long DocumentSize { get; set; }

        [Required]
        public byte[] DocumentAttachment { get; set; } = Array.Empty<byte>();
    }

    public class CreateDocumentRequest
    {
        [Required]
        [StringLength(255)]
        public string DocumentDescription { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DocumentType { get; set; }

        [StringLength(50)]
        public string? DocumentCategory { get; set; }

        [Required]
        public IFormFile Document { get; set; } = null!;
    }

    public class UpdateDocumentRequest
    {
        [Required]
        [StringLength(255)]
        public string DocumentDescription { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DocumentType { get; set; }

        [StringLength(50)]
        public string? DocumentCategory { get; set; }

        // Optional: only update the file if provided
        public IFormFile? Document { get; set; }
    }

    public class DocumentResponse
    {
        public int DocumentId { get; set; }
        public string DocumentDescription { get; set; } = string.Empty;
        public string? DocumentType { get; set; }
        public string? DocumentCategory { get; set; }
        public string DocumentExtension { get; set; } = string.Empty;
        public long DocumentSize { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
    }

    public class DocumentSearchRequest
    {
        /// <summary>
        /// Match DocumentDescription containing this text (case-insensitive).
        /// </summary>
        public string? DescriptionContains { get; set; }

        /// <summary>
        /// Match DocumentType containing this text (case-insensitive).
        /// </summary>
        public string? TypeContains { get; set; }

        /// <summary>
        /// Match DocumentCategory containing this text (case-insensitive).
        /// </summary>
        public string? CategoryContains { get; set; }
    }
}