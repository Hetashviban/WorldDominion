//The cart model will hold the cart Items
namespace WorldDominion.Models
{
    public class Cart
    {
        public List<CartItem> CartItems {get; set;} = new List<CartItem>();
        //This is holding the list of cart items
   }
}