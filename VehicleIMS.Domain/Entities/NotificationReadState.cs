using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS.Domain.Entities;

public class NotificationReadState
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public Users User { get; set; } = null!;

    [Required]
    public string NotificationKey { get; set; } = string.Empty;

    [Required]
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;
}
