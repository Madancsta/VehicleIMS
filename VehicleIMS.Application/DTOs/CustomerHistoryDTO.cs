namespace VehicleIMS.Application.DTOs
{
    public class CustomerHistoryDTO
    {
        public int CustomerId { get; set; }

        public Guid UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public List<CustomerBookingHistoryDTO> Bookings { get; set; } = new();

        public List<CustomerSalesHistoryDTO> Sales { get; set; } = new();

        public List<CustomerReviewHistoryDTO> Reviews { get; set; } = new();

        public List<CustomerRequestHistoryDTO> Requests { get; set; } = new();
    }

    public class CustomerBookingHistoryDTO
    {
        public int BookingId { get; set; }

        public DateTime BookingDate { get; set; }

        public TimeSpan BookingTime { get; set; }

        public string BookingStatus { get; set; } = string.Empty;

        public int VehicleId { get; set; }

        public string VehicleNumber { get; set; } = string.Empty;

        public string VehicleModel { get; set; } = string.Empty;

        public string VehicleBrand { get; set; } = string.Empty;
    }

    public class CustomerSalesHistoryDTO
    {
        public int SalesId { get; set; }

        public DateTime SalesDate { get; set; }

        public decimal SalesAmount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public int BookingId { get; set; }
    }

    public class CustomerReviewHistoryDTO
    {
        public int ReviewId { get; set; }

        public int Rating { get; set; }

        public string ReviewComment { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; }

        public int SalesId { get; set; }
    }

    public class CustomerRequestHistoryDTO
    {
        public int RequestId { get; set; }

        public DateTime RequestedDate { get; set; }

        public string RequestStatus { get; set; } = string.Empty;

        public int? BookingId { get; set; }

        public int? PartId { get; set; }

        public string PartName { get; set; } = string.Empty;

        public int? RequestQuantity { get; set; }

        public string RequestDescription { get; set; } = string.Empty;
    }
}