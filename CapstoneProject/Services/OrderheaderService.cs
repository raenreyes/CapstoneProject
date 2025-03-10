using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProject.Services
{
    public class OrderheaderService : IOrderHeaderService
    {
        private readonly AppDbContext _context;

        public OrderheaderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderHeader> GetOrderHeaderById(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return await _context.OrderHeaders
                .Include(o => o.Product) // Include the related Product entity
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<OrderHeader> GetOrderHeaderByPaymentIntentId(string paymentIntentId)
        {
            if (string.IsNullOrEmpty(paymentIntentId))
            {
                return null;
            }
            return await _context.OrderHeaders.Include(p => p.Product).FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);
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
    }
}
