namespace VehicleIMS.Application.DTOs.Vendor;

public class VendorResponseDto
{
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string VendorEmail { get; set; } = string.Empty;
    public long VendorPhone { get; set; }
    public string VendorAddress { get; set; } = string.Empty;
}

public class CreateVendorDto
{
    public string VendorName { get; set; } = string.Empty;
    public string VendorEmail { get; set; } = string.Empty;
    public long VendorPhone { get; set; }
    public string VendorAddress { get; set; } = string.Empty;
}

public class UpdateVendorDto
{
    public string VendorName { get; set; } = string.Empty;
    public string VendorEmail { get; set; } = string.Empty;
    public long VendorPhone { get; set; }
    public string VendorAddress { get; set; } = string.Empty;
}