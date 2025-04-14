using cloud_backend.Models;
using cloud_backend.Request;

namespace cloud_backend.Repositories.AuthRepo
{
    public interface IAuthRepository
    {
        Task<User_Credentials?> AuthenticateAsync(CredentialRequest credential);
        Task<object?> GetUserDetailsAsync(User_Credentials userCredential);
    }
}
