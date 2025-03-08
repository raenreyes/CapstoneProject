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
        public async Task<IActionResult> Summary(int productId,int count)
        {
            var product = await _productService.GetProductById(productId);
            ShoppingCartVM shoppingCart = new ShoppingCartVM()
            {
                Product = product,
                OrderHeader = new OrderHeader() {ProductId = productId,Quantity = count, OrderTotal = (double)(count * product.ProductPrice) }
            };
            
            return View(shoppingCart);
        }
        public IActionResult Test()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(ShoppingCartVM shoppingCart)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                await _orderHeaderService.SaveOrderHeader(shoppingCart.OrderHeader);
                return RedirectToAction("StripePost", "Home", new { orderHeaderId = shoppingCart.OrderHeader.Id });
            }
            
            return View(shoppingCart);
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
           

            var domain = "http://localhost:5062";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                //SuccessUrl = domain + $"customer/cart/OrderConfirmation",
                SuccessUrl = $"{domain}/Home/OrderConfirmation/{orderHeaderId}",
                CancelUrl = domain,
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
                    Quantity = orderHeader.Quantity
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
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var orderHeader = await _orderHeaderService.GetOrderHeaderById(id);
            if (orderHeader == null)
            {
                return NotFound();
            }
            return View(orderHeader);
        }
    }
}
