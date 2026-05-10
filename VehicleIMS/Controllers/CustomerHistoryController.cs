using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff,Customer")]
    public class CustomerHistoryController : ControllerBase
    {
        private readonly ICustomerHistoryService _customerHistoryService;

        public CustomerHistoryController(ICustomerHistoryService customerHistoryService)
        {
            _customerHistoryService = customerHistoryService;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerHistory(int customerId)
        {
            var history = await _customerHistoryService.GetCustomerHistoryAsync(customerId);

            if (history == null)
            {
                return NotFound("Customer history not found.");
            }

            return Ok(history);
        }
    }
}