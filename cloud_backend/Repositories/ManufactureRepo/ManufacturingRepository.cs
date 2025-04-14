using cloud_backend.Data;
using cloud_backend.Models;

namespace cloud_backend.Repositories.ManufactureRepo
{
    public class ManufacturingRepository : IManufacturingRepository
    {
        private readonly AppDbContext _context;

        public ManufacturingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Manufacturing_Request?>> GetManufacturingRequests() =>
            await _context.Manufacturing_Request.Include(p => p.Products).ToListAsync();

        public async Task<Manufacturing_Request?> GetManufacturingRequest(Guid requestId) =>
            await _context.Manufacturing_Request.Include(p => p.Products).FirstOrDefaultAsync(r => r.requestId == requestId);

        public async Task CreateManufacturingRequest(Manufacturing_Request request)
        {
            _context.Manufacturing_Request.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateManufacturingRequest(Manufacturing_Request request)
        {
            _context.Manufacturing_Request.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteManufacturingRequest(Manufacturing_Request request)
        {
            _context.Manufacturing_Request.Remove(request);
            await _context.SaveChangesAsync();
        }
    }
}
