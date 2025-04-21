using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Request.Customer;
using cloud_backend.Request.Staff;
using cloud_backend.Request.Store;
using cloud_backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("register")]
    public class RegisterController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly HashService _hashService;

        public RegisterController(AppDbContext context, EmailService emailService, HashService hashService) {
            _context = context;
            _emailService = emailService;
            _hashService = hashService;
        }

        private string ValidateUserCredentials(string username, string email, string password, string contactNumber)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) || 
                string.IsNullOrWhiteSpace(contactNumber))
            {
                return "Email, Username, and Password are required.";
            }
            if (_context.User_Credentials.Any(u => u.email == email))
            {
                return "Email is already taken.";
            }
            if (_context.User_Credentials.Any(u => u.username == username))
            {
                return "Username is already taken.";
            }
            if (_context.User_Credentials.Any(u => u.contactNumber == contactNumber))
            {
                return "Phone Number is already registered.";
            }
            return null;
        }

        [HttpPost("customer")]
        public async Task<ActionResult> PostCustomer([FromBody] CustomerRegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "User data is required." });
            }

            var validationError = ValidateUserCredentials(request.username, request.email, request.password, request.contactNumber);
            if (validationError != null) return BadRequest(new { message = validationError });

            var credentials = new User_Credentials
            {
                credentialId = Guid.NewGuid(),
                username = request.username,
                name = request.name,
                password = _hashService.HashPassword(request.password),
                email = request.email,
                contactNumber = request.contactNumber,
                role = 5
            };

            var customerUser = new Customer_User
            {
                customerId = Guid.NewGuid(),
                credentialId = credentials.credentialId,
                address = request.address,
                createdAt = DateTime.UtcNow.AddHours(8),
                updatedAt = DateTime.UtcNow.AddHours(8),
                verificationToken = Guid.NewGuid().ToString(),
                isVerified = false,
                User_Credential = credentials
            };

            string verificationLink = $"http://localhost:5041/Email/verifyEmail?token={customerUser.verificationToken}";
            bool emailSent = await _emailService.SendVerificationEmail(credentials.email, verificationLink);

            if (!emailSent)
            {
                return BadRequest(new { message = "Failed to send verification email. Please try again." });
            }

            _context.User_Credentials.Add(credentials);
            _context.Customer_User.Add(customerUser);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("staff")]
        public async Task<ActionResult> PostStaff([FromBody] StaffRegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "User data is required." });
            }

            var validationError = ValidateUserCredentials(request.username, request.email, request.password, request.contactNumber);
            if (validationError != null) return BadRequest(new { message = validationError });

            if (request.role > 3 || request.role < 1)
            {
                return BadRequest(new { message = "User Role is invalid. Must be 'Super Admin', 'Admin' or 'Staff'." });
            }

            var credentials = new User_Credentials
            {
                credentialId = Guid.NewGuid(),
                username = request.username,
                name = request.name,
                password = _hashService.HashPassword(request.password),
                email = request.email,
                contactNumber = request.contactNumber,
                role = request.role
            };

            var staffUser = new Staff_User
            {
                staffId = Guid.NewGuid(),
                credentialId = credentials.credentialId,
                isActive = true,
                createdAt = DateTime.UtcNow.AddHours(8),
                updatedAt = DateTime.UtcNow.AddHours(8),
                User_Credential = credentials
            };

            _context.User_Credentials.Add(credentials);
            _context.Staff_User.Add(staffUser);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("store")]
        public async Task<ActionResult> PostStore([FromBody] StoreRegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "User data is required." });
            }

            var validationError = ValidateUserCredentials(request.username, request.email, request.password, request.contactNumber);
            if (validationError != null) return BadRequest(new { message = validationError });

            var credentials = new User_Credentials
            {
                credentialId = Guid.NewGuid(),
                username = request.username,
                name = request.name,
                password = _hashService.HashPassword(request.password),
                email = request.email,
                contactNumber = request.contactNumber,
                role = 4
            };

            var storeUser = new Store_User
            {
                storeId = Guid.NewGuid(),
                credentialId = credentials.credentialId,
                address = request.address,
                isActive = true,
                createdAt = DateTime.UtcNow.AddHours(8),
                updatedAt = DateTime.UtcNow.AddHours(8),
                User_Credential = credentials
            };

            _context.User_Credentials.Add(credentials);
            _context.Store_User.Add(storeUser);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
