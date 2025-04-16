using cloud_backend.Dto;
using cloud_backend.Request.Store;

namespace cloud_backend.Repositories.StoreRepo
{
    public interface IStoreRepository
    {
        Task<IEnumerable<StoreFindRequest>> GetAllStores();
        Task<GetPaginatedDto<StoreFindRequest>> GetAllStoresPaginated(StorePaginationRequest request);
        Task<StoreFindRequest?> GetStoreById(Guid storeId);
        Task<StoreFindRequest?> GetStoreByCredentialId(Guid credentialId);
        Task<bool> IsStoreActive(Guid storeId);
        Task<bool> UpdateStore(Guid storeId, StoreUpdateRequest request);
    }
}
