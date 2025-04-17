using cloud_backend.Repositories.ProductRepo;
using cloud_backend.Request.Product;
using cloud_backend.Request.S3;
using cloud_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("s3")]
    public class S3Controller : ControllerBase
    {
        private readonly S3Service _s3Service;
        private readonly IProductRepository _productRepo;

        public S3Controller(S3Service s3Service, IProductRepository productRepository)
        {
            _s3Service = s3Service;
            _productRepo = productRepository;
        }

        [HttpGet("presignedUrl")]
        public async Task<ActionResult> GetPresignedUrl([FromQuery] string extension)
        {
            if (extension == "jpg" || extension == "jpeg" || extension == "png" || extension == "svg" || extension == "gif") 
            { 
                if(extension == "jpg")
                {
                    extension = "jpeg";
                }
                var url = _s3Service.GeneratePresignedUrl(extension);
                return Ok(url);
            }
            else
            {
                return BadRequest(new { message = "Invalid extension type" });
            }
        }

        [HttpPost("uploadImage/{productId}")]
        public async Task<ActionResult> UploadImage(Guid productId, [FromBody] ProductUpdateRequest request)
        {
            if (string.IsNullOrEmpty(request.imageUrl) || productId == Guid.Empty)
            {
                return BadRequest(new { message = "Url or productId not found"});
            }
            var existingProduct = await _productRepo.GetProductById(productId);
            if (existingProduct != null)
            {
                return BadRequest(new { message = "Product not found" });
            }

            existingProduct.imageUrl = request.imageUrl.Split("?")[0];

            await _productRepo.UpdateProduct(existingProduct);
            
            return Ok();
        }
    }

}
