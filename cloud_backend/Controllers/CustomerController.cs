using cloud_backend.Data;
using cloud_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cloud_backend.Request.Customer;
using cloud_backend.Repositories.CustomerRepo;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // GET: customer/findAll
        [Authorize]
        [HttpPost("findAll")]
        public async Task<IActionResult> GetAllCustomersPaginated([FromBody] CustomerPaginationRequest request)
        {
            var customers = await _customerRepository.GetAllCustomersPaginated(request);
            return Ok(customers);
        }

        // GET: customer/findOne/{customerId}
        [Authorize]
        [HttpGet("findOne/{customerId}")]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        {
            var customer = await _customerRepository.GetCustomerById(customerId);
            return customer != null ? Ok(customer) : NotFound(new { message = "Customer not found" });
        }

        [Authorize]
        [HttpPatch("update/{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromBody] CustomerUpdateRequest request)
        {
            var updated = await _customerRepository.UpdateCustomer(customerId, request);

            if (!updated)
                return BadRequest(new { message = "Update failed. Either customer not found or credentials already exist." });

            return Ok(new { message = "Customer updated successfully" });
        }
    }
}