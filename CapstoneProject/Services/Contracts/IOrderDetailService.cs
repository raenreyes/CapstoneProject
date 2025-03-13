using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface IOrderDetailService
    {
        Task AddOrderDetail(OrderDetail orderDetail);
       Task<List<OrderDetail>> GetOrderDetailsByOrderHeaderId(int orderHeaderId);
    }
}
