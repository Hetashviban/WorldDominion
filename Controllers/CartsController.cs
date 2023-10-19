using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;
using WorldDominion.Models;


//There is no point of doing async for in - memory stuff
namespace WorldDominion.Controllers
{
    public class CartsController : Controller
    {
        private readonly string _cartSessionKey;
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _cartSessionKey = "Cart";
            _context = context;
        }

        //This Async function displays the cart
        public async Task<IActionResult> Index()
        {
            //Get our cart (or get a new cart if it doesn't exist)
            var cart = GetCart();

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

                    if(product != null)
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
        public async Task<IActionResult> AddToCart (int productId, int quantity)
        {
            var cart = GetCart();

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

                cartItem = new CartItem {ProductId = productId, Quantity = quantity, Product = product};
                cart.CartItems.Add(cartItem);
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        //If we have a cart already on our session, it will return it an di fwe don't it will create a new one
        //No matter what, it will return a cart
        private Cart? GetCart()
        {
            //Anything stored in a session is a string and the following command will get back the value as a string 
            //If you want to store an object, we need to convert it to the object
            var cartJson = HttpContext.Session.GetString(_cartSessionKey);

            return cartJson == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(cartJson);
            //If cartJson is null then return a new cart else convert the string you found by calling DeserializeObject, DeserializeObject into a cart (cartJson is where you gonna pull it from); 
        }

        //Save the cart to the session key
        private void SaveCart(Cart cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);

            HttpContext.Session.SetString(_cartSessionKey, cartJson);
        }
    }
}