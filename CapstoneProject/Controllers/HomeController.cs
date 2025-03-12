using System.Diagnostics;
using CapstoneProject.Models;
using CapstoneProject.Models.VM;
using CapstoneProject.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using CapstoneProject.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace CapstoneProject.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly StripeSettings _stripeSettings;

        public HomeController(ILogger<HomeController> logger, IOptions<StripeSettings> stripeSettings, IProductService productService, IOrderHeaderService orderHeaderService)
        {
            _logger = logger;
            _productService = productService;
            _orderHeaderService = orderHeaderService;
            _stripeSettings = stripeSettings.Value;
        }
        
        public IActionResult Index()
        {
            var secretKey = _stripeSettings.SecretKey;
            var publishableKey = _stripeSettings.PublishableKey;
            var whSecret = _stripeSettings.WHSecret;

            
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Shop()
        {
            var products = await _productService.GetAllProducts();
            return View(products);
        }
        [Authorize]
        public async Task<IActionResult> Service()
        {
            var products = await _productService.GetAllServices();
            return View(products);
        }
        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            var product = await _productService.GetProductById(productId);
            return View(product);
        }
        [Authorize]
        public async Task<IActionResult> Summary(int productId, int count)
        {
            var product = await _productService.GetProductById(productId);
            ShoppingCartVM shoppingCart = new ShoppingCartVM()
            {
                Product = product,
                OrderHeader = new OrderHeader() { ProductId = productId, Quantity = count, OrderTotal = (double)(count * product.ProductPrice) }
            };

            return View(shoppingCart);
        }

        [Authorize]
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
        [Authorize]
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


            var domain = "https://localhost:7090/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                //SuccessUrl = domain + $"customer/cart/OrderConfirmation",
                SuccessUrl = $"{domain}Home/OrderConfirmation/{orderHeaderId}",
                CancelUrl = domain,
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
                Metadata = new Dictionary<string, string>
        {
            { "orderHeaderID", orderHeaderId.ToString() },


        },
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
        
        [HttpPost]
        [Route("/home/socalpayment")]
        public async Task<IActionResult> SoCalPayment()
        {
           
            

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _stripeSettings.WHSecret);
            try
            {


                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {

                    var session = stripeEvent.Data.Object as Session;


                    var service = new Stripe.Checkout.SessionService();
                    var fullSession = await service.GetAsync(session.Id);

                    string customerId = fullSession.CustomerId;


                    var orderHeaderId = int.Parse(session.Metadata["orderHeaderID"]);

                    var getOrderHeaderById = await _orderHeaderService.GetOrderHeaderById(orderHeaderId);



                    if (getOrderHeaderById != null)
                    {

                        getOrderHeaderById.PaymentIntentId = fullSession.PaymentIntentId;
                        getOrderHeaderById.OrderDate = DateTime.Now;
                        await _orderHeaderService.UpdateOrderHeader(getOrderHeaderById);
                    }

                }
                else if (stripeEvent.Type == EventTypes.ChargeUpdated)
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    var paymentIntentId = charge.PaymentIntentId;
             
                    var orderHeader = await _orderHeaderService.GetOrderHeaderByPaymentIntentId(paymentIntentId);
                    if (orderHeader != null)
                    {
                        // Update order header or perform any necessary actions
                        orderHeader.PaymentStatus = charge.Status;
                        await _orderHeaderService.UpdateOrderHeader(orderHeader);
                    }
                   
                }
            }


            catch (StripeException ex)
            {
                return BadRequest();
            }
            return Ok();
        }




    
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
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
