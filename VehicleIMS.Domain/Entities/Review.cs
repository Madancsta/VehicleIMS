using System;
using System.ComponentModel.DataAnnotations;

namespace VehicleIMS.Domain.Entities
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string ReviewComment { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
        [Required]
        [ForeignKey(nameof(Sales))]
        public int SalesId { get; set; }
    }
}
