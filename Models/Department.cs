using System.ComponentModel.DataAnnotations;
//This is going to attach the data annotation library for us -- helps us to create validation/constraints

namespace WorldDominion.Models

{
    public class Department //So the model department would be like a table in SQL called department
    {
        //Define the properties that need to be available 
        //We can think of the properties as columns in the database
        //All the properties we define for the database will be public

        [Key]//Decorators syntax (Data annotation) - specifies that it is a primary key 
        //Only one primary key 
        //If we declare two primary key that means that we are using primary key combination which means that those two values together will be unique
        //Key is automatically required and when it is created it will be automatically incremented and the primary key 
        public int Id{ get; set; }  =  0;

        [Required, StringLength(300)] //Making this field required
        //String Length is the maximum number of characters allowed to enter in the field 
        public string Name { get; set;} = String.Empty; //default/ preset as empty field

        [StringLength(1000)]
        public string? Description { get; set;} = String.Empty; //We are going to preset it as empty string
        //Best practice to preset an optional field
        //The above field is optional 
        //The question marks indicates that this property does'nt have to be filled, it's allowed to be empty 




        //relationship with Products and place to store products in the department instance
        public virtual ICollection<Product> Products { get; set;} = new List<Product>();//This will state that the product table is a child and will allow us to retrieve a department from the database and automatically load the department record/instance object up with products.
    }
}