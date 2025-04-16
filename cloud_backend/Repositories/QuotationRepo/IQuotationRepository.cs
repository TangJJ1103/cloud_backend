using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Quotation;

namespace cloud_backend.Repositories.QuotationRepo
{
    public interface IQuotationRepository
    {
        Task<IEnumerable<GetQuotationsDto?>> GetQuotationsDto();
        Task<GetPaginatedDto<GetQuotationsDto>> GetQuotationsDtoPaginated(QuotationPaginationRequest request);
        Task<GetQuotationsDto?> GetQuotationDtoById(Guid quotationId);
        Task<Quotations?> GetQuotationById(Guid quotationId);

        Task CreateQuotation(Quotations quotation);
        Task UpdateQuotation(Quotations quotation);
        Task DeleteQuotation(Quotations quotation);
    }
}
