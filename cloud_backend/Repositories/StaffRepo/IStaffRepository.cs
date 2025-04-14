using cloud_backend.Request.Staff;

namespace cloud_backend.Repositories.StaffRepo
{
    public interface IStaffRepository
    {
        Task<IEnumerable<StaffFindRequest>> GetAllStaffs();
        Task<StaffFindRequest?> GetStaffById(Guid staffId);
        Task<(IEnumerable<object> staffs, int totalRecords)> FindStaffs(StaffPaginationRequest request);
        Task<bool> UpdateStaff(Guid staffId, StaffUpdateRequest request);
    }
}
