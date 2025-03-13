using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.Services.Contracts;

namespace CapstoneProject.Services
{
    public class TicketService : ITicketService
    {
        private ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
