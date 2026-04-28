using System.ComponentModel.DataAnnotations;

namespace VehicleIMS.Domain.Entities
{
    public class PartCategory
    {
        [Key]
        public int PartCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
    }
}
