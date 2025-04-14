using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Request.Customer;

namespace cloud_backend.Repositories.CustomerRepo
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerFindRequest>> GetAllCustomers()
        {
            return await _context.Customer_User
                .Include(c => c.User_Credential)
                .Select(c => new CustomerFindRequest
                {
                    customerId = c.customerId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    address = c.address,
                    createdAt = c.createdAt,
                    isVerified = c.isVerified
                })
                .ToListAsync();
        }

        public async Task<CustomerFindRequest?> GetCustomerById(Guid customerId)
        {
            return await _context.Customer_User
                .Include(c => c.User_Credential)
                .Where(c => c.customerId == customerId)
                .Select(c => new CustomerFindRequest
                {
                    customerId = c.customerId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    address = c.address,
                    isVerified = c.isVerified
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerFindRequest?> GetCustomerByCredentialId(Guid credentialId)
        {
            return await _context.Customer_User
                .Include(c => c.User_Credential)
                .Where(c => c.credentialId == credentialId)
                .Select(c => new CustomerFindRequest
                {
                    customerId = c.customerId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    address = c.address,
                    isVerified = c.isVerified
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<object> customers, int totalRecords)> FindCustomers(CustomerPaginationRequest request)
        {
            var query = _context.Customer_User.Include(c => c.User_Credential).AsQueryable();

            if (request.isVerified.HasValue)
            {
                query = query.Where(c => c.isVerified == request.isVerified.Value);
            }

            if (!string.IsNullOrEmpty(request.filterBy))
            {
                query = request.filterBy.ToLower() switch
                {
                    "name" => query.OrderBy(c => c.User_Credential.name),
                    "email" => query.OrderBy(c => c.User_Credential.email),
                    "contactnumber" => query.OrderBy(c => c.User_Credential.contactNumber),
                    "lastlogon" => query.OrderByDescending(c => c.User_Credential.lastLogOn),
                    "createdat" => query.OrderByDescending(c => c.createdAt),
                    _ => query
                };
            }

            var totalRecords = await query.CountAsync();

            var customers = await query
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(c => new
                {
                    c.customerId,
                    c.User_Credential.name,
                    c.User_Credential.email,
                    c.User_Credential.contactNumber,
                    c.address,
                    c.isVerified
                })
                .ToListAsync();

            return (customers, totalRecords);
        }

        public async Task<bool> UpdateCustomer(Guid customerId, CustomerUpdateRequest request)
        {
            var customer = await _context.Customer_User
                .Include(c => c.User_Credential)
                .FirstOrDefaultAsync(c => c.customerId == customerId);

            if (customer == null)
                return false;

            var credentials = customer.User_Credential;

            var existingCredential = await _context.User_Credentials
                .Where(u => u.credentialId != credentials.credentialId &&
                       (u.username == request.username ||
                        u.email == request.email ||
                        u.contactNumber == request.contactNumber))
                .FirstOrDefaultAsync();

            if (existingCredential != null)
                return false; // You can handle error messages in the controller

            if (!string.IsNullOrEmpty(request.username))
                credentials.username = request.username;

            if (!string.IsNullOrEmpty(request.password))
                credentials.password = request.password;

            if (!string.IsNullOrEmpty(request.name))
                credentials.name = request.name;

            if (!string.IsNullOrEmpty(request.contactNumber))
                credentials.contactNumber = request.contactNumber;

            if (!string.IsNullOrEmpty(request.email))
                credentials.email = request.email;

            if (request.address != null)
                customer.address = request.address;

            customer.updatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer_User?> VerifyEmail(string token)
        {
            var user = await _context.Customer_User.FirstOrDefaultAsync(u => u.verificationToken == token);

            if (user == null)
                return null;

            user.updatedAt = DateTime.UtcNow;
            user.isVerified = true;
            user.verificationToken = null;

            await _context.SaveChangesAsync();
            return user;
        }

    }
}
