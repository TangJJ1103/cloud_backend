using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Repositories.StoreRepo;
using cloud_backend.Request.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("store")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreRepository _storeRepository;

        public StoreController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        [Authorize]
        [HttpGet("findAll")]
        public async Task<ActionResult<IEnumerable<StoreFindRequest>>> GetAllStores()
        {
            var stores = await _storeRepository.GetAllStores();
            return Ok(stores);
        }

        [Authorize]
        [HttpGet("findOne/{storeId}")]
        public async Task<ActionResult<StoreFindRequest>> GetStoreById(Guid storeId)
        {
            var store = await _storeRepository.GetStoreById(storeId);
            return store != null ? Ok(store) : NotFound(new { message = "Store not found" });
        }

        [Authorize]
        [HttpPost("find")]
        public async Task<IActionResult> FindStores([FromBody] StorePaginationRequest request)
        {
            if (request == null || request.offset < 1 || request.currentIndex < 1)
                return BadRequest(new { message = "Invalid pagination parameters." });

            var (stores, totalRecords) = await _storeRepository.FindStores(request);
            return Ok(new { totalRecords, stores });
        }

        [Authorize]
        [HttpPatch("update/{storeId}")]
        public async Task<ActionResult> UpdateStore(Guid storeId, [FromBody] StoreUpdateRequest request)
        {
            var result = await _storeRepository.UpdateStore(storeId, request);
            return result ? Ok(new { message = "Store updated successfully" }) : NotFound(new { message = "Store not found or duplicate data" });
        }
    }

}
