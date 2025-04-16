using cloud_backend.Data;
using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Customer;
using cloud_backend.Request.Manufacturing;

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

        public async Task<GetPaginatedDto<Manufacturing_Request>> GetManufacturingRequestsPaginated(ManufacturingPaginationRequest request)
        {
            var query = _context.Manufacturing_Request
                .Include(p => p.Products)
                .AsQueryable();

            if (request.status.HasValue)
                query = query.Where(c => c.status == request.status.Value);

            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                string term = request.searchTerm.ToLower();
                query = query.Where(c =>
                    c.requestId.ToString().ToLower().Contains(term) ||
                    c.productId.ToString().ToLower().Contains(term) ||
                    c.Products.name.ToLower().Contains(term) ||
                    c.Products.category.ToLower().Contains(term) ||
                    c.Products.model.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(c => c.createdAt)
                .Skip(request.currentIndex)
                .Take(request.offset).ToListAsync();

            return new GetPaginatedDto<Manufacturing_Request>
            {
                data = data,
                total = totalCount
            };
        }

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
