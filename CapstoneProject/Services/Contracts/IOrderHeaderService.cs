using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface IOrderHeaderService
    {
        Task SaveOrderHeader(OrderHeader orderHeader);
        Task<OrderHeader> GetOrderHeaderById(int id);
    }
}
