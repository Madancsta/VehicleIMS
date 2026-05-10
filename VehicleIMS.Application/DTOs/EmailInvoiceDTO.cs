using System.ComponentModel.DataAnnotations;

public class EmailInvoiceDTO
{
    [Required]
    public int SalesId { get; set; }
    [EmailAddress]
    public string? RecipientEmail { get; set; }
}