using cloud_backend.Dto;
using cloud_backend.Request.Staff;

namespace cloud_backend.Repositories.StaffRepo
{
    public interface IStaffRepository
    {
        Task<IEnumerable<StaffFindRequest>> GetAllStaffs();
        Task<GetPaginatedDto<StaffFindRequest>> GetAllStaffsPaginated(StaffPaginationRequest request);
        Task<StaffFindRequest?> GetStaffById(Guid staffId);
        Task<bool> UpdateStaff(Guid staffId, StaffUpdateRequest request);
    }
}
