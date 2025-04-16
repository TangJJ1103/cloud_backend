using cloud_backend.Data;
using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Customer;
using cloud_backend.Request.Receipts;
using Microsoft.EntityFrameworkCore;

namespace cloud_backend.Repositories.ReceiptRepo
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly AppDbContext _context;

        public ReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetReceiptsDto>> GetAllReceiptsDto()
        {
            return await _context.Receipts
            .Include(r => r.Orders)
            .ThenInclude(r => r.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Select(r => new GetReceiptsDto
            {
                receiptId = r.receiptId,
                credentialId = r.credentialId,
                amount = r.amount,
                paymentMethod = r.paymentMethod,
                paymentType = r.paymentType,
                createdAt = r.createdAt,
                order = new GetOrdersDto
                {
                    orderId = r.Orders.orderId,
                    credentialId = r.Orders.credentialId,
                    quantity = r.Orders.quantity,
                    amount = r.Orders.amount,
                    discountPercentage = r.Orders.discountPercentage,
                    createdAt = r.Orders.createdAt,
                    updatedAt = r.Orders.updatedAt,
                    fulfilledAt = r.Orders.fulfilledAt,
                    status = r.Orders.status,
                    orderItems = r.Orders.OrderItems.Select(oi => new GetOrderItemsDto
                    {
                        orderItemId = oi.orderItemId,
                        orderId = oi.orderId,
                        productId = oi.productId,
                        quantity = oi.quantity,
                        unitPrice = oi.unitPrice,
                        discountPercentage = oi.discountPercentage,
                        product = new GetProductsDto
                        {
                            productId = oi.Product.productId,
                            name = oi.Product.name,
                            cost = oi.Product.cost,
                            price = oi.Product.price,
                            stockQuantity = oi.Product.stockQuantity,
                            description = oi.Product.description,
                            model = oi.Product.model,
                            category = oi.Product.category,
                            isActive = oi.Product.isActive,
                            discountPercentage = oi.Product.discountPercentage,
                            createdAt = oi.Product.createdAt,
                            updatedAt = oi.Product.updatedAt,
                        }
                    }).ToList()
                }
            }).ToListAsync();
        }

        public async Task<GetPaginatedDto<GetReceiptsDto>> GetAllReceiptsDtoPaginated(ReceiptPaginationRequest request)
        {
            var query = _context.Receipts
            .Include(r => r.Orders)
            .ThenInclude(r => r.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

            // Filtering
            if (request.paymentMethod.HasValue)
                query = query.Where(c => c.paymentMethod == request.paymentMethod.Value);

            if (request.paymentType.HasValue)
                query = query.Where(c => c.paymentType == request.paymentType.Value);

            // Search (e.g., by name or email)
            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                string term = request.searchTerm.ToLower();
                query = query.Where(c => 
                    c.receiptId.ToString().ToLower().Contains(term) || 
                    c.orderId.ToString().ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            // Apply pagination
            var data = await query
                .OrderByDescending(c => c.createdAt)
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(r => new GetReceiptsDto
                {
                    receiptId = r.receiptId,
                    credentialId = r.credentialId,
                    amount = r.amount,
                    paymentMethod = r.paymentMethod,
                    paymentType = r.paymentType,
                    createdAt = r.createdAt,
                    order = new GetOrdersDto
                    {
                        orderId = r.Orders.orderId,
                        credentialId = r.Orders.credentialId,
                        quantity = r.Orders.quantity,
                        amount = r.Orders.amount,
                        discountPercentage = r.Orders.discountPercentage,
                        createdAt = r.Orders.createdAt,
                        updatedAt = r.Orders.updatedAt,
                        fulfilledAt = r.Orders.fulfilledAt,
                        status = r.Orders.status,
                        orderItems = r.Orders.OrderItems.Select(oi => new GetOrderItemsDto
                        {
                            orderItemId = oi.orderItemId,
                            orderId = oi.orderId,
                            productId = oi.productId,
                            quantity = oi.quantity,
                            unitPrice = oi.unitPrice,
                            discountPercentage = oi.discountPercentage,
                            product = new GetProductsDto
                            {
                                productId = oi.Product.productId,
                                name = oi.Product.name,
                                cost = oi.Product.cost,
                                price = oi.Product.price,
                                stockQuantity = oi.Product.stockQuantity,
                                description = oi.Product.description,
                                model = oi.Product.model,
                                category = oi.Product.category,
                                isActive = oi.Product.isActive,
                                discountPercentage = oi.Product.discountPercentage,
                                createdAt = oi.Product.createdAt,
                                updatedAt = oi.Product.updatedAt,
                            }
                        }).ToList()
                    }
                }).ToListAsync();

            return new GetPaginatedDto<GetReceiptsDto>
            {
                data = data,
                total = totalCount
            };
        }

        public async Task<GetReceiptsDto?> GetOneReceiptDto(Guid receiptId)
        {
            return await _context.Receipts.Where(r => r.receiptId == receiptId)
            .Include(r => r.Orders)
            .ThenInclude(r => r.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Select(r => new GetReceiptsDto
            {
                receiptId = r.receiptId,
                credentialId = r.credentialId,
                amount = r.amount,
                paymentMethod = r.paymentMethod,
                paymentType = r.paymentType,
                createdAt = r.createdAt,
                order = new GetOrdersDto
                {
                    orderId = r.Orders.orderId,
                    credentialId = r.Orders.credentialId,
                    quantity = r.Orders.quantity,
                    amount = r.Orders.amount,
                    discountPercentage = r.Orders.discountPercentage,
                    createdAt = r.Orders.createdAt,
                    updatedAt = r.Orders.updatedAt,
                    fulfilledAt = r.Orders.fulfilledAt,
                    status = r.Orders.status,
                    orderItems = r.Orders.OrderItems.Select(oi => new GetOrderItemsDto
                    {
                        orderItemId = oi.orderItemId,
                        orderId = oi.orderId,
                        productId = oi.productId,
                        quantity = oi.quantity,
                        unitPrice = oi.unitPrice,
                        discountPercentage = oi.discountPercentage,
                        product = new GetProductsDto
                        {
                            productId = oi.Product.productId,
                            name = oi.Product.name,
                            cost = oi.Product.cost,
                            price = oi.Product.price,
                            stockQuantity = oi.Product.stockQuantity,
                            description = oi.Product.description,
                            model = oi.Product.model,
                            category = oi.Product.category,
                            isActive = oi.Product.isActive,
                            discountPercentage = oi.Product.discountPercentage,
                            createdAt = oi.Product.createdAt,
                            updatedAt = oi.Product.updatedAt,
                        }
                    }).ToList()
                }
            }).FirstOrDefaultAsync();
        }

        public async Task<Receipts> GetOneReceipt(Guid receiptId) =>
            await _context.Receipts.FirstOrDefaultAsync(r => r.receiptId == receiptId);

        public async Task CreateReceipt(Receipts receipt)
        {
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteReceipt(Receipts receipt)
        {
            _context?.Receipts.Remove(receipt);
            await _context.SaveChangesAsync();
        }
    }
}
