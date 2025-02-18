using System.Diagnostics;
using CapstoneProject.Models;
using CapstoneProject.Models.VM;
using CapstoneProject.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IOrderHeaderService _orderHeaderService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IOrderHeaderService orderHeaderService)
        {
            _logger = logger;
            _productService = productService;
            _orderHeaderService = orderHeaderService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProducts();
            return View(products);
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _productService.GetProductById(productId);
            return View(product);
        }
        public async Task<IActionResult> Summary(int productId)
        {
            ShoppingCartVM shoppingCart = new ShoppingCartVM()
            {
                Product = await _productService.GetProductById(productId),
                OrderHeader = new OrderHeader() {ProductId = productId }
            };
            
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(ShoppingCartVM shoppingCart)
        {
                await _orderHeaderService.SaveOrderHeader(shoppingCart.OrderHeader);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
