using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.Services.Contracts;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<Ticket>> GetAllTickets()
        {
            return await _context.Tickets.Where(u => u.IsCompleted == false).Include(u => u.User).ToListAsync();
        }
        public async Task<List<Ticket>> GetCompletedTickets()
        {
            return await _context.Tickets.Where(t => t.IsCompleted).Include(u => u.User).ToListAsync();
        }
        public async Task<Ticket> GetTicket(int ticketId)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }

        public async Task MarkTicketComplete(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
