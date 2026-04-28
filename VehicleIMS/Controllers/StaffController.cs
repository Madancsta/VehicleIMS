using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;

namespace VehicleIMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var staff = await _staffService.GetAllStaffAsync();

            return Ok(staff);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStaffById(Guid userId)
        {
            var staff = await _staffService.GetStaffByIdAsync(userId);

            if (staff == null)
            {
                return NotFound("Staff user not found.");
            }

            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDTO createStaffDTO)
        {
            var createdStaff = await _staffService.CreateStaffAsync(createStaffDTO);

            if (createdStaff == null)
            {
                return BadRequest("Staff creation failed. Email may already exist or password is invalid.");
            }

            return CreatedAtAction(
                nameof(GetStaffById),
                new { userId = createdStaff.UserId },
                createdStaff
            );
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateStaff(Guid userId, [FromBody] UpdateStaffDTO updateStaffDTO)
        {
            var updatedStaff = await _staffService.UpdateStaffAsync(userId, updateStaffDTO);

            if (updatedStaff == null)
            {
                return NotFound("Staff user not found.");
            }

            return Ok(updatedStaff);
        }

        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeUserRoleDTO changeUserRoleDTO)
        {
            var result = await _staffService.ChangeRoleAsync(changeUserRoleDTO);

            if (!result)
            {
                return BadRequest("Role change failed.");
            }

            return Ok("Role changed successfully.");
        }

        [HttpPut("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateStaff(Guid userId)
        {
            var result = await _staffService.DeactivateStaffAsync(userId);

            if (!result)
            {
                return NotFound("Staff user not found.");
            }

            return Ok("Staff user deactivated successfully.");
        }

        [HttpPut("activate/{userId}")]
        public async Task<IActionResult> ActivateStaff(Guid userId)
        {
            var result = await _staffService.ActivateStaffAsync(userId);

            if (!result)
            {
                return NotFound("Staff user not found.");
            }

            return Ok("Staff user activated successfully.");
        }
    }
}