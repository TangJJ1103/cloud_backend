using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Product;

namespace cloud_backend.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        Task<IEnumerable<GetProductsDto?>> GetAllProductsDto();
        Task<GetProductsDto?> GetProductDtoById(Guid productId);
        Task<Products?> GetProductById(Guid productId);
        Task<bool> CreateProduct(Products product);
        Task UpdateProduct(Products product);
    }
}
