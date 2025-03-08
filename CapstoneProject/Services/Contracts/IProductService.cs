using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Product>> GetAllServices();
        Task <Product> GetProductById(int id);
    }
}
