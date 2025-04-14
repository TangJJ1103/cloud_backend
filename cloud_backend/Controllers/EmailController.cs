using Microsoft.AspNetCore.Mvc;
using cloud_backend.Data;
using cloud_backend.Services;
using cloud_backend.Repositories.CustomerRepo;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("email")]
    public class EmailController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public EmailController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // ✅ GET: email/verifyEmail
        [HttpGet("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var user = await _customerRepository.VerifyEmail(token);

            if (user == null)
                return BadRequest(new { message = "Invalid verification token." });

            return Ok(new { message = "Email verified successfully! You can now log in." });
        }
    }

}
