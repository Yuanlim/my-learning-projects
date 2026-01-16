using System.ComponentModel.DataAnnotations;

namespace SchoolAppointmentApp.Entities;

public class Cart
{
    public int CartId { get; set; } // Pk
    public string CustomerId { get; set; } = null!; // Fk key of a Teacher
    public Teacher Teacher { get; set; } = null!;
    public ICollection<CartItem> CartProductList { get; set; } = []; // should be modifible and indexing
    public int TotalCost { get; set; }
    public bool Ordered { get; set; }
}

public class CartItem
{
    public int CartItemId { get; set; }
    public int CartId { get; set; } // Fk of cart
    public Cart Cart { get; set; } = null!;
    public int ProductId { get; set; } // Fk of Product
    public Product Product { get; set; } = null!; 
    [Range(1, int.MaxValue)] public int Quantity { get; set; } 
}
