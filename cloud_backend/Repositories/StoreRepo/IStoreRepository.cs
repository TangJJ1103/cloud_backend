using cloud_backend.Request.Store;

namespace cloud_backend.Repositories.StoreRepo
{
    public interface IStoreRepository
    {
        Task<IEnumerable<StoreFindRequest>> GetAllStores();
        Task<StoreFindRequest?> GetStoreById(Guid storeId);
        Task<StoreFindRequest?> GetStoreByCredentialId(Guid credentialId);
        Task<bool> IsStoreActive(Guid storeId);
        Task<(IEnumerable<object> Stores, int TotalRecords)> FindStores(StorePaginationRequest request);
        Task<bool> UpdateStore(Guid storeId, StoreUpdateRequest request);
    }
}
