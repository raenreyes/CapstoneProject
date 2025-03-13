using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface IOrderHeaderService
    {
        Task SaveOrderHeader(OrderHeader orderHeader);
        Task<OrderHeader> GetOrderHeaderById(int id);
        Task<OrderHeader> GetOrderHeaderByPaymentIntentId(string paymentIntentId);
        Task UpdateOrderHeader(OrderHeader orderHeader);
        Task<List<OrderHeader>> GetAllOrderHeaders();
    }
}
