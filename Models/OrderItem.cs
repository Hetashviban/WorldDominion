using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldDominion.Models
{
    public class OrderItem
    {
        [Key]
        public int Id {get; set;} = 0;

        //This needs to be tied with the order
        //One order to many order items
        [Required]
        public int OrderId {get; set;} = 0;

        //Snapshot data - products name, price and quantity
        [Required]
        public string ProductName {get; set;} = String.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price {get; set; } = 0.00M;

        [Required]
        public int Quantity {get; set;} = 0;

        //This will help us to pull with the actual order information
        [ForeignKey("OrderId")]
        public virtual Order? Order {get; set;}
    }
}