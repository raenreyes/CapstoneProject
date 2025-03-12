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
using System.Security.Claims;
using Stripe.Issuing;

namespace CapstoneProject.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly StripeSettings _stripeSettings;

        public HomeController(ILogger<HomeController> logger, IOptions<StripeSettings> stripeSettings, IProductService productService, IOrderHeaderService orderHeaderService
                                , IShoppingCartService shoppingCartService, IOrderDetailService orderDetailService)
        {
            _logger = logger;
            _productService = productService;
            _orderHeaderService = orderHeaderService;
            _stripeSettings = stripeSettings.Value;
            _shoppingCartService = shoppingCartService;
            _orderDetailService = orderDetailService;
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
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int productId, int count)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCart(userId, productId);

            if (shoppingCart != null)
            {
                shoppingCart.Count += count;
                await _shoppingCartService.UpdateShoppingCart(shoppingCart);
            }
            else
            {
                var newCart = new ShoppingCart
                {
                    ProductId = productId,
                    Count = count,
                    IdentityUserId = userId
                };
                await _shoppingCartService.AddShoppingCart(newCart);
            }

            return RedirectToAction(nameof(Cart));
        }


        public async Task<IActionResult> Remove(int cartId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCartByCartId(userId, cartId);
            if (shoppingCart != null)
            {
                await _shoppingCartService.DeleteShoppingCartById(shoppingCart);
            }


            return RedirectToAction(nameof(Cart));
        }

        //[Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Summary(ShoppingCartVM shoppingCart)
        //{
        //    var errors = ModelState.Values.SelectMany(v => v.Errors);
        //    foreach (var error in errors)
        //    {
        //        Console.WriteLine(error.ErrorMessage);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        await _orderHeaderService.SaveOrderHeader(shoppingCart.OrderHeader);
        //        return RedirectToAction("StripePost", "Home", new { orderHeaderId = shoppingCart.OrderHeader.Id });
        //    }

        //    return View(shoppingCart);
        //}
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> StripePost(int orderHeaderId)
        //{
        //    return await StripePostInternal(orderHeaderId);
        //}
        [Authorize]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShopCartVM cartfromDb = new()
            {
                ShoppingCartList = await _shoppingCartService.GetAllShoppingCarts(userId),
                OrderHeader = new() { IdentityUserId = userId }

            };
            foreach (var cart in cartfromDb.ShoppingCartList)
            {
                cartfromDb.OrderHeader.OrderTotal += ((double)cart.Product.ProductPrice * cart.Count);
            }
            return View(cartfromDb);
        }
        //this is where all the stripe checkout logic will go
        [HttpPost]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPOST(ShopCartVM shopCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            //shopCartVM.OrderHeader.OrderDate = DateTime.Now;
            await _orderHeaderService.SaveOrderHeader(shopCartVM.OrderHeader);
            // Retrieve the shopping cart list
            var shoppingCartList = await _shoppingCartService.GetAllShoppingCarts(userId);

            foreach (var cart in shoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shopCartVM.OrderHeader.Id,
                    Price = (double)cart.Product.ProductPrice * cart.Count,
                    Count = cart.Count
                };
                await _orderDetailService.AddOrderDetail(orderDetail);
            }

            var domain = "https://localhost:7090/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = $"{domain}Home/OrderConfirmation",
                CancelUrl = domain,
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
                Metadata = new Dictionary<string, string>
        {
            { "orderHeaderID", shopCartVM.OrderHeader.Id.ToString() }

            }
            };

            foreach (var item in shoppingCartList)
            {
                var sessionLineItems = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.ProductPrice * 100),//$20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ProductName,
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItems);
            }

            var service = new Stripe.Checkout.SessionService();
            Session session = service.Create(options);

            // Save the OrderHeader to the database


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
        public async Task<IActionResult> OrderConfirmation()
        {
            //var orderHeader = await _orderHeaderService.GetOrderHeaderById(id);
            //if (orderHeader == null)
            //{
            //    return NotFound();
            //}
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var shoppingCartList = await _shoppingCartService.GetAllShoppingCarts(userId);
            if (shoppingCartList == null)
            {
                shoppingCartList = new List<ShoppingCart>();
            }

            ShopCartVM shoppingCart = new ShopCartVM()
            {
                ShoppingCartList = shoppingCartList,
                OrderHeader = new OrderHeader()
            };

            foreach (var cart in shoppingCart.ShoppingCartList)
            {
                shoppingCart.OrderHeader.OrderTotal += ((double)cart.Product.ProductPrice * cart.Count);
            }
            return View(shoppingCart);
        }

    }
}
