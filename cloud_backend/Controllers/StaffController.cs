using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Repositories.StaffRepo;
using cloud_backend.Request.Customer;
using cloud_backend.Request.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("staff")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffRepository _staffRepository;

        public StaffController(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        [Authorize]
        [HttpPost("findAll")]
        public async Task<IActionResult> GetAllStaffsPaginated([FromBody] StaffPaginationRequest request)
        {
            var staffs = await _staffRepository.GetAllStaffsPaginated(request);
            return Ok(staffs);
        }

        [Authorize]
        [HttpGet("findOne/{staffId}")]
        public async Task<ActionResult<StaffFindRequest>> GetStaffById(Guid staffId)
        {
            var staff = await _staffRepository.GetStaffById(staffId);
            return staff != null ? Ok(staff) : NotFound(new { message = "Staff not found" });
        }

        [Authorize]
        [HttpPatch("update/{staffId}")]
        public async Task<ActionResult> UpdateStaff(Guid staffId, [FromBody] StaffUpdateRequest request)
        {
            var result = await _staffRepository.UpdateStaff(staffId, request);
            return result ? Ok(new { message = "Staff updated successfully" }) : NotFound(new { message = "Staff not found or duplicate data" });
        }
    }

}
