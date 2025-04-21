using cloud_backend.Repositories.OrderRepo;
using cloud_backend.Repositories.ProductRepo;
using cloud_backend.Repositories.QuotationRequestRepo;
using cloud_backend.Repositories.QuotationRepo;
using cloud_backend.Repositories.UserCredentialRepo;
using cloud_backend.Repositories.CustomerRepo;
using cloud_backend.Repositories.StoreRepo;
using cloud_backend.Repositories.StaffRepo;
using cloud_backend.Repositories.ManufactureRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using cloud_backend.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly ICustomerRepository _customerRepo;
        private readonly IStoreRepository _storeRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IManufacturingRepository _manufacturingRequestRepo;

        public DashboardController(
            IOrderRepository orderRepo, 
            IProductRepository productRepository, 
            IUserCredentialRepository userCredentialRepository,
            IQuotationRepository quotationRepository,
            IQuotationRequestRepository quotationRequestRepository,
            ICustomerRepository customerRepository,
            IStoreRepository storeRepository,
            IStaffRepository staffRepository,
            IManufacturingRepository manufacturingRepository
            )
        {
            _orderRepo = orderRepo;
            _productRepo = productRepository;
            _userCredentialRepo = userCredentialRepository;
            _quotationRepo = quotationRepository;
            _quotationRequestRepo = quotationRequestRepository;
            _customerRepo = customerRepository;
            _storeRepo = storeRepository;
            _staffRepo = staffRepository;
            _manufacturingRequestRepo = manufacturingRepository;
        }

        #region superAdmin
        [Authorize]
        [HttpGet("orderQuotationAndUserData")]
        public async Task<ActionResult> GetOrderQuotationAndUserData()
        {
            var userData = await _userCredentialRepo.getAllUserData();
            var customer = userData.Count(u => u.role == 5);
            var store = userData.Count(u => u.role == 4);
            var staff = userData.Count(u => u.role == 3);

            var orders = await _orderRepo.GetAllOrdersDto();
            var quotations = await _quotationRepo.GetQuotationsDto();
            var quotationRequests = await _quotationRequestRepo.GetQuotationRequestsDto();

            var pendingOrders = orders.Count(o => o.status == 1);
            var pendingQuotations = quotations.Count(q => q.status == 1);
            var pendingQuotationRequests = quotationRequests.Count(qr => qr.status == 1);

            var result = new List<object>
            {
                new { title = "Total Customers", count = customer },
                new { title = "Total Stores", count = store },
                new { title = "Available Staff", count = staff },
                new { title = "Pending Orders", count = pendingOrders },
                new { title = "Pending Quotations", count = pendingQuotations },
                new { title = "Pending Quotation Requests", count = pendingQuotationRequests },
            };

            return Ok(result);
        }

        [Authorize]
        [HttpGet("revenueDailyData")]
        public async Task<ActionResult> GetDailyRevenueData()
        {
            var today = DateTime.UtcNow.AddHours(8).Date;

            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6);

            var ordersThisWeek = (await _orderRepo.GetAllOrdersDto())
                .Where(o => o.createdAt.Date >= startOfWeek && o.createdAt.Date <= endOfWeek)
                .ToList();

            var requestsThisWeek = (await _manufacturingRequestRepo.GetManufacturingRequests())
                .Where(m => m.createdAt.Date >= startOfWeek && m.createdAt.Date <= endOfWeek && m.status == 3)
                .ToList();

            var result = new List<object>();

            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);

                var revenue = ordersThisWeek
                    .Where(o => o.createdAt.Date == date)
                    .Sum(o => o.amount);

                var expenses = requestsThisWeek
                    .Where(m => m.createdAt.Date == date)
                    .Sum(m => m.cost * m.quantity);

                result.Add(new
                {
                    xAxis = date.ToString("yyyy-MM-dd"),
                    Revenue = revenue,
                    Expenses = expenses,
                    Profit = revenue - expenses
                });
            }

            return Ok(result);
        }
        #endregion

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

        [Authorize]
        [HttpGet("orderDailyData")]
        public async Task<ActionResult> GetOrderDailyData()
        {
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weekStart = today.AddDays(-1 * diff).Date;
            DateTime weekEnd = weekStart.AddDays(7).Date;

            var orders = await _orderRepo.GetDailyOrders();

            var groupedOrders = orders
                .GroupBy(o => o.createdAt.Date)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Pending = g.Count(o => o.status == 0),
                        Processing = g.Count(o => o.status == 1),
                        Completed = g.Count(o => o.status == 2),
                        Rejected = g.Count(o => o.status == 3)
                    });

            var result = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = weekStart.AddDays(offset).Date;
                    var data = groupedOrders.ContainsKey(date) ? groupedOrders[date] : new { Pending = 0, Processing = 0, Completed = 0, Rejected = 0 };

                    return new
                    {
                        xAxis = date.ToString("yyyy-MM-dd"),
                        data.Pending,
                        data.Processing,
                        data.Completed,
                        data.Rejected
                    };
                })
                .ToList();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("orderWeeklyData")]
        public async Task<ActionResult> GetOrderWeeklyData()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayNextMonth = firstDayOfMonth.AddMonths(1);

            var orders = await _orderRepo.GetWeeklyOrders();

            var result = orders
                .GroupBy(o =>
                {
                    var weekNumber = ((o.createdAt.Day - 1) / 7) + 1;
                    return $"Week {weekNumber}";
                })
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    xAxis = g.Key,
                    Pending = g.Count(o => o.status == 0),
                    Processing = g.Count(o => o.status == 1),
                    Completed = g.Count(o => o.status == 2),
                    Rejected = g.Count(o => o.status == 3)
                })
                .ToList();

            return Ok(result);
        }

        
        [HttpGet("monthlyUserData")]
        public async Task<ActionResult> GetMonthlyUserData()
        {
            var currentYear = DateTime.Today.Year;

            var customers = (await _customerRepo.GetAllCustomers())
                .Where(c => c.createdAt.Year == currentYear && (c.isVerified ?? false))
                .ToList();

            var staff = (await _staffRepo.GetAllStaffs())
                .Where(s => s.createdAt.Year == currentYear && s.isActive)
                .ToList();

            var store = (await _storeRepo.GetAllStores())
                .Where(s => s.createdAt.Year == currentYear && s.isActive)
                .ToList();

            var admin = staff.Where(s => s.role == 2).ToList();
            var superAdmin = staff.Where(s => s.role == 1).ToList();

            Console.WriteLine("Super Admin Count1: " + superAdmin.Count());

            var monthNames = Enumerable.Range(1, 12)
                .Select(i => new DateTime(currentYear, i, 1).ToString("MMM"))
                .ToList();

            var result = monthNames.Select((monthName, index) =>
            {
                var monthNumber = index + 1;

                var customersInMonth = customers
                    .Where(u => u.createdAt.Month == monthNumber);

                var staffInMonth = staff
                    .Where(u => u.createdAt.Month == monthNumber);

                var storeInMonth = store
                    .Where(u => u.createdAt.Month == monthNumber);

                var adminInMonth = admin
                    .Where(u => u.createdAt.Month == monthNumber);

                var superAdminInMonth = superAdmin
                    .Where(u => u.createdAt.Month == monthNumber);
                Console.WriteLine("Super Admin Count2: " + superAdminInMonth.Count());
                return new
                {
                    xAxis = monthName,
                    SuperAdmin = superAdminInMonth.Count(),
                    Admin = adminInMonth.Count(),
                    Staff = staffInMonth.Count(u => u.role == 3),
                    Store = storeInMonth.Count(),
                    Customer = customersInMonth.Count()
                };
            }).ToList();

            return Ok(result);
        }
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

        [Authorize]
        [HttpGet("customerRecentOrders/{credentialId}")]
        public async Task<ActionResult> GetCustomerRecentOrders(Guid credentialId)
        {
            if (credentialId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid input" });
            }

            var userOrders = await _orderRepo.GetUserOrdersDto(credentialId);
            if (!userOrders.Any())
            {
                return Ok(new List<object> { });
            }

            var result = userOrders
                .OrderByDescending(order => order.updatedAt ?? order.createdAt)
                .Take(5);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("promotionalProducts")]
        public async Task<ActionResult> GetPromotionalProducts()
        {
            var products = await _productRepo.GetAllProductsDto();
            if (!products.Any())
            {
                return Ok(new List<object> { });
            }

            var result = products.OrderByDescending(p => p.discountPercentage).Take(10);

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
            var acceptedQuotations = quotations.Count(o => o.status == 2);
            var rejectedQuotations = quotations.Count(o => o.status == 3);
            var cancelledQuotations = quotations.Count(o => o.status == 4);

            var result = new List<object>
            {
                new { title = "Pending", count = pendingQuotations },
                new { title = "Accepted", count = acceptedQuotations },
                new { title = "Rejected", count = rejectedQuotations },
                new { title = "Cancelled", count = cancelledQuotations },
            };

            return Ok(result);
        }


        [Authorize]
        [HttpGet("quotationRequestData")]
        public async Task<ActionResult> GetQuotationRequestData()
        {
            var quotationRequests = await _quotationRequestRepo.GetQuotationRequestsDto();

            var pendingQuotationRequests = quotationRequests.Count(o => o.status == 1);
            var acceptedQuotationRequests = quotationRequests.Count(o => o.status == 2);
            var rejectedQuotationRequests = quotationRequests.Count(o => o.status == 3);
            var cancelledQuotationRequests = quotationRequests.Count(o => o.status == 4);

            var result = new List<object>
            {
                new { title = "Pending", count = pendingQuotationRequests },
                new { title = "Accepted", count = acceptedQuotationRequests },
                new { title = "Rejected", count = rejectedQuotationRequests },
                new { title = "Cancelled", count = cancelledQuotationRequests },
            };

            return Ok(result);
        }
        #endregion
    }
}
