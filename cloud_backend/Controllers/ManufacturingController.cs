using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Repositories.ManufactureRepo;
using cloud_backend.Repositories.ProductRepo;
using cloud_backend.Request.Manufacturing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cloud_backend.Controllers
{
    [Route("manufacture")]
    [ApiController]
    public class ManufacturingController : ControllerBase
    {
        private readonly IManufacturingRepository _manufactureRepo;
        private readonly IProductRepository _productRepo;

        public ManufacturingController(IManufacturingRepository manufacturingRepository, IProductRepository productRepository)
        {
            _manufactureRepo = manufacturingRepository;
            _productRepo = productRepository;
        }

        [Authorize]
        [HttpPost("findAll")]
        public async Task<IActionResult> GetAllManufacturingRequestsPaginated([FromBody] ManufacturingPaginationRequest request)
        {
            var requests = await _manufactureRepo.GetManufacturingRequestsPaginated(request);
            return Ok(requests);
        }

        [Authorize]
        [HttpGet("findOne/{requestId}")]
        public async Task<ActionResult<Manufacturing_Request>> GetOneManufacturingRequests(Guid requestId)
        {
            var request = await _manufactureRepo.GetManufacturingRequest(requestId);

            if (request == null)
            {
                return Ok(request);
            }

            var result = new
            {
                requestId = request.requestId,
                quantity = request.quantity,
                cost = request.cost,
                status = request.status,
                createdAt = request.createdAt,
                updatedAt = request.updatedAt,
                product = request.Products,
            };
            return Ok(result);
        }

        //manufacture/create
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateManufacturingRequest([FromBody] CreateManufacturingRequest request)
        {
            if (request == null || request.quantity <= 0)
            {
                return BadRequest(new { message = "Invalid manufacturing request." });
            }

            // Retrieve product details
            var product = await _productRepo.GetProductById(request.productId);

            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            // Use product cost in the manufacturing request
            double manufacturingCost = product.cost;

            // Create new manufacturing request
            var manufacturingRequest = new Manufacturing_Request
            {
                requestId = Guid.NewGuid(),
                productId = request.productId,
                quantity = request.quantity,
                cost = manufacturingCost, 
                createdAt = DateTime.UtcNow.AddHours(8),
                updatedAt = DateTime.UtcNow.AddHours(8),
                status = 1
            };

            await _manufactureRepo.CreateManufacturingRequest(manufacturingRequest);

            return Ok(new { message = "Manufacturing request created successfully.", requestId = manufacturingRequest.requestId });
        }

        [Authorize]
        [HttpPatch("updateStatus/{requestId}")]
        public async Task<IActionResult> UpdateManufacturingStatus(Guid requestId, [FromBody] UpdateManufacturingRequest newStatus)
        {
            // Find the manufacturing request
            var request = await _manufactureRepo.GetManufacturingRequest(requestId);

            if (request == null)
            {
                return NotFound(new { message = "Manufacturing request not found." });
            }
                
            // Update the status
            request.status = newStatus.status;
            request.updatedAt = DateTime.UtcNow.AddHours(8);

            // If status is 3 (Completed), update product stock
            if (newStatus.status == 3 && request.Products != null)
            {
                request.Products.stockQuantity += request.quantity;
                _productRepo.UpdateProduct(request.Products);
            }

            // Save changes
            await _manufactureRepo.UpdateManufacturingRequest(request);

            return Ok(new { message = "Manufacturing request updated successfully."});
        }
    }
}
