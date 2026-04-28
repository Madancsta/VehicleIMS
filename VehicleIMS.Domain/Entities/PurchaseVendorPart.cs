using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS.Domain.Entities
{
    public class PurchaseVendorPart
    {
        [Key]
        public int PurchaseVendorPartId { get; set; }

        [Required]
        public int PurchaseId { get; set; }

        [ForeignKey(nameof(PurchaseId))]
        public Purchase Purchase { get; set; }

        [Required]
        public int VendorId { get; set; }

        [ForeignKey(nameof(VendorId))]
        public Vendor Vendor { get; set; }

        [Required]
        public int PartId { get; set; }

        [ForeignKey(nameof(PartId))]
        public Part Part { get; set; }
    }
}