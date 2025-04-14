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
        [HttpGet("findAll")]
        public async Task<ActionResult<IEnumerable<StaffFindRequest>>> GetAllStaffs()
        {
            var staffs = await _staffRepository.GetAllStaffs();
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
        [HttpPost("find")]
        public async Task<IActionResult> FindStaffs([FromBody] StaffPaginationRequest request)
        {
            if (request == null || request.offset < 1 || request.currentIndex < 1)
                return BadRequest(new { message = "Invalid pagination parameters." });

            var (staffs, totalRecords) = await _staffRepository.FindStaffs(request);
            return Ok(new { totalRecords, staffs });
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
