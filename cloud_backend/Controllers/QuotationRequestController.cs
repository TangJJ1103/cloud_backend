using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Request.Quotation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cloud_backend.Controllers
{
    [Route("quotationRequest")]
    [ApiController]
    public class QuotationRequestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuotationRequestController(AppDbContext context) {
            _context = context;
        }

        [Authorize]
        [HttpGet("findAll")]
        public async Task<ActionResult<IEnumerable<Quotation_Request>>> GetAllQuotationRequests()
        {
            var requests = await _context.Quotation_Request
                .Include(qr => qr.Store_User) // Include store details
                .Include(qr => qr.quotationRequestItems) // Include request items
                    .ThenInclude(qri => qri.Products) // Include product details
                .Select(qr => new
                {
                    quotationRequestId = qr.quotationRequestId,
                    storeId = qr.storeId,
                    status = qr.status,
                    createdAt = qr.createdAt,
                    updatedAt = qr.updatedAt,
                    store = qr.Store_User,
                    quotationRequestItems = qr.quotationRequestItems.Select(qri => new
                    {
                        quotationRequestItemId = qri.quotationRequestItemId,
                        productId = qri.productId,
                        unitPrice = qri.unitPrice,
                        quantity = qri.quantity,
                        discountPercentage = qri.discountPercentage,
                        product = qri.Products
                    }).ToList()
                })
                .ToListAsync();

            if (!requests.Any())
                return Ok(new List<object>());

            return Ok(requests);
        }

        [Authorize]
        [HttpGet("findOne/{quotationRequestId}")]
        public async Task<ActionResult> GetQuotationRequestById(Guid quotationRequestId)
        {
            var quotationRequest = await _context.Quotation_Request
                .Include(qr => qr.Store_User) // Include store details
                .Include(qr => qr.quotationRequestItems) // Include request items
                    .ThenInclude(qri => qri.Products) // Include product details
                .Where(qr => qr.quotationRequestId == quotationRequestId)
                .Select(qr => new
                {
                    quotationRequestId = qr.quotationRequestId,
                    storeId = qr.storeId,
                    status = qr.status,
                    createdAt = qr.createdAt,
                    updatedAt = qr.updatedAt,
                    store = qr.Store_User,
                    quotationRequestItems = qr.quotationRequestItems.Select(qri => new
                    {
                        quotationRequestItemId = qri.quotationRequestItemId,
                        productId = qri.productId,
                        unitPrice = qri.unitPrice,
                        quantity = qri.quantity,
                        discountPercentage = qri.discountPercentage,
                        product = qri.Products
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return Ok(quotationRequest);
        }


        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateQuotationRequest([FromBody] CreateQuotationRequest request)
        {
            if (request == null || request.quotationRequestItems == null || !request.quotationRequestItems.Any())
            {
                return BadRequest(new { message = "Invalid quotation request." });
            }

            // Validate if store exists and is active
            var store = await _context.Store_User
                .FirstOrDefaultAsync(s => s.storeId == request.storeId && s.isActive);

            if (store == null)
            {
                return BadRequest(new { message = "Invalid or inactive store." });
            }

            // Create new Quotation Request
            var quotationRequest = new Quotation_Request
            {
                quotationRequestId = Guid.NewGuid(),
                storeId = request.storeId,
                status = request.status,
                createdAt = DateTime.UtcNow,
                updatedAt = null,
                quotationRequestItems = new List<Quotation_Request_Items>()
            };

            // Validate products and add items
            foreach (var item in request.quotationRequestItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.productId == item.productId);

                if (product == null)
                {
                    return BadRequest(new { message = $"Product with ID '{item.productId}' not found." });
                }

                quotationRequest.quotationRequestItems.Add(new Quotation_Request_Items
                {
                    quotationRequestItemId = Guid.NewGuid(),
                    quotationRequestId = quotationRequest.quotationRequestId,
                    productId = item.productId,
                    unitPrice = item.unitPrice,
                    quantity = item.quantity,
                    discountPercentage = item.discountPercentage
                });
            }

            _context.Quotation_Request.Add(quotationRequest);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Quotation request created successfully."
            });
        }

        [Authorize]
        [HttpPatch("updateStatus/{quotationRequestId}")]
        public async Task<IActionResult> UpdateQuotationRequestStatus(Guid quotationRequestId, [FromBody] UpdateQuotationRequest request)
        {
            var quotationRequest = await _context.Quotation_Request
                .FirstOrDefaultAsync(qr => qr.quotationRequestId == quotationRequestId);

            if (quotationRequest == null)
            {
                return NotFound(new { message = "Quotation request not found." });
            }

            // Update status and timestamp
            quotationRequest.status = request.status;
            quotationRequest.updatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Quotation request status updated successfully.", quotationRequest });
        }


    }
}
