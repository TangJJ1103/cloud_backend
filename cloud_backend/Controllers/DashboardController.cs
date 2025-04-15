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
            var today = DateTime.UtcNow.Date;

            // Get Monday of the current week
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6);

            // Fetch orders for this week
            var ordersThisWeek = (await _orderRepo.GetAllOrdersDto())
                .Where(o => o.createdAt.Value.Date >= startOfWeek && o.createdAt.Value.Date <= endOfWeek)
                .ToList();

            // Fetch manufacturing requests with status == 3 for this week
            var requestsThisWeek = (await _manufacturingRequestRepo.GetManufacturingRequests())
                .Where(m => m.createdAt.Value.Date >= startOfWeek && m.createdAt.Value.Date <= endOfWeek && m.status == 3)
                .ToList();

            var result = new List<object>();

            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);

                var revenue = ordersThisWeek
                    .Where(o => o.createdAt.Value.Date == date)
                    .Sum(o => o.amount);

                var expenses = requestsThisWeek
                    .Where(m => m.createdAt.Value.Date == date)
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

            // 2. Fetch orders in current week
            var orders = await _orderRepo.GetDailyOrders();

            // 3. Group orders by date
            var groupedOrders = orders
                .GroupBy(o => o.createdAt.Value.Date)
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
                    var weekNumber = ((o.createdAt.Value.Day - 1) / 7) + 1;
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

        [Authorize]
        [HttpGet("monthlyUserData")]
        public async Task<ActionResult> GetMonthlyUserData()
        {
            var currentYear = DateTime.Today.Year;

            // Fetch all the customers, staff, stores, and admins for the current year
            var customers = (await _customerRepo.GetAllCustomers())
                .Where(c => c.createdAt.Year == currentYear && (c.isVerified ?? false))
                .ToList();

            var staff = (await _staffRepo.GetAllStaffs())
                .Where(s => s.createdAt.Year == currentYear && s.isActive)
                .ToList();

            var store = (await _storeRepo.GetAllStores())
                .Where(s => s.createdAt.Year == currentYear && s.isActive)
                .ToList();

            var admin = staff.Where(s => s.role == 4).ToList();
            var superAdmin = staff.Where(s => s.role == 5).ToList();

            // Combine all users into a single collection
            var allUsers = customers
                .Select(c => new { c.createdAt, userRole = "Customer" })
                .Concat(staff.Select(s => new { s.createdAt, userRole = s.role == 4 ? "Admin" : s.role == 5 ? "SuperAdmin" : "Staff" }))
                .Concat(store.Select(s => new { s.createdAt, userRole = "Store" }))
                .ToList();

            var monthNames = Enumerable.Range(1, 12)
                .Select(i => new DateTime(currentYear, i, 1).ToString("MMM"))
                .ToList();

            var result = monthNames.Select((monthName, index) =>
            {
                var monthNumber = index + 1;

                // Filter the users based on the month
                var usersInMonth = allUsers
                    .Where(u => u.createdAt.Month == monthNumber);

                return new
                {
                    xAxis = monthName,
                    SuperAdmin = usersInMonth.Count(u => u.userRole == "SuperAdmin"),
                    Admin = usersInMonth.Count(u => u.userRole == "Admin"),
                    Staff = usersInMonth.Count(u => u.userRole == "Staff"),
                    Store = usersInMonth.Count(u => u.userRole == "Store"),
                    Customer = usersInMonth.Count(u => u.userRole == "Customer")
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
