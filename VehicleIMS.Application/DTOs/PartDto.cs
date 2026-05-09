namespace VehicleIMS.Application.DTOs.Part;

public class PartResponseDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int PartCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal PartPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsLowStock => StockQuantity < 10;
}

public class CreatePartDto
{
    public string PartName { get; set; } = string.Empty;
    public int PartCategoryId { get; set; }
    public decimal PartPrice { get; set; }
 
}

public class UpdatePartDto
{
    public string PartName { get; set; } = string.Empty;
    public int PartCategoryId { get; set; }
    public decimal PartPrice { get; set; }

}