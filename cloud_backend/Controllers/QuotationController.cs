using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Repositories.ProductRepo;
using cloud_backend.Repositories.QuotationRepo;
using cloud_backend.Repositories.StoreRepo;
using cloud_backend.Request.Quotation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cloud_backend.Controllers
{
    [Route("quotation")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly IQuotationRepository _quotationRepo;
        private readonly IStoreRepository _storeRepo;
        private readonly IProductRepository _productRepo;

        public QuotationController(IQuotationRepository quotationRepository, IStoreRepository storeRepository, IProductRepository productRepository)
        {
            _quotationRepo = quotationRepository;
            _storeRepo = storeRepository;
            _productRepo = productRepository;
        }

        [Authorize]
        [HttpPost("findAll")]
        public async Task<IActionResult> GetAllQuotationsPaginated([FromBody] QuotationPaginationRequest request)
        {
            var quotations = await _quotationRepo.GetQuotationsDtoPaginated(request);

            return Ok(quotations);
        }

        [Authorize]
        [HttpGet("findOne/{quotationId}")]
        public async Task<ActionResult> GetQuotationById(Guid quotationId)
        {
            var quotation = await _quotationRepo.GetQuotationDtoById(quotationId);

            return Ok(quotation);
        }


        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateQuotation([FromBody] CreateQuotation request)
        {
            if (request == null || request.quotationItems == null || !request.quotationItems.Any())
            {
                return BadRequest(new { message = "Invalid quotation." });
            }

            // Validate if store exists and is active
            var store = await _storeRepo.IsStoreActive(request.storeId);

            if (!store)
            {
                return BadRequest(new { message = "Invalid or inactive store." });
            }

            // Calculate totalAmount and totalQuantity
            double totalAmount = 0;
            int totalQuantity = 0;

            foreach (var item in request.quotationItems)
            {
                totalQuantity += item.quantity;

                var discountedPrice = item.unitPrice * (1 - (item.discountPercentage / 100.0));
                totalAmount += discountedPrice * item.quantity;
            }

            // Create new Quotation
            var quotation = new Quotations
            {
                quotationId = Guid.NewGuid(),
                storeId = request.storeId,
                status = 1,
                totalAmount = totalAmount,
                totalQuantity = totalQuantity,
                discountPercentage = request.discountPercentage,
                createdAt = DateTime.UtcNow.AddHours(8),
                updatedAt = null,
                quotationItems = new List<Quotation_Items>()
            };

            // Validate products and add items
            foreach (var item in request.quotationItems)
            {
                var product = await _productRepo.GetProductById(item.productId);

                if (product == null)
                {
                    return BadRequest(new { message = $"Product with ID '{item.productId}' not found." });
                }

                quotation.quotationItems.Add(new Quotation_Items
                {
                    quotationItemId = Guid.NewGuid(),
                    quotationId = quotation.quotationId,
                    productId = item.productId,
                    unitPrice = item.unitPrice,
                    quantity = item.quantity,
                    discountPercentage = item.discountPercentage
                });
            }

            await _quotationRepo.CreateQuotation(quotation);

            return Ok(new
            {
                message = "Quotation created successfully."
            });
        }


        [Authorize]
        [HttpPatch("updateStatus/{quotationId}")]
        public async Task<IActionResult> UpdateQuotationStatus(Guid quotationId, [FromBody] UpdateQuotationRequest request)
        {
            var quotation = await _quotationRepo.GetQuotationById(quotationId);

            if (quotation == null)
            {
                return NotFound(new { message = "Quotation not found." });
            }

            // Update status and timestamp
            quotation.status = request.status;
            quotation.updatedAt = DateTime.UtcNow.AddHours(8);

            await _quotationRepo.UpdateQuotation(quotation);

            return Ok(new { message = "Quotation status updated successfully.", quotation });
        }
    }
}
