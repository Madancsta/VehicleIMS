using VehicleIMS.Domain.Enums;

namespace VehicleIMS.Application.DTOs
{
    public class ServiceResponseDTO
    {
        public int ServiceId { get; set; }
        public VehicleType VehicleType { get; set; }
        public string VehicleTypeName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public float ServiceCharge { get; set; }
        public string FormattedServiceCharge { get; set; } = string.Empty;
    }
}