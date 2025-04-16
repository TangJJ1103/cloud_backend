using cloud_backend.Data;
using cloud_backend.Models;

namespace cloud_backend.Repositories.UserCredentialRepo
{
    public class UserCredentialRepository : IUserCredentialRepository
    {
        private readonly AppDbContext _context;
        
        public UserCredentialRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<bool> IsStoreOrCustomer(Guid credentialId)
        {
            var userRole = await _context.User_Credentials.FirstOrDefaultAsync(u => u.credentialId == credentialId);
            Console.WriteLine("the user role" + userRole.role);
            if (userRole == null)
            {
                return false;
            }

            if (userRole.role != 4 && userRole.role != 5)
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<User_Credentials?>> getAllUserData() =>
            await _context.User_Credentials.ToListAsync();
    }
}
