using System.Collections.Concurrent;
using System.Configuration;
using System.Security.Claims;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using WorldDominion.Models;
using WorldDominion.Services;

namespace WorldDominion.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CartService _cartService;
        private ApplicationDbContext _context;

        private readonly IConfiguration _configuration;

        public OrdersController(CartService cartService, ApplicationDbContext context, IConfiguration configuration)
        {
            _cartService = cartService;
            _context = context;
            _configuration = configuration;
        }

        [Authorize()]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null) return NotFound();

            var orders = await _context.Orders
                .Include(order => order.OrderItems)
                .Include(order => order.User)
                .Where(order => order.UserId == userId)
                .Where(order => order.PaymentReceived == true)
                .OrderByDescending(order => order.Id)
                .ToListAsync();

            return View(orders);
        }

        [Authorize()]
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null) return NotFound();

            var order = await _context.Orders
                .Include(order => order.OrderItems)
                .Include(order => order.User)
                .Where(order => order.UserId == userId)
                .FirstOrDefaultAsync(order => order.Id == id);

            return View(order);
        }

        [Authorize()] //has to be logged in but doesn't have to be role specific
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = _cartService.GetCart();

            if(cart == null)
            {
                return NotFound();
            }
 
            var order = new Order
            {
                UserId = userId,
                Total = cart.CartItems.Sum(CartItem => (decimal)(CartItem.Quantity * CartItem.Product.MSRP)),
                OrderItems = new List<OrderItem>()
            };

            //Snapshot
            foreach(var CartItem in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem {
                    OrderId = order.Id,
                    ProductName = CartItem.Product.Name,
                    Quantity = CartItem.Quantity,
                    Price = CartItem.Product.MSRP
                });
            }
            return View("Details", order);
        }

        [Authorize()]
        [HttpPost]
        public IActionResult ProcessPayment()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = _cartService.GetCart();

            if(userId == null || cart == null) return NotFound();

            var order = new Order
            {
                UserId = userId,
                Total = cart.CartItems.Sum(cartItem => cartItem.Quantity * cartItem.Product.MSRP),
                OrderItems = new List<OrderItem>()
            };

            //Set Stripe API key
            StripeConfiguration.ApiKey = _configuration.GetSection("Stripe")["SecretKey"];

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)order.Total * 100,
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "WorldDominion Purchase"
                            }
                        },
                        Quantity = 1
                    }
                },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Orders/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Carts/ViewMyCart"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize()]
        public async Task<IActionResult> SaveOrder()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = _cartService.GetCart();

            if (userId == null || cart == null) return NotFound();

            var order = new Order
            {
                UserId = userId,
                Total = cart.CartItems.Sum(cartItem => cartItem.Quantity * cartItem.Product.MSRP),
                OrderItems = new List<OrderItem>(),
                PaymentReceived = true
            };

            foreach (var cartItem in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductName = cartItem.Product.Name,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.MSRP
                });
            }

            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            _cartService.DestroyCart();

            return RedirectToAction("Details", new { id = order.Id });
        }
    }
}