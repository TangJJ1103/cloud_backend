using cloud_backend.Data;
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
                    createdAt = (DateTime)c.createdAt
                })
                .ToListAsync();
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

        public async Task<(IEnumerable<object> staffs, int totalRecords)> FindStaffs(StaffPaginationRequest request)
        {
            var query = _context.Staff_User.Include(c => c.User_Credential).AsQueryable();

            if (request.isActive.HasValue)
            {
                query = query.Where(c => c.isActive == request.isActive.Value);
            }

            if (request.role.HasValue)
            {
                query = query.Where(c => c.User_Credential.role == request.role.Value);
            }

            if (!string.IsNullOrEmpty(request.filterBy))
            {
                query = request.filterBy.ToLower() switch
                {
                    "name" => query.OrderBy(c => c.User_Credential.name),
                    "email" => query.OrderBy(c => c.User_Credential.email),
                    "contactnumber" => query.OrderBy(c => c.User_Credential.contactNumber),
                    "lastlogon" => query.OrderByDescending(c => c.User_Credential.lastLogOn),
                    "createdat" => query.OrderByDescending(c => c.createdAt),
                    _ => query
                };
            }

            var totalRecords = await query.CountAsync();

            var staffs = await query
                .Skip(request.currentIndex)
                .Take(request.offset)
                .Select(c => new
                {
                    c.staffId,
                    c.User_Credential.name,
                    c.User_Credential.email,
                    c.User_Credential.contactNumber,
                    c.User_Credential.role,
                    c.isActive
                })
                .ToListAsync();

            return (staffs, totalRecords);
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

            staff.updatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
