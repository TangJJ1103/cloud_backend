using cloud_backend.Repositories.ReceiptRepo;
using cloud_backend.Repositories.ProductRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using cloud_backend.Models;
using cloud_backend.Dto;

namespace cloud_backend.Controllers
{
    [Route("receipt")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepo;
        private readonly IProductRepository _productRepo;

        public ReceiptController(IReceiptRepository receiptRepo, IProductRepository productRepo)
        {
            _receiptRepo = receiptRepo;
            _productRepo = productRepo;
        }

        [Authorize]
        [HttpGet("getAllReceipts/{userId}")]
        public async Task<ActionResult<GetReceiptsDto?>> GetAllReceipts(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid input" });
            }

            var receipts = await _receiptRepo.GetAllReceiptsDto();
            
            return Ok(receipts);
        }
    }
}
