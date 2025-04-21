using cloud_backend.Data;
using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Quotation;

namespace cloud_backend.Repositories.QuotationRequestRepo
{
    public class QuotationRequestRepository : IQuotationRequestRepository
    {
        private readonly AppDbContext _context;

        public QuotationRequestRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<IEnumerable<GetQuotationRequestsDto?>> GetQuotationRequestsDto()
        {
            return await _context.Quotation_Request
                .Include(qr => qr.Store_User)
                .Include(qr => qr.quotationRequestItems)
                .ThenInclude(qri => qri.Products)
                .Select(qr => new GetQuotationRequestsDto
                {
                    quotationRequestId = qr.quotationRequestId,
                    storeId = qr.storeId,
                    status = qr.status,
                    createdAt = qr.createdAt,
                    updatedAt = qr.updatedAt,
                    quotationRequestItems = qr.quotationRequestItems.Select(qri => new GetQuotationRequestItemsDto
                    {
                        quotationRequestItemId = qri.quotationRequestItemId,
                        quotationRequestId = qr.quotationRequestId,
                        productId = qri.productId,
                        unitPrice = qri.unitPrice,
                        quantity = qri.quantity,
                        discountPercentage = qri.discountPercentage,
                        product = new GetProductsDto
                        {
                            productId = qri.Products.productId,
                            name = qri.Products.name,
                            cost = qri.Products.cost,
                            price = qri.Products.price,
                            stockQuantity = qri.Products.stockQuantity,
                            description = qri.Products.description,
                            model = qri.Products.model,
                            category = qri.Products.category,
                            isActive = qri.Products.isActive,
                            discountPercentage = qri.Products.discountPercentage,
                            imageUrl = qri.Products.imageUrl,
                            createdAt = qri.Products.createdAt,
                            updatedAt = qri.Products.updatedAt,
                        }
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<GetPaginatedDto<GetQuotationRequestsDto>> GetQuotationRequestsDtoPaginated(QuotationRequestPaginationRequest request)
        {
            var query = _context.Quotation_Request
                .Include(qr => qr.Store_User)
                .Include(qr => qr.quotationRequestItems)
                .ThenInclude(qri => qri.Products)
                .AsQueryable();

            if (request.storeId != Guid.Empty)
                query = query.Where(c => c.storeId == request.storeId);

            if (request.status.HasValue)
                query = query.Where(c => c.status == request.status.Value);

            // Search (e.g., by name or email)
            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                string term = request.searchTerm.ToLower();
                query = query.Where(c =>
                    c.quotationRequestId.ToString().ToLower().Contains(term) ||
                    c.storeId.ToString().ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            // Apply pagination
            var data = await query
                .OrderByDescending(c => c.createdAt)
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(qr => new GetQuotationRequestsDto
                {
                    quotationRequestId = qr.quotationRequestId,
                    storeId = qr.storeId,
                    status = qr.status,
                    createdAt = qr.createdAt,
                    updatedAt = qr.updatedAt,
                    quotationRequestItems = qr.quotationRequestItems.Select(qri => new GetQuotationRequestItemsDto
                    {
                        quotationRequestItemId = qri.quotationRequestItemId,
                        quotationRequestId = qr.quotationRequestId,
                        productId = qri.productId,
                        unitPrice = qri.unitPrice,
                        quantity = qri.quantity,
                        discountPercentage = qri.discountPercentage,
                        product = new GetProductsDto
                        {
                            productId = qri.Products.productId,
                            name = qri.Products.name,
                            cost = qri.Products.cost,
                            price = qri.Products.price,
                            stockQuantity = qri.Products.stockQuantity,
                            description = qri.Products.description,
                            model = qri.Products.model,
                            category = qri.Products.category,
                            isActive = qri.Products.isActive,
                            discountPercentage = qri.Products.discountPercentage,
                            imageUrl = qri.Products.imageUrl,
                            createdAt = qri.Products.createdAt,
                            updatedAt = qri.Products.updatedAt,
                        }
                    }).ToList()
                }).ToListAsync();

            return new GetPaginatedDto<GetQuotationRequestsDto>
            {
                data = data,
                total = totalCount
            };
        }

        public async Task<GetQuotationRequestsDto?> GetQuotationRequestDtoById(Guid quotationRequestId)
        {
            return await _context.Quotation_Request.Where(qr => qr.quotationRequestId == quotationRequestId)
                .Include(qr => qr.Store_User)
                .Include(qr => qr.quotationRequestItems)
                .ThenInclude(qri => qri.Products)
                .Select(qr => new GetQuotationRequestsDto
                {
                    quotationRequestId = qr.quotationRequestId,
                    storeId = qr.storeId,
                    status = qr.status,
                    createdAt = qr.createdAt,
                    updatedAt = qr.updatedAt,
                    quotationRequestItems = qr.quotationRequestItems.Select(qri => new GetQuotationRequestItemsDto
                    {
                        quotationRequestItemId = qri.quotationRequestItemId,
                        quotationRequestId = qr.quotationRequestId,
                        productId = qri.productId,
                        unitPrice = qri.unitPrice,
                        quantity = qri.quantity,
                        discountPercentage = qri.discountPercentage,
                        product = new GetProductsDto
                        {
                            productId = qri.Products.productId,
                            name = qri.Products.name,
                            cost = qri.Products.cost,
                            price = qri.Products.price,
                            stockQuantity = qri.Products.stockQuantity,
                            description = qri.Products.description,
                            model = qri.Products.model,
                            category = qri.Products.category,
                            isActive = qri.Products.isActive,
                            discountPercentage = qri.Products.discountPercentage,
                            imageUrl = qri.Products.imageUrl,
                            createdAt = qri.Products.createdAt,
                            updatedAt = qri.Products.updatedAt,
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Quotation_Request?> GetQuotationRequestById(Guid quotationRequestId)
        {
            return await _context.Quotation_Request.Where(q => q.quotationRequestId == quotationRequestId)
                .Include(qr => qr.Store_User)
                .Include(qr => qr.quotationRequestItems)
                .ThenInclude(qri => qri.Products).FirstOrDefaultAsync();
        }

        public async Task CreateQuotationRequest(Quotation_Request quotationRequest)
        {
            _context.Quotation_Request.Add(quotationRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuotationRequest(Quotation_Request quotationRequest)
        {
            _context.Quotation_Request.Update(quotationRequest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuotationRequest(Quotation_Request quotationRequest)
        {
            _context.Quotation_Request.Remove(quotationRequest);
            await _context.SaveChangesAsync();
        }
    }
}
