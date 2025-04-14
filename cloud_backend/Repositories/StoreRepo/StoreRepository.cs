using cloud_backend.Data;
using cloud_backend.Request.Store;

namespace cloud_backend.Repositories.StoreRepo
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _context;

        public StoreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StoreFindRequest>> GetAllStores()
        {
            return await _context.Store_User
                .Include(c => c.User_Credential)
                .Select(c => new StoreFindRequest
                {
                    storeId = c.storeId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    address = c.address,
                    isActive = c.isActive
                }).ToListAsync();
        }

        public async Task<StoreFindRequest?> GetStoreById(Guid storeId)
        {
            return await _context.Store_User
                .Include(c => c.User_Credential)
                .Where(c => c.storeId == storeId)
                .Select(c => new StoreFindRequest
                {
                    storeId = c.storeId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    address = c.address,
                    isActive = c.isActive
                }).FirstOrDefaultAsync();
        }

        public async Task<StoreFindRequest?> GetStoreByCredentialId(Guid credentialId)
        {
            return await _context.Store_User
                .Include(c => c.User_Credential)
                .Where(c => c.credentialId == credentialId)
                .Select(c => new StoreFindRequest
                {
                    storeId = c.storeId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    address = c.address,
                    isActive = c.isActive
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> IsStoreActive(Guid storeId)
        {
            return await _context.Store_User
                .AnyAsync(s => s.storeId == storeId && s.isActive);
        }

        public async Task<(IEnumerable<object> Stores, int TotalRecords)> FindStores(StorePaginationRequest request)
        {
            var query = _context.Store_User
                .Include(c => c.User_Credential)
                .AsQueryable();

            if (request.isActive.HasValue)
                query = query.Where(c => c.isActive == request.isActive.Value);

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

            int totalRecords = await query.CountAsync();

            var stores = await query
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(c => new
                {
                    c.storeId,
                    c.User_Credential.name,
                    c.User_Credential.email,
                    c.User_Credential.contactNumber,
                    c.address,
                    c.isActive
                })
                .ToListAsync();

            return (stores, totalRecords);
        }

        public async Task<bool> UpdateStore(Guid storeId, StoreUpdateRequest request)
        {
            var store = await _context.Store_User
                .Include(s => s.User_Credential)
                .FirstOrDefaultAsync(s => s.storeId == storeId);

            if (store == null) return false;

            var credentials = store.User_Credential;

            var existingCredential = await _context.User_Credentials
                .Where(u => u.credentialId != credentials.credentialId &&
                        (u.username == request.username ||
                        u.email == request.email ||
                        u.contactNumber == request.contactNumber))
                .FirstOrDefaultAsync();

            if (existingCredential != null) return false;

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

            if (request.isActive.HasValue)
                store.isActive = request.isActive.Value;

            if (!string.IsNullOrEmpty(request.address))
                store.address = request.address;

            store.updatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
    
}
