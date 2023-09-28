using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldDominion.Models
{
 
    public enum ProductWeightUnit
    {
        GRAMS,
        KILOGRAMS, 
        POUNDS, 
        OUNCES,
        UNITS,
        LITERS,
    }

    //The name of the class should always begin with capital letter - that's the naming convention
    public class Product
    {
        [Key]
        public int Id{ get; set; }  =  0;

        [Required]
        [Display(Name = "Department")]  //This is the key that is going to connect with the department
        //We cannot use [key] because then it will create primary key combination
        //Therefore we are using required
        public int DepartmentId { get; set;} = 0;
        //We are basically rewriting the key of the department model but not using the key data annotation

        [Required, StringLength(300)]
        public string Name { get; set;} = String.Empty;

        [StringLength(1000)]
        public string? Description { get; set;} = String.Empty;

        [StringLength(1000)]
        //Why are we storing an Image as a string? It's because when we do images we are going to write the image to a directory and then 
        //we are going to write the path of that directory and the info about the image to the database in a field called image
        //This is restrictive as we are only allowed to have only one image 
        //If we want two images -- we would have to create a different kind of model for that 
        public string? Image { get; set;} = String.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        [DataType(DataType.Currency)] //This data annotation makes sure that we are storing as a currency 
        public decimal MSRP { get; set;} = 0.01M;//pre define as 1 cent
        //MSRP - Manufactured suggested retail price

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Weight { get; set;} = 0.01M;

        [Required]
        public ProductWeightUnit WeightUnit { get; set;} = ProductWeightUnit.UNITS; //We are presetting the unit/value here


        //This is going to be one to many relationship 
        //The one is going to be the department model (Parent)
        //The many will be the child would be the products
        //So the one to many relationship will be one department has many products or one product has one department 
        //We need to define this because when we go to create the migration it actually needs to know what that relationship is so that it can set up the foreign key constrains in the database 



        //We need to define the relationship of each other in both the models, doing it in one would'nt help 

        
        //Defining the relationship with department 
        //Defining the relationship in one model does'nt automatically make it defined in the other model 
        [ForeignKey("DepartmentId")] //In the double inverted commas, we are going to define which of the following is going to be the foreign key
        public virtual Department? Department { get; set;}//This line is a property in the class that will allow you to store department record directly inside this value
        //creates the association to departments 
        //allows a department to be stored in an instance of a project 
        //virtual is for items where we are defining a variable that is'nt necessarily a part of this model 
    }
}