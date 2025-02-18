using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.Services.Contracts;

namespace CapstoneProject.Services
{
    public class OrderheaderService : IOrderHeaderService
    {
        private readonly AppDbContext _context;

        public OrderheaderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveOrderHeader(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Add(orderHeader);
            await _context.SaveChangesAsync();
        }
    }
}
