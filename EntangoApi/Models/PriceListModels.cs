using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntangoApi.Models
{
    public class PriceList
    {
        [Key]
        public int PriceListId { get; set; }

        [StringLength(200)]
        public string? PriceListDescription { get; set; }

        [StringLength(100)]
        public string? PriceListAuthor { get; set; }

        [StringLength(20)]
        public string? PriceListVersion { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ProductDescription { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ProductExtendedDescription { get; set; }

        [StringLength(10)]
        public string? ProductUM { get; set; } = string.Empty;  // Unit of Measure

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ProductPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ProductNetPrice { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProductLabourPerc { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProductSafetyPerc { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProductMaterialPerc { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProductEquipmentPerc { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProductHirePerc { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProductGeneralExpensePerc { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProductBusinessProfitPerc { get; set; }

        [StringLength(100)]
        public string? ProductClassification { get; set; }
    }

    public class CreatePriceListRequest
    {
        [StringLength(200)]
        public string? PriceListDescription { get; set; }

        [StringLength(100)]
        public string? PriceListAuthor { get; set; }

        [StringLength(20)]
        public string? PriceListVersion { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ProductDescription { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ProductExtendedDescription { get; set; }


        [StringLength(10)]
        public string? ProductUM { get; set; }

        public decimal? ProductPrice { get; set; }

   
        public decimal? ProductNetPrice { get; set; }


        [Range(0, 100)]
        public decimal? ProductLabourPerc { get; set; }


        [Range(0, 100)]
        public decimal? ProductSafetyPerc { get; set; }


        [Range(0, 100)]
        public decimal? ProductMaterialPerc { get; set; }


        [Range(0, 100)]
        public decimal? ProductEquipmentPerc { get; set; }


        [Range(0, 100)]
        public decimal? ProductHirePerc { get; set; }


        [Range(0, 100)]
        public decimal? ProductGeneralExpensePerc { get; set; }


        [Range(0, 100)]
        public decimal? ProductBusinessProfitPerc { get; set; }

        [StringLength(100)]
        public string? ProductClassification { get; set; }  
    }

    public class UpdatePriceListRequest : CreatePriceListRequest
    {
        [Required]
        public int PriceListId { get; set; }
    }
}