using cloud_backend.Data;
using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Customer;
using cloud_backend.Request.Product;

namespace cloud_backend.Repositories.ProductRepo
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetProductsDto?>> GetAllProductsDto()
        {
            return await _context.Products.Select(p => new GetProductsDto
            {
                productId = p.productId,
                name = p.name,
                cost = p.cost,
                price = p.price,
                stockQuantity = p.stockQuantity,
                description = p.description,
                model = p.model,
                category = p.category,
                isActive = p.isActive,
                discountPercentage = p.discountPercentage,
                createdAt = p.createdAt,
                updatedAt = p.updatedAt,
            }).ToListAsync();
        }

        public async Task<GetPaginatedDto<GetProductsDto>> GetAllProductsDtoPaginated(ProductPaginationRequest request)
        {
            var query = _context.Products.AsQueryable();

            if (request.isActive.HasValue)
                query = query.Where(c => c.isActive == request.isActive.Value);

            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                string term = request.searchTerm.ToLower();
                query = query.Where(c =>
                    c.productId.ToString().ToLower().Contains(term) ||
                    c.name.ToLower().Contains(term) ||
                    c.category.ToLower().Contains(term) ||
                    c.description.ToLower().Contains(term) ||
                    c.model.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(c => c.createdAt)
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(p => new GetProductsDto
                {
                    productId = p.productId,
                    name = p.name,
                    cost = p.cost,
                    price = p.price,
                    stockQuantity = p.stockQuantity,
                    description = p.description,
                    model = p.model,
                    category = p.category,
                    isActive = p.isActive,
                    discountPercentage = p.discountPercentage,
                    createdAt = p.createdAt,
                    updatedAt = p.updatedAt,
                })
                .ToListAsync();

            return new GetPaginatedDto<GetProductsDto>
            {
                data = data,
                total = totalCount
            };
        }

        public async Task<GetProductsDto?> GetProductDtoById(Guid productId)
        {
            return await _context.Products.Where(p => p.productId == productId)
                .Select(p => new GetProductsDto
                {
                    productId = p.productId,
                    name = p.name,
                    cost = p.cost,
                    price = p.price,
                    stockQuantity = p.stockQuantity,
                    description = p.description,
                    model = p.model,
                    category = p.category,
                    isActive = p.isActive,
                    discountPercentage = p.discountPercentage,
                    createdAt = p.createdAt,
                    updatedAt = p.updatedAt,
                }).FirstOrDefaultAsync();
        }

        public async Task<Products?> GetProductById(Guid productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.productId == productId);
        }

        public async Task<bool> CreateProduct(Products product)
        {
            if (_context.Products.Any(m => m.model == product.model))
                return false;

            product.productId = Guid.NewGuid();
            product.createdAt = DateTime.UtcNow.AddHours(8);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateProduct(Products product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
