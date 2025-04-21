using Microsoft.AspNetCore.Mvc;
using cloud_backend.Data;
using cloud_backend.Services;
using cloud_backend.Request;
using cloud_backend.Repositories.AuthRepo;
using cloud_backend.Request.Customer;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtService _jwtService;
        private readonly EmailService _emailService;

        public AuthController(IAuthRepository authRepository, JwtService jwtService, EmailService emailService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAuth([FromBody] CredentialRequest credential)
        {
            if (string.IsNullOrWhiteSpace(credential.username) || string.IsNullOrWhiteSpace(credential.password))
                return BadRequest(new { message = "Invalid credentials provided." });

            var userCredential = await _authRepository.AuthenticateAsync(credential);
            if (userCredential == null)
                return BadRequest(new { message = "Invalid username or password." });

            var user = await _authRepository.GetUserDetailsAsync(userCredential);
            if (user == null)
                return BadRequest(new { message = "User not authorized or account deactivated." });

            var token = _jwtService.GenerateToken(userCredential.credentialId, userCredential.username);
            return Ok(new { user, authToken = token });
        }
    }
}
