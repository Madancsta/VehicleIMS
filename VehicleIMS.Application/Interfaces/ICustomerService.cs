using VehicleIMS.Domain.Entities;

public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomers();
    Task<Customer> GetCustomerById(int id);
}