using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProject.Services
{
    public class OrderheaderService : IOrderHeaderService
    {
        private readonly ApplicationDbContext _context;

        public OrderheaderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderHeader>> GetAllOrderHeaders()
        {
            return await _context.OrderHeaders.Where(u => u.PaymentStatus == "succeeded").ToListAsync();
        }

        public async Task<OrderHeader> GetOrderHeaderById(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return await _context.OrderHeaders.FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<OrderHeader> GetOrderHeaderByPaymentIntentId(string paymentIntentId)
        {
            if (string.IsNullOrEmpty(paymentIntentId))
            {
                return null;
            }
            return await _context.OrderHeaders.FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);
        }

        public async Task SaveOrderHeader(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Add(orderHeader);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateOrderHeader(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
            await _context.SaveChangesAsync();
        }



        //public Task SaveOrderHeader(OrderHeader orderHeader)
        //{
        //    throw new NotImplementedException();
        //}

       
    }
}
