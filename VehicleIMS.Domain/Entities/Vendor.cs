using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS.Domain.Entities
{
    public class Vendor
    {
        [Key]
        public int VendorId { get; set; }

        [Required]
        [StringLength(100)]
        public string VendorName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VendorEmail { get; set; } = string.Empty;

        [Required]
        public long VendorPhone { get; set; }

        [Required]
        public string VendorAddress { get; set; } = string.Empty;
    }
}
