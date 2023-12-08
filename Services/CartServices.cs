using Microsoft.AspNetCore.Identity;
using WorldDominion.Models;
using Newtonsoft.Json;

namespace WorldDominion.Services
{
    public class CartService
    {
        //Access the current request that is passing through our application
        //We need to access the request so that we can access the session
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string _cartSessionKey = "Cart";
        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        //If we have a cart already on our session, it will return it an di fwe don't it will create a new one
        //No matter what, it will return a cart
        public Cart? GetCart()
        {
            //Anything stored in a session is a string and the following command will get back the value as a string 
            //If you want to store an object, we need to convert it to the object
            var cartJson = _httpContextAccessor.HttpContext.Session.GetString(_cartSessionKey);

            return cartJson == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(cartJson);
            //If cartJson is null then return a new cart else convert the string you found by calling DeserializeObject, DeserializeObject into a cart (cartJson is where you gonna pull it from); 
        }

        //Save the cart to the session key
        public void SaveCart(Cart cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);

            _httpContextAccessor.HttpContext.Session.SetString(_cartSessionKey, cartJson);
        }

        public void DestroyCart()
        {
            _httpContextAccessor.HttpContext.Session.Remove(_cartSessionKey);
        }
    }
}