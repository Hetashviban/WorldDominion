using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using WorldDominion.Models;
using WorldDominion.Services;


//There is no point of doing async for in - memory stuff
namespace WorldDominion.Controllers
{
    public class CartsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public CartsController(CartService cartService, ApplicationDbContext context)
        {
            _context = context;
            _cartService = cartService;
        }

        //This Async function displays the cart
        public async Task<IActionResult> Index()
        {
            //Get our cart (or get a new cart if it doesn't exist)
            var cart = _cartService.GetCart();

            if (cart == null)
            {
                return NotFound();
            }

            //If the cart exists, lets fill it with our products
            if (cart.CartItems.Count > 0)
            {
                foreach (var cartItem in cart.CartItems)
                {
                    /*
                        SELECT * FROM Products
                        JOIN Departments ON Products.DepartmentId = Departments.Id
                        WHERE Products.Id = 1
                    */
                    //Query builder
                    var product = await _context.Products
                    .Include(p => p.Department)
                    .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

                    if (product != null)
                    {
                        cartItem.Product = product;
                    }
                }
            }
            return View(cart);
        }


        //This Async function actually adds the items into the cart 
        //To access this we need to send a http request
        [HttpPost] //This is a http declarator and it tells the action which type of method to be used to access it.
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var cart = _cartService.GetCart();

            if (cart == null)
            {
                return NotFound();
            }

            var cartItem = cart.CartItems.Find(cartItem => cartItem.ProductId == productId);
            if (cartItem != null && cartItem.Product != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    return NotFound();
                }

                cartItem = new CartItem { ProductId = productId, Quantity = quantity, Product = product };
                cart.CartItems.Add(cartItem);
            }

            _cartService.SaveCart(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = _cartService.GetCart();

            if(cart == null)
            {
                return NotFound();
            }

            var cartItem = cart.CartItems.Find(cartItem => cartItem.ProductId == productId);
            
            if(cartItem != null)
            {
                cart.CartItems.Remove(cartItem);

                _cartService.SaveCart(cart);
            }

            return RedirectToAction("Index");
        }
    }
}