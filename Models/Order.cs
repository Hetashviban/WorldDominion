using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WorldDominion.Models
{
    public class Order
    {
        [Key]
        public int Id {get; set;} = 0;

        //This will refer to the user itself 
        [Required]
        public string UserId {get; set;} = String.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Total {get; set;} = 0.00M;

        [Required]
        public bool PaymentReceived {get; set;} = false;

        //This will store the user when we actually retrieve an order
        //So if we want to retrieve the user information we can
        // [ForeignKey("UserId")]
        public IdentityUser User {get; set; } = new IdentityUser();

        //This will store the snapshot of the products
        //This will help us to store all the order items that is associated with the order
        public virtual ICollection<OrderItem> OrderItems {get; set;} = new List<OrderItem>();
    }
}