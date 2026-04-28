using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleIMS.Domain.Entities
{
    public class RequestPart
    {
        [Required]
        public int RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public Request Request { get; set; } = null!;

        [Required]
        public int PartId { get; set; }

        [ForeignKey(nameof(PartId))]
        public Part Part { get; set; } = null!;

        [Required]
        public int RequestQuantity { get; set; }

        [Required]
        [StringLength(500)]
        public string RequestDescription { get; set; } = string.Empty;
    }
}