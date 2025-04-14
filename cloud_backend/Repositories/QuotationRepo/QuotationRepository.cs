using cloud_backend.Data;
using cloud_backend.Dto;
using cloud_backend.Models;

namespace cloud_backend.Repositories.QuotationRepo
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly AppDbContext _context;

        public QuotationRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<IEnumerable<GetQuotationsDto?>> GetQuotationsDto()
        {
            return await _context.Quotations
                .Include(qr => qr.Store_User)
                .Include(qr => qr.quotationItems)
                .ThenInclude(qri => qri.Products)
                .Select(qr => new GetQuotationsDto
                {
                    quotationId = qr.quotationId,
                    storeId = qr.storeId,
                    status = qr.status,
                    orderId = qr.orderId,
                    discountPercentage = qr.discountPercentage,
                    totalAmount = qr.totalAmount,
                    totalQuantity = qr.totalQuantity,
                    createdAt = qr.createdAt,
                    updatedAt = qr.updatedAt,
                    quotationItems = qr.quotationItems.Select(qri => new GetQuotationItemsDto
                    {
                        quotationItemId = qri.quotationItemId,
                        quotationId = qr.quotationId,
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
                            createdAt = qri.Products.createdAt,
                            updatedAt = qri.Products.updatedAt,
                        }
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<GetQuotationsDto?> GetQuotationDtoById(Guid quotationId)
        {
            return await _context.Quotations.Where(q => q.quotationId == quotationId)
                .Include(qr => qr.Store_User)
                .Include(qr => qr.quotationItems)
                .ThenInclude(qri => qri.Products)
                .Select(qr => new GetQuotationsDto
                {
                    quotationId = qr.quotationId,
                    storeId = qr.storeId,
                    status = qr.status,
                    orderId = qr.orderId,
                    discountPercentage = qr.discountPercentage,
                    totalAmount = qr.totalAmount,
                    totalQuantity = qr.totalQuantity,
                    createdAt = qr.createdAt,
                    updatedAt = qr.updatedAt,
                    quotationItems = qr.quotationItems.Select(qri => new GetQuotationItemsDto
                    {
                        quotationItemId = qri.quotationItemId,
                        quotationId = qr.quotationId,
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
                            createdAt = qri.Products.createdAt,
                            updatedAt = qri.Products.updatedAt,
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Quotations?> GetQuotationById(Guid quotationId)
        {
            return await _context.Quotations.Where(q => q.quotationId == quotationId)
                .Include(qr => qr.Store_User)
                .Include(qr => qr.quotationItems)
                .ThenInclude(qri => qri.Products).FirstOrDefaultAsync();
        }

        public async Task CreateQuotation(Quotations quotation)
        {
            _context.Quotations.Add(quotation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuotation(Quotations quotation)
        {
            _context.Quotations.Update(quotation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuotation(Quotations quotation)
        {
            _context.Quotations.Remove(quotation);
            await _context.SaveChangesAsync();
        }
    }
}
