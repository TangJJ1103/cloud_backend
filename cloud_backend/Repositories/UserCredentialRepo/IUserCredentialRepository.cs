using cloud_backend.Models;

namespace cloud_backend.Repositories.UserCredentialRepo
{
    public interface IUserCredentialRepository
    {
        Task<bool> IsStoreOrCustomer(Guid credentialId);
        Task<IEnumerable<User_Credentials?>> getAllUserData();
    }
}
