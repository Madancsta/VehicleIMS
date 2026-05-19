using VehicleIMS.Application.DTOs;

namespace VehicleIMS.Application.Interfaces;

public interface ISalesService
{
    Task<SalesResponseDTO> CreateSaleAsync(CreateSalesDTO dto);
    Task<SalesResponseDTO?> GetSaleByIdAsync(int salesId);
    Task<InvoiceSummaryDTO?> GetInvoiceSummaryAsync(int salesId);
    Task<List<SalesResponseDTO>> GetSalesByCustomerAsync(int customerId);
    Task<List<SalesResponseDTO>> GetAllSalesAsync();
    Task<SalesResponseDTO> UpdateSalePaymentStatusAsync(int salesId, UpdateSalesStatusDTO dto);
}
