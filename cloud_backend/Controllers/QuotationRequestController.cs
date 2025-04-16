using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Repositories.QuotationRequestRepo;
using cloud_backend.Repositories.StoreRepo;
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
        private readonly IQuotationRequestRepository _quotationRequestRepo;
        private readonly IStoreRepository _storeRepo;

        public QuotationRequestController(IQuotationRequestRepository quotationRequestRepository, IStoreRepository storeRepository) {
            _quotationRequestRepo = quotationRequestRepository;
            _storeRepo = storeRepository;
        }

        [Authorize]
        [HttpPost("findAll")]
        public async Task<IActionResult> GetAllQuotationRequestsPaginated(QuotationRequestPaginationRequest request)
        {
            var requests = await _quotationRequestRepo.GetQuotationRequestsDtoPaginated(request);

            return Ok(requests);
        }

        [Authorize]
        [HttpGet("findOne/{quotationRequestId}")]
        public async Task<ActionResult> GetQuotationRequestById(Guid quotationRequestId)
        {
            var quotationRequest = await _quotationRequestRepo.GetQuotationRequestById(quotationRequestId);

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
            var store = await _storeRepo.IsStoreActive(request.storeId);

            if (!store)
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

            await _quotationRequestRepo.CreateQuotationRequest(quotationRequest);

            return Ok(new
            {
                message = "Quotation request created successfully."
            });
        }

        [Authorize]
        [HttpPatch("updateStatus/{quotationRequestId}")]
        public async Task<IActionResult> UpdateQuotationRequestStatus(Guid quotationRequestId, [FromBody] UpdateQuotationRequest request)
        {
            var quotationRequest = await _quotationRequestRepo.GetQuotationRequestById(quotationRequestId);

            if (quotationRequest == null)
            {
                return NotFound(new { message = "Quotation request not found." });
            }

            // Update status and timestamp
            quotationRequest.status = request.status;
            quotationRequest.updatedAt = DateTime.UtcNow;

            await _quotationRequestRepo.UpdateQuotationRequest(quotationRequest);

            return Ok(new { message = "Quotation request status updated successfully.", quotationRequest });
        }


    }
}
