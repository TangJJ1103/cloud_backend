using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Quotation;

namespace cloud_backend.Repositories.QuotationRequestRepo
{
    public interface IQuotationRequestRepository
    {
        Task<IEnumerable<GetQuotationRequestsDto>> GetQuotationRequestsDto();
        Task<GetPaginatedDto<GetQuotationRequestsDto>> GetQuotationRequestsDtoPaginated(QuotationRequestPaginationRequest request);
        Task<GetQuotationRequestsDto?> GetQuotationRequestDtoById(Guid quotationId);
        Task<Quotation_Request?> GetQuotationRequestById(Guid quotationId);

        Task CreateQuotationRequest(Quotation_Request quotationRequest);
        Task UpdateQuotationRequest(Quotation_Request quotationRequest);
        Task DeleteQuotationRequest(Quotation_Request quotationRequest);
    }
}
