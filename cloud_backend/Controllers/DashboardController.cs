using cloud_backend.Repositories.OrderRepo;
using cloud_backend.Repositories.ProductRepo;
using cloud_backend.Repositories.QuotationRequestRepo;
using cloud_backend.Repositories.QuotationRepo;
using cloud_backend.Repositories.UserCredentialRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IUserCredentialRepository _userCredentialRepo;
        private readonly IQuotationRequestRepository _quotationRequestRepo;
        private readonly IQuotationRepository _quotationRepo;

        public DashboardController(
            IOrderRepository orderRepo, 
            IProductRepository productRepository, 
            IUserCredentialRepository userCredentialRepository,
            IQuotationRepository quotationRepository,
            IQuotationRequestRepository quotationRequestRepository
            )
        {
            _orderRepo = orderRepo;
            _productRepo = productRepository;
            _userCredentialRepo = userCredentialRepository;
            _quotationRepo = quotationRepository;
            _quotationRequestRepo = quotationRequestRepository;
        }

        #region admin
        [Authorize]
        [HttpGet("userData")]
        public async Task<ActionResult> GetUserData()
        {
            var userData = await _userCredentialRepo.getAllUserData();
            var customer = userData.Count(u => u.role == 5);
            var store = userData.Count(u => u.role == 4);
            var staff = userData.Count(u => u.role == 3);

            var result = new List<object>
            {
                new { title = "Total Customers", count = customer },
                new { title = "Total Stores", count = store },
                new { title = "Available Staff", count = staff },
            };

            return Ok(result);
        }

        //[Authorize]
        //[HttpGet("orderDailyData")]
        //public async Task<ActionResult> GetOrderDailyData()
        //{

        //}
        #endregion

        #region customer
        [Authorize]
        [HttpGet("customerData/{credentialId}")]
        public async Task<ActionResult> GetCustomerDashboardData(Guid credentialId)
        {
            if(credentialId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid input" });
            }

            var userOrders = await _orderRepo.GetUserOrdersDto(credentialId);
            if (!userOrders.Any())
            {
                return Ok(new List<object>
                {
                    new { title = "Processing Orders", count = 0 },
                    new { title = "Pending Orders", count = 0 },
                    new { title = "Completed Orders", count = 0 },
                    new { title = "Total Spent (RM)", count = 0.0 }
                });
            }

            var processingCount = userOrders.Count(o => o.status == 2);
            var pendingCount = userOrders.Count(o => o.status == 1); 
            var completedCount = userOrders.Count(o => o.status == 3);
            var totalSpent = userOrders
                .Where(o => o.status == 3)
                .Sum(o => o.amount);

            var result = new List<object>
            {
                new { title = "Processing Orders", count = processingCount },
                new { title = "Pending Orders", count = pendingCount },
                new { title = "Completed Orders", count = completedCount },
                new { title = "Total Spent (RM)", count = totalSpent }
            };

            return Ok(result);
        }
        #endregion

        #region store
        [Authorize]
        [HttpGet("stockData")]
        public async Task<ActionResult> GetStockData()
        {
            var stockData = await _productRepo.GetAllProductsDto();

            if(!stockData.Any())
            {
                return Ok(new { Available = 0, LowStock = 0, OutOfStock = 0 });
            }

            var Available = stockData.Count();
            var LowStock = stockData.Count(s => s.stockQuantity < 50);
            var OutOfStock = stockData.Count(o => o.stockQuantity == 0);

            return Ok(new { Available, LowStock, OutOfStock });
        }

        [Authorize]
        [HttpGet("lowStockData")]
        public async Task<ActionResult> GetLowStockData()
        {
            var stockData = await _productRepo.GetAllProductsDto();

            var lowStock = stockData
                .Where(p => p.stockQuantity < 50)
                .Select(p => new
                {
                    name = p.name,
                    quantity = p.stockQuantity,
                    model = p.model
                })
                .ToList();

            return Ok(lowStock);
        }

        [Authorize]
        [HttpGet("categorySalesData")]
        public async Task<ActionResult> GetCategorySalesData()
        {
            var stockData = await _productRepo.GetAllProductsDto();
            var result = stockData
                .GroupBy(p => p.category)
                .Select(g => new
                {
                    xAxis = g.Key,
                    value = g.Sum(p => p.soldQuantity)
                })
                .ToList();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("topSalesProductData")]
        public async Task<ActionResult> GetTopSalesProductData()
        {
            var stockData = await _productRepo.GetAllProductsDto();
            var result = stockData
                .OrderByDescending(p => p.soldQuantity)
                .Take(10)
                .Select(p => new
                {
                    xAxis = p.name,
                    sales = p.soldQuantity
                })
                .ToList();

            return Ok(result);
        }
        #endregion

        #region staff
        [Authorize]
        [HttpGet("orderAndQuotationData")]
        public async Task<ActionResult> GetOrderAndQuotationData()
        {
            var orders = await _orderRepo.GetAllOrdersDto();
            var quotations = await _quotationRepo.GetQuotationsDto();
            var quotationRequests = await _quotationRequestRepo.GetQuotationRequestsDto();

            var pendingOrders = orders.Count(o => o.status == 1);
            var pendingQuotations = quotations.Count(q => q.status == 1);
            var pendingQuotationRequests = quotationRequests.Count(qr => qr.status == 1);

            var result = new List<object>
            {
                new { title = "Pending Orders", count = pendingOrders },
                new { title = "Pending Quotations", count = pendingQuotations },
                new { title = "Pending Quotation Requests", count = pendingQuotationRequests },
            };

            return Ok(result);
        }

        [Authorize]
        [HttpGet("orderData")]
        public async Task<ActionResult> GetOrderData()
        {
            var orders = await _orderRepo.GetAllOrdersDto();

            var pendingOrders = orders.Count(o => o.status == 1);
            var processingOrders = orders.Count(o => o.status == 2);
            var completedOrders = orders.Count(o => o.status == 3);
            var rejectedOrders = orders.Count(o => o.status == 4);

            var result = new List<object>
            {
                new { title = "Pending", count = pendingOrders },
                new { title = "Processing", count = processingOrders },
                new { title = "Completed", count = completedOrders },
                new { title = "Rejected", count = rejectedOrders },
            };

            return Ok(result);
        }

        [Authorize]
        [HttpGet("quotationData")]
        public async Task<ActionResult> GetQuotationData()
        {
            var quotations = await _quotationRepo.GetQuotationsDto();

            var pendingQuotations = quotations.Count(o => o.status == 1);
            var processingQuotations = quotations.Count(o => o.status == 2);
            var completedQuotations = quotations.Count(o => o.status == 3);
            var rejectedQuotations = quotations.Count(o => o.status == 4);

            var result = new List<object>
            {
                new { title = "Pending", count = pendingQuotations },
                new { title = "Processing", count = processingQuotations },
                new { title = "Completed", count = completedQuotations },
                new { title = "Rejected", count = rejectedQuotations },
            };

            return Ok(result);
        }


        [Authorize]
        [HttpGet("quotationRequestData")]
        public async Task<ActionResult> GetQuotationRequestData()
        {
            var quotationRequests = await _quotationRequestRepo.GetQuotationRequestsDto();

            var pendingQuotationRequests = quotationRequests.Count(o => o.status == 1);
            var processingQuotationRequests = quotationRequests.Count(o => o.status == 2);
            var completedQuotationRequests = quotationRequests.Count(o => o.status == 3);
            var rejectedQuotationRequests = quotationRequests.Count(o => o.status == 4);

            var result = new List<object>
            {
                new { title = "Pending", count = pendingQuotationRequests },
                new { title = "Processing", count = processingQuotationRequests },
                new { title = "Completed", count = completedQuotationRequests },
                new { title = "Rejected", count = rejectedQuotationRequests },
            };

            return Ok(result);
        }
        #endregion
    }
}
