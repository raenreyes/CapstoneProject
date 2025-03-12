using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface IOrderDetailService
    {
        Task AddOrderDetail(OrderDetail orderDetail);
       
    }
}
