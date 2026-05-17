using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Infrastructure.Repositories;

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
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerRepository.GetCustomerById(id);

        if (customer == null)
            return NotFound("Customer not found.");

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CustomerCreateDto dto)
    {
        var customer = await _customerRepository.CreateCustomer(dto);
        return Ok(customer);
    }
}