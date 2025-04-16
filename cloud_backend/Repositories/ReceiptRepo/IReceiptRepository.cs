using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Receipts;

namespace cloud_backend.Repositories.ReceiptRepo
{
    public interface IReceiptRepository
    {
        Task<IEnumerable<GetReceiptsDto>> GetAllReceiptsDto();
        Task<GetPaginatedDto<GetReceiptsDto>> GetAllReceiptsDtoPaginated(ReceiptPaginationRequest request);
        Task<GetReceiptsDto> GetOneReceiptDto(Guid receiptId);
        Task<Receipts> GetOneReceipt(Guid receiptId);
        Task CreateReceipt(Receipts receipt);
        Task DeleteReceipt(Receipts receipt);
    }
}
