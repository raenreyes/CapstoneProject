using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task <Product> GetProductById(int id);
    }
}
