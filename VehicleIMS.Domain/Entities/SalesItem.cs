
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS.Domain.Entities;
public class SalesItem
{
    [Key]
    public int SalesItemId { get; set; }

    [Required]
    [ForeignKey(nameof(Sales))]
    public int SalesId { get; set; }
    public Sales Sales { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Part))]
    public int PartId { get; set; }
    public Part Part { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

    ///Unit price captured at sale time (price can change later)
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal => Quantity * UnitPrice;
}
