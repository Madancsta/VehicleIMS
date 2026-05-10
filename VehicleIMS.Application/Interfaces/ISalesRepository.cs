using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleIMS.Domain.Entities;

namespace VehicleIMS.Application.Interfaces;

public interface ISalesRepository : IRepositoryBase<Sales>
{
    Task<Sales?> GetByIdWithDetailsAsync(int salesId);

    Task<List<Sales>> GetByCustomerIdAsync(int customerId);
    Task<string> GenerateInvoiceNumberAsync();
}