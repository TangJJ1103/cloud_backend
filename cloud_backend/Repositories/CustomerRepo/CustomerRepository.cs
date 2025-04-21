using cloud_backend.Data;
using cloud_backend.Dto;
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
                    updatedAt = c.updatedAt,
                    isVerified = c.isVerified
                })
                .ToListAsync();
        }

        public async Task<GetPaginatedDto<CustomerFindRequest>> GetAllCustomersPaginated(CustomerPaginationRequest request)
        {
            var query = _context.Customer_User
            .Include(c => c.User_Credential)
            .AsQueryable();

            if (request.isVerified.HasValue)
                query = query.Where(c => c.isVerified == request.isVerified.Value);

            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                string term = request.searchTerm.ToLower();
                query = query.Where(c =>
                    c.User_Credential.name.ToLower().Contains(term) ||
                    c.User_Credential.email.ToLower().Contains(term) || 
                    c.credentialId.ToString().ToLower().Contains(term) ||
                    c.customerId.ToString().ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(c => c.createdAt)
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(c => new CustomerFindRequest
                {
                    customerId = c.customerId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    address = c.address,
                    createdAt = c.createdAt,
                    updatedAt = c.updatedAt,
                    isVerified = c.isVerified
                })
                .ToListAsync();

            return new GetPaginatedDto<CustomerFindRequest>
            {
                data = data,
                total = totalCount
            };
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
                    createdAt = c.createdAt,
                    updatedAt = c.updatedAt,
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
                    createdAt = c.createdAt,
                    updatedAt = c.updatedAt,
                    isVerified = c.isVerified
                })
                .FirstOrDefaultAsync();
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
                return false;

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

            customer.updatedAt = DateTime.UtcNow.AddHours(8);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer_User?> VerifyEmail(string token)
        {
            var user = await _context.Customer_User.FirstOrDefaultAsync(u => u.verificationToken == token);

            if (user == null)
                return null;

            user.updatedAt = DateTime.UtcNow.AddHours(8);
            user.isVerified = true;
            user.verificationToken = null;

            await _context.SaveChangesAsync();
            return user;
        }

    }
}
