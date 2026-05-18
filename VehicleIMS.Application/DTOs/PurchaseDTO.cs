namespace VehicleIMS.Application.DTOs.Purchase;

public class PurchaseResponseDto
{
    public int PurchaseId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<PurchaseItemResponseDto> Items { get; set; } = new();
}

public class PurchaseItemResponseDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal => Quantity * UnitPrice;
}

public class CreatePurchaseDto
{
    public List<PurchaseItemDto> Items { get; set; } = new();
}

public class PurchaseItemDto
{
    public int VendorId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}