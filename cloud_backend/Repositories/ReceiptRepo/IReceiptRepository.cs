using cloud_backend.Dto;
using cloud_backend.Models;

namespace cloud_backend.Repositories.ReceiptRepo
{
    public interface IReceiptRepository
    {
        Task<IEnumerable<GetReceiptsDto>> GetAllReceiptsDto();
        Task<GetReceiptsDto> GetOneReceiptDto(Guid receiptId);
        Task<Receipts> GetOneReceipt(Guid receiptId);
        Task CreateReceipt(Receipts receipt);
        Task DeleteReceipt(Receipts receipt);
    }
}
