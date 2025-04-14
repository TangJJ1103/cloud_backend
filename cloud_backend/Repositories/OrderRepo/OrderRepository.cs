using cloud_backend.Data;
using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Repositories.ReceiptRepo;
using cloud_backend.Request.Order;
using cloud_backend.Result;
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
                            createdAt = oi.Product.createdAt,
                            updatedAt = oi.Product.updatedAt,
                        }
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<GetOrdersDto?>> GetUserOrdersDto(Guid credentialId)
        {
            return await _context.Orders
                .Where(o => o.credentialId == credentialId)
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
                            createdAt = oi.Product.createdAt,
                            updatedAt = oi.Product.updatedAt,
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CreateOrderResult?> CreateOrderAsync(CreateOrderRequest request)
        {
            // Validate product availability
            double totalAmount = 0;
            int totalQuantity = 0;
            var orderItems = new List<Order_Items>();

            foreach (var item in request.orderItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.productId == item.productId && p.stockQuantity > 0);

                if (product == null || product.stockQuantity < item.quantity) return null;

                double itemTotal = product.price * item.quantity;
                totalAmount += itemTotal * ((100 - item.discountPercentage) / 100);
                totalQuantity += item.quantity;

                orderItems.Add(new Order_Items
                {
                    orderItemId = Guid.NewGuid(),
                    productId = item.productId,
                    quantity = item.quantity,
                    unitPrice = product.price,
                    discountPercentage = item.discountPercentage
                });

                product.stockQuantity -= item.quantity;
                product.soldQuantity += item.quantity;
            }

            var orderId = Guid.NewGuid();
            var order = new Orders
            {
                orderId = orderId,
                credentialId = request.credentialId,
                quantity = totalQuantity,
                amount = totalAmount,
                discountPercentage = request.discountPercentage,
                createdAt = DateTime.UtcNow,
                status = 1, // Pending
                OrderItems = orderItems
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
                createdAt = DateTime.UtcNow,
            };

            await _receiptRepository.CreateReceipt(receipt);

            return new CreateOrderResult { order = order, receipt = receipt };
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, int status)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.orderId == orderId);

            if (order == null) return false;

            order.status = status;
            order.updatedAt = DateTime.UtcNow;

            if (status == 3) // Fulfilled
            {
                order.fulfilledAt = DateTime.UtcNow;
            }
            else if (status == 4) // Cancelled -> Restore stock
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.productId == item.productId);
                    if (product != null)
                    {
                        product.soldQuantity -= item.quantity;
                        product.stockQuantity += item.quantity;
                        product.updatedAt = DateTime.UtcNow;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
