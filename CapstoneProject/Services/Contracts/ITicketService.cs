using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface ITicketService
    {
        Task AddTicket(Ticket ticket);
    }
}
