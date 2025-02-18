using System.Diagnostics;
using CapstoneProject.Models;
using CapstoneProject.Models.VM;
using CapstoneProject.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;

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
            return RedirectToAction("StripePost", "Home", new { orderHeaderId = shoppingCart.OrderHeader.Id });

        }
        [HttpGet]
        public async Task<IActionResult> StripePost(int orderHeaderId)
        {
            return await StripePostInternal(orderHeaderId);
        }
        //this is where all the stripe checkout logic will go
        [HttpPost]
        //[Route("Home/StripePost/{orderHeaderId}")]
        public async Task<IActionResult> StripePostInternal(int orderHeaderId)
        {
            var orderHeader = await _orderHeaderService.GetOrderHeaderById(orderHeaderId);
            if (orderHeader == null)
            {
                return NotFound();
            }
            string productName = orderHeader.Product.ProductName;

            var domain = "https://localhost:7090/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                //SuccessUrl = domain + $"customer/cart/OrderConfirmation",
                SuccessUrl = "https://nike.com",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };
            
                var sessionLineItems = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(orderHeader.Product.ProductPrice * 100),//$20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = orderHeader.Product.ProductName
                        }

                    },
                    Quantity = 1
                };
                options.LineItems.Add(sessionLineItems);
            
            var service = new Stripe.Checkout.SessionService();
            Session session = service.Create(options);
            

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


            
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
