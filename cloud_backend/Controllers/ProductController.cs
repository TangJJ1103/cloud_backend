using cloud_backend.Models;
using cloud_backend.Request.Product;
using cloud_backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using cloud_backend.Repositories.ProductRepo;

namespace cloud_backend.Controllers
{
    [Route("product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [Authorize]
        [HttpPost("findAll")]
        public async Task<IActionResult> GetAllProductsPaginated([FromBody] ProductPaginationRequest request)
        {
            var products = await _productRepository.GetAllProductsDtoPaginated(request);
            return Ok(products);
        }

        [Authorize]
        [HttpGet("getAll")]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllProductsDto();
            return Ok(products);
        }

        [Authorize]
        [HttpGet("findOne/{productId}")]
        public async Task<ActionResult<Products>> GetProductById(Guid productId)
        {
            var product = await _productRepository.GetProductDtoById(productId);
            return product != null ? Ok(product) : NotFound(new { message = "Product not found" });
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateProduct([FromBody] Products product)
        {
            if (product == null)
                return BadRequest(new { message = "Invalid product data." });

            var result = await _productRepository.CreateProduct(product);
            return result ? Ok(new { message = "Product created successfully." }) : BadRequest(new { message = "This model already exists." });
        }

        [Authorize]
        [HttpPatch("update/{productId}")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] ProductUpdateRequest updatedProduct)
        {
            var product = await _productRepository.GetProductById(productId);

            if (product == null)
            {
                return BadRequest(new { message = $"Product {productId} was not found." });
            }
            product.name = updatedProduct.name ?? product.name;
            product.category = updatedProduct.category ?? product.category;
            product.cost = updatedProduct.cost ?? product.cost;
            product.price = updatedProduct.price ?? product.price;
            product.stockQuantity = updatedProduct.stockQuantity ?? product.stockQuantity;
            product.description = updatedProduct.description ?? product.description;
            product.model = updatedProduct.model ?? product.model;
            product.isActive = updatedProduct.isActive ?? product.isActive;
            product.discountPercentage = updatedProduct.discountPercentage ?? product.discountPercentage;

            await _productRepository.UpdateProduct(product);

            return Ok(new { message = "Product updated successfully." });
        }
    }
}
