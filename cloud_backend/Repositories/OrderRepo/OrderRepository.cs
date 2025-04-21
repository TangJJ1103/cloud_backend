using cloud_backend.Data;
using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Repositories.ReceiptRepo;
using cloud_backend.Request.Customer;
using cloud_backend.Request.Order;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;

namespace cloud_backend.Repositories.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IReceiptRepository _receiptRepository;

        public OrderRepository(AppDbContext context, IReceiptRepository receiptRepository)
        {
            _context = context;
            _receiptRepository = receiptRepository;
        }

        public async Task<IEnumerable<GetOrdersDto?>> GetAllOrdersDto()
        {
            return await _context.Orders
                .Include(o => o.Receipts)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Select(o => new GetOrdersDto
                {
                    orderId = o.orderId,
                    credentialId = o.credentialId,
                    quantity = o.quantity,
                    amount = o.amount,
                    discountPercentage = o.discountPercentage,
                    createdAt = o.createdAt,
                    updatedAt = o.updatedAt,
                    fulfilledAt = o.fulfilledAt,
                    status = o.status,
                    receipt = new GetReceiptOnlyDto
                    {
                        receiptId = o.Receipts.receiptId,
                        credentialId = o.Receipts.credentialId,
                        amount = o.Receipts.amount,
                        paymentMethod = o.Receipts.paymentMethod,
                        paymentType = o.Receipts.paymentType,
                        createdAt = o.Receipts.createdAt
                    },
                    orderItems = o.OrderItems.Select(oi => new GetOrderItemsDto
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
                            soldQuantity = oi.Product.soldQuantity,
                            imageUrl = oi.Product.imageUrl,
                            createdAt = oi.Product.createdAt,
                            updatedAt = oi.Product.updatedAt,
                        }
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<GetPaginatedDto<GetOrdersDto>> GetAllOrdersDtoPaginated(OrderPaginationRequest request)
        {
            var query = _context.Orders
            .Include(o => o.Receipts)
            .Include(c => c.OrderItems)
            .AsQueryable();

            if (request.credentialId != Guid.Empty)
                query = query.Where(c => c.credentialId == request.credentialId);

            if (request.status.HasValue)
                query = query.Where(c => c.status == request.status);

            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                string term = request.searchTerm.ToLower();
                query = query.Where(c => c.orderId.ToString().ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            // Apply pagination
            var data = await query
                .OrderByDescending(c => c.createdAt)
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(o => new GetOrdersDto
                {
                    orderId = o.orderId,
                    credentialId = o.credentialId,
                    quantity = o.quantity,
                    amount = o.amount,
                    discountPercentage = o.discountPercentage,
                    createdAt = o.createdAt,
                    updatedAt = o.updatedAt,
                    fulfilledAt = o.fulfilledAt,
                    status = o.status,
                    receipt = new GetReceiptOnlyDto
                    {
                        receiptId = o.Receipts.receiptId,
                        credentialId = o.Receipts.credentialId,
                        amount = o.Receipts.amount,
                        paymentMethod = o.Receipts.paymentMethod,
                        paymentType = o.Receipts.paymentType,
                        createdAt = o.Receipts.createdAt
                    },
                    orderItems = o.OrderItems.Select(oi => new GetOrderItemsDto
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
                            soldQuantity = oi.Product.soldQuantity,
                            imageUrl = oi.Product.imageUrl,
                            createdAt = oi.Product.createdAt,
                            updatedAt = oi.Product.updatedAt,
                        }
                    }).ToList()
                })
                .ToListAsync();

            return new GetPaginatedDto<GetOrdersDto>
            {
                data = data,
                total = totalCount
            };
        }

        public async Task<IEnumerable<GetOrdersDto?>> GetUserOrdersDto(Guid credentialId)
        {
            return await _context.Orders
                .Where(o => o.credentialId == credentialId)
                .Include(o => o.Receipts)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Select(o => new GetOrdersDto
                {
                    orderId = o.orderId,
                    credentialId = o.credentialId,
                    quantity = o.quantity,
                    amount = o.amount,
                    discountPercentage = o.discountPercentage,
                    createdAt = o.createdAt,
                    updatedAt = o.updatedAt,
                    fulfilledAt = o.fulfilledAt,
                    status = o.status,
                    receipt = new GetReceiptOnlyDto
                    {
                        receiptId = o.Receipts.receiptId,
                        credentialId = o.Receipts.credentialId,
                        amount = o.Receipts.amount,
                        paymentMethod = o.Receipts.paymentMethod,
                        paymentType = o.Receipts.paymentType,
                        createdAt = o.Receipts.createdAt
                    },
                    orderItems = o.OrderItems.Select(oi => new GetOrderItemsDto
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
                            soldQuantity = oi.Product.soldQuantity,
                            imageUrl = oi.Product.imageUrl,
                            createdAt = oi.Product.createdAt,
                            updatedAt = oi.Product.updatedAt,
                        }
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<GetOrdersDto?> GetOrderDtoById(Guid orderId)
        {
            return await _context.Orders
                .Where(o => o.orderId == orderId)
                .Include(o => o.Receipts)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Select(o => new GetOrdersDto
                {
                    orderId = o.orderId,
                    credentialId = o.credentialId,
                    quantity = o.quantity,
                    amount = o.amount,
                    discountPercentage = o.discountPercentage,
                    createdAt = o.createdAt,
                    updatedAt = o.updatedAt,
                    fulfilledAt = o.fulfilledAt,
                    status = o.status,
                    receipt = new GetReceiptOnlyDto
                    {
                        receiptId = o.Receipts.receiptId,
                        credentialId = o.Receipts.credentialId,
                        amount = o.Receipts.amount,
                        paymentMethod = o.Receipts.paymentMethod,
                        paymentType = o.Receipts.paymentType,
                        createdAt = o.Receipts.createdAt
                    },
                    orderItems = o.OrderItems.Select(oi => new GetOrderItemsDto
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
                            soldQuantity = oi.Product.soldQuantity,
                            imageUrl = oi.Product.imageUrl,
                            createdAt = oi.Product.createdAt,
                            updatedAt = oi.Product.updatedAt,
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Orders?>> GetDailyOrders()
        {
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weekStart = today.AddDays(-1 * diff).Date;
            DateTime weekEnd = weekStart.AddDays(7).Date;
            return await _context.Orders.Where(o => o.createdAt >= weekStart && o.createdAt < weekEnd).ToListAsync();
        }

        public async Task<IEnumerable<Orders?>> GetWeeklyOrders()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayNextMonth = firstDayOfMonth.AddMonths(1);

            return await _context.Orders
            .Where(o => o.createdAt >= firstDayOfMonth && o.createdAt < firstDayNextMonth)
            .ToListAsync();
        }

        public async Task<bool> CreateOrderAsync(CreateOrderRequest request)
        {
            double totalAmount = 0;
            int totalQuantity = 0;

            foreach (var item in request.orderItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.productId == item.productId && p.stockQuantity > 0);

                if (product == null || product.stockQuantity < item.quantity) return false;

                double itemTotal = product.price * item.quantity;
                double discounted = itemTotal * ((100.0 - item.discountPercentage) / 100.0);

                totalAmount += discounted;
                totalQuantity += item.quantity;
            }

            var orderId = Guid.NewGuid();
            var order = new Orders
            {
                orderId = orderId,
                credentialId = request.credentialId,
                quantity = totalQuantity,
                amount = totalAmount,
                discountPercentage = request.discountPercentage,
                createdAt = DateTime.UtcNow.AddHours(8),
                status = 1, // Pending
                OrderItems = request.orderItems.Select(oi => new Order_Items
                {
                    orderItemId = Guid.NewGuid(),
                    orderId = orderId,
                    productId = oi.productId,
                    quantity = oi.quantity,
                    unitPrice = oi.unitPrice,
                    discountPercentage = oi.discountPercentage
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var receipt = new Receipts
            {
                receiptId = Guid.NewGuid(),
                credentialId = request.credentialId,
                orderId = orderId,
                amount = totalAmount,
                paymentMethod = request.paymentMethod,
                paymentType = request.paymentType,
                createdAt = DateTime.UtcNow.AddHours(8),
            };
            await _receiptRepository.CreateReceipt(receipt);

            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, int status)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.orderId == orderId);

            if (order == null) return false;

            order.status = status;
            order.updatedAt = DateTime.UtcNow.AddHours(8);

            if (status == 3)
            {
                order.fulfilledAt = DateTime.UtcNow.AddHours(8);

                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.productId == item.productId && p.stockQuantity > 0);

                    if (product == null || product.stockQuantity < item.quantity) return false;

                    product.stockQuantity -= item.quantity;
                    product.soldQuantity += item.quantity;
                }

            }
            else if (status == 4)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.productId == item.productId);
                    if (product != null)
                    {
                        product.soldQuantity -= item.quantity;
                        product.stockQuantity += item.quantity;
                        product.updatedAt = DateTime.UtcNow.AddHours(8);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
