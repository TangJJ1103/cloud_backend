using cloud_backend.Dto;
using cloud_backend.Models;

namespace cloud_backend.Repositories.QuotationRepo
{
    public interface IQuotationRepository
    {
        Task<IEnumerable<GetQuotationsDto?>> GetQuotationsDto();
        Task<GetQuotationsDto?> GetQuotationDtoById(Guid quotationId);
        Task<Quotations?> GetQuotationById(Guid quotationId);

        Task CreateQuotation(Quotations quotation);
        Task UpdateQuotation(Quotations quotation);
        Task DeleteQuotation(Quotations quotation);
    }
}
