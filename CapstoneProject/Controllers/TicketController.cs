using CapstoneProject.Models;
using CapstoneProject.Services;
using CapstoneProject.Services.Contracts;
using CapstoneProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CapstoneProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TicketController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITicketService _ticketService;
        

        public TicketController(ILogger<HomeController> logger,ITicketService ticketService)
        {
            _logger = logger;
            _ticketService = ticketService;
        }
        public async Task<IActionResult> Index()
        {
            List<Ticket> allTickets = await _ticketService.GetAllTickets();
            return View(allTickets);
        }
        public async Task<IActionResult> Completed(int ticketId)
        {
            Ticket ticketToComplete = await _ticketService.GetTicket(ticketId);
            if (ticketToComplete != null)
            {
                ticketToComplete.IsCompleted = true;
                await _ticketService.MarkTicketComplete(ticketToComplete);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
