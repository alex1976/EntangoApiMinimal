using System.ComponentModel.DataAnnotations;

namespace EntangoApi.Models
{
    public class PriceListSearchRequest
    {
        /// <summary>
        /// Match PriceListAuthor containing this text (case-insensitive)
        /// </summary>
        public string? AuthorContains { get; set; }
        
        /// <summary>
        /// Match ProductDescription or ProductExtendedDescription containing this text (case-insensitive)
        /// </summary>
        public string? DescriptionContains { get; set; }

        /// <summary>
        /// Match ProductClassification equal to this value (case-insensitive)
        /// </summary>
        public string? ClassificationEquals { get; set; }

        /// <summary>
        /// Match ProductPrice exactly to this value.
        /// </summary>
        public decimal? ProductPriceEquals { get; set; }

        /// <summary>
        /// Match ProductPrice greater than this value.
        /// </summary>
        public decimal? ProductPriceGreaterThan { get; set; }

        /// <summary>
        /// Match ProductPrice less than this value.
        /// </summary>
        public decimal? ProductPriceLessThan { get; set; }

        /// <summary>
        /// Match ProductPrice between ProductPriceBetweenMin and ProductPriceBetweenMax (inclusive).
        /// </summary>
        public decimal? ProductPriceBetweenMin { get; set; }
        public decimal? ProductPriceBetweenMax { get; set; }

        /// <summary>
        /// Optional paging
        /// </summary>
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
