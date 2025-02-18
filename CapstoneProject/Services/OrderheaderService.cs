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
            return await _context.OrderHeaders.Include(p => p.Product).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task SaveOrderHeader(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Add(orderHeader);
            await _context.SaveChangesAsync();
        }
    }
}
