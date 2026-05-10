using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces;

public interface IEmailService
{
    Task SendInvoiceEmailAsync(InvoiceSummaryDTO invoice, string recipientEmail);
}