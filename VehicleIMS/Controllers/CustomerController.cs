using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Domain.Entities;
using VehicleIMS.Infrastructure.Repository;

namespace VehicleIMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerRepository _customerRepository;

    public CustomerController(CustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerRepository.GetAllCustomers();

        var result = customers.Select(FormatCustomerReport);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query is required.");
        }

        var customers = await _customerRepository.SearchCustomersAsync(query);

        var result = customers.Select(FormatCustomerReport);

        return Ok(result);
    }

    [HttpGet("reports/high-spenders")]
    public async Task<IActionResult> GetHighSpenders()
    {
        var customers = await _customerRepository.GetHighSpendersAsync();

        return Ok(customers.Select(FormatCustomerReport));
    }

    [HttpGet("reports/pending-credits")]
    public async Task<IActionResult> GetPendingCredits()
    {
        var customers = await _customerRepository.GetPendingCreditsAsync();

        return Ok(customers.Select(FormatCustomerReport));
    }

    [HttpGet("reports/regulars")]
    public async Task<IActionResult> GetRegularCustomers()
    {
        var customers = await _customerRepository.GetRegularCustomersAsync();

        return Ok(customers.Select(FormatCustomerReport));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerRepository.GetCustomerById(id);

        if (customer == null)
            return NotFound("Customer not found.");

        return Ok(FormatCustomerReport(customer));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CustomerCreateDto dto)
    {
        var customer = await _customerRepository.CreateCustomer(dto);

        return Ok(FormatCustomerReport(customer));
    }

    private object FormatCustomerReport(Customer c)
    {
        return new
        {
            c.CustomerId,
            c.FirstName,
            c.LastName,
            Email = c.User?.Email,
            PhoneNumber = c.User?.PhoneNumber,
            c.LoyaltyPoints,
            c.TotalSpent,
            c.CreditBalance,
            Vehicles = c.Vehicles.Select(v => new
            {
                v.VehicleId,
                v.VehicleNumber,
                v.Brand,
                v.Model,
                v.Color,
                v.Year
            })
        };
    }
}