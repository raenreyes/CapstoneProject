using CapstoneProject.Models;
using CapstoneProject.Models.VM;
using CapstoneProject.Services;
using CapstoneProject.Services.Contracts;
using CapstoneProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CapstoneProject.Controllers
{
    
    public class TicketController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITicketService _ticketService;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;


        public TicketController(ILogger<HomeController> logger,ITicketService ticketService, IOrderHeaderService orderHeaderService, IOrderDetailService orderDetailService)
        {
            _logger = logger;
            _ticketService = ticketService;
            _orderHeaderService = orderHeaderService;
            _orderDetailService = orderDetailService;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<Ticket> allTickets = await _ticketService.GetAllTickets();
            ViewBag.IsViewingCompleted = false;
            return View(allTickets);
        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompletedTickets()
        {
            List<Ticket> completedTickets = await _ticketService.GetCompletedTickets();
            ViewBag.IsViewingCompleted = true;
            return View("Index", completedTickets);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Orders(int ticketId)
        {
            List<OrderHeader> orders = await _orderHeaderService.GetAllOrderHeaders();
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            List<OrderDetail> orderDetails = await _orderDetailService.GetOrderDetailsByOrderHeaderId(orderId);
            if (orderDetails == null)
            {
                return NotFound();
            }
            return View(orderDetails);
        }
        [Authorize]
        public async Task<IActionResult> UserTickets()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<Ticket> userTickets = await _ticketService.GetUserTickets(userId);
            if (userTickets == null)
            {
                userTickets = new List<Ticket>();
            }
            
            return View(userTickets);
        }
    }
}
