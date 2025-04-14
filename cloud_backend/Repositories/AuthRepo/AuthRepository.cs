using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Request;
using cloud_backend.Services;

namespace cloud_backend.Repositories.AuthRepo
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly HashService _hashService;

        public AuthRepository(AppDbContext context, HashService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        public async Task<User_Credentials?> AuthenticateAsync(CredentialRequest credential)
        {
            var userCredential = await _context.User_Credentials
                .FirstOrDefaultAsync(u => u.username == credential.username || u.email == credential.username);

            if (userCredential == null) return null;

            // Correct password verification
            if (userCredential.password != _hashService.HashPassword(credential.password))
                return null;

            userCredential.lastLogOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return userCredential;
        }

        public async Task<object?> GetUserDetailsAsync(User_Credentials userCredential)
        {
            switch (userCredential.role)
            {
                case 1:
                case 2:
                case 3: // Factory Staff
                    var factory = await _context.Staff_User
                        .FirstOrDefaultAsync(f => f.credentialId == userCredential.credentialId);
                    return factory != null && factory.isActive ? factory : null;

                case 4: // Store
                    var store = await _context.Store_User
                        .FirstOrDefaultAsync(s => s.credentialId == userCredential.credentialId);
                    return store != null && store.isActive ? store : null;

                case 5: // Customer
                    var customer = await _context.Customer_User
                        .FirstOrDefaultAsync(c => c.credentialId == userCredential.credentialId);
                    return customer != null && customer.isVerified ? customer : null;

                default:
                    return null;
            }
        }
    }
}
