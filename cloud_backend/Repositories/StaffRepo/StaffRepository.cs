using cloud_backend.Data;
using cloud_backend.Dto;
using cloud_backend.Request.Customer;
using cloud_backend.Request.Staff;

namespace cloud_backend.Repositories.StaffRepo
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffFindRequest>> GetAllStaffs()
        {
            return await _context.Staff_User
                .Include(c => c.User_Credential)
                .Select(c => new StaffFindRequest
                {
                    staffId = c.staffId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    role = c.User_Credential.role,
                    isActive = c.isActive,
                    createdAt = c.createdAt
                })
                .ToListAsync();
        }

        public async Task<GetPaginatedDto<StaffFindRequest>> GetAllStaffsPaginated(StaffPaginationRequest request)
        {
            var query = _context.Staff_User
                .Include(c => c.User_Credential)
                .AsQueryable();

            if (request.isActive.HasValue)
                query = query.Where(c => c.isActive == request.isActive.Value);

            if(request.role.HasValue)
                query = query.Where(c => c.User_Credential.role == request.role.Value);

            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                string term = request.searchTerm.ToLower();
                query = query.Where(c =>
                    c.User_Credential.name.ToLower().Contains(term) ||
                    c.User_Credential.email.ToLower().Contains(term) ||
                    c.credentialId.ToString().ToLower().Contains(term) ||
                    c.staffId.ToString().ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(c => c.createdAt)
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(c => new StaffFindRequest
                {
                    staffId = c.staffId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    role = c.User_Credential.role,
                    isActive = c.isActive,
                    createdAt = c.createdAt
                })
                .ToListAsync();

            return new GetPaginatedDto<StaffFindRequest>
            {
                data = data,
                total = totalCount
            };
        }

        public async Task<StaffFindRequest?> GetStaffById(Guid staffId)
        {
            return await _context.Staff_User
                .Include(c => c.User_Credential)
                .Where(c => c.staffId == staffId)
                .Select(c => new StaffFindRequest
                {
                    staffId = c.staffId,
                    username = c.User_Credential.username,
                    name = c.User_Credential.name,
                    email = c.User_Credential.email,
                    contactNumber = c.User_Credential.contactNumber,
                    role = c.User_Credential.role,
                    isActive = c.isActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateStaff(Guid staffId, StaffUpdateRequest request)
        {
            var staff = await _context.Staff_User
                .Include(s => s.User_Credential)
                .FirstOrDefaultAsync(s => s.staffId == staffId);

            if (staff == null)
                return false;

            var credentials = staff.User_Credential;

            var existingCredential = await _context.User_Credentials
                .Where(u => u.credentialId != credentials.credentialId &&
                       (u.username == request.username ||
                        u.email == request.email ||
                        u.contactNumber == request.contactNumber))
                .FirstOrDefaultAsync();

            if (existingCredential != null)
                return false;

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

            if (request.role.HasValue)
                credentials.role = request.role.Value;

            if (request.isActive.HasValue)
                staff.isActive = request.isActive.Value;

            staff.updatedAt = DateTime.UtcNow.AddHours(8);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
