using cloud_backend.Models;

namespace cloud_backend.Repositories.ManufactureRepo
{
    public interface IManufacturingRepository
    {
        Task<IEnumerable<Manufacturing_Request?>> GetManufacturingRequests();
        Task<Manufacturing_Request?> GetManufacturingRequest(Guid requestId);
        Task CreateManufacturingRequest(Manufacturing_Request request);
        Task UpdateManufacturingRequest(Manufacturing_Request request);
        Task DeleteManufacturingRequest(Manufacturing_Request request);
    }
}
