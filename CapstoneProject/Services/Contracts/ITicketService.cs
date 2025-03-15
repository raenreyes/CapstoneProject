using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface ITicketService
    {
        Task AddTicket(Ticket ticket);
        Task<List<Ticket>> GetAllTickets();
        Task<List<Ticket>> GetCompletedTickets();
        Task MarkTicketComplete(Ticket ticket);
        Task<Ticket> GetTicket(int ticketId);
        Task <List<Ticket>> GetUserTickets(string userId);
    }
}
