using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Manufacturing;

namespace cloud_backend.Repositories.ManufactureRepo
{
    public interface IManufacturingRepository
    {
        Task<IEnumerable<Manufacturing_Request?>> GetManufacturingRequests();
        Task<GetPaginatedDto<Manufacturing_Request>> GetManufacturingRequestsPaginated(ManufacturingPaginationRequest request);
        Task<Manufacturing_Request?> GetManufacturingRequest(Guid requestId);
        Task CreateManufacturingRequest(Manufacturing_Request request);
        Task UpdateManufacturingRequest(Manufacturing_Request request);
        Task DeleteManufacturingRequest(Manufacturing_Request request);
    }
}
