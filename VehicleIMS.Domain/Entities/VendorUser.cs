using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS.Domain.Entities
{
    public class VendorUser
    {
        [Key]

        [Required]
        public int VendorId { get; set; }

        [ForeignKey(nameof(VendorId))]
        public Vendor Vendor { get; set; }

        [Key]
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public Users User { get; set; }
    }
}

