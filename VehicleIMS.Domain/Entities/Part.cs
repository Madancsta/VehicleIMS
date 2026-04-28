using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS.Domain.Entities
{
    public class Part
    {
        [Key]
        public int PartId { get; set; }

        [Required]
        [StringLength(100)]
        public string PartName { get; set; } = string.Empty;

        [Required]
        public int PartCategoryId { get; set; }

        [ForeignKey("PartCategoryId")]
        public PartCategory PartCategory { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PartPrice { get; set; }

        [Required]
        public int StockQuantity { get; set; }
    }
}