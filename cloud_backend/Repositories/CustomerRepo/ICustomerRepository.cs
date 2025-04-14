using cloud_backend.Models;
using cloud_backend.Request.Customer;

namespace cloud_backend.Repositories.CustomerRepo
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerFindRequest>> GetAllCustomers();
        Task<CustomerFindRequest?> GetCustomerById(Guid customerId);
        Task<CustomerFindRequest?> GetCustomerByCredentialId(Guid credentialId);
        Task<(IEnumerable<object> customers, int totalRecords)> FindCustomers(CustomerPaginationRequest request);
        Task<bool> UpdateCustomer(Guid customerId, CustomerUpdateRequest request);
        Task<Customer_User?> VerifyEmail(string token);
    }
}
