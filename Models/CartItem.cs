namespace WorldDominion.Models
{
    public class CartItem
    {
        public int ProductId { get; set;}
        public int Quantity { get; set;}
        public Product Product {get; set;} = new Product();
    }
}