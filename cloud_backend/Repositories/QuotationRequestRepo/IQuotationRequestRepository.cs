using cloud_backend.Dto;
using cloud_backend.Models;

namespace cloud_backend.Repositories.QuotationRequestRepo
{
    public interface IQuotationRequestRepository
    {
        Task<IEnumerable<GetQuotationRequestsDto>> GetQuotationRequestsDto();
        Task<GetQuotationRequestsDto?> GetQuotationRequestDtoById(Guid quotationId);
        Task<Quotation_Request?> GetQuotationRequestById(Guid quotationId);

        Task CreateQuotationRequest(Quotation_Request quotationRequest);
        Task UpdateQuotationRequest(Quotation_Request quotationRequest);
        Task DeleteQuotationRequest(Quotation_Request quotationRequest);
    }
}
