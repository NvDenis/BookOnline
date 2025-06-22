// Models/CartItem.cs
namespace bookstore.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Price * Quantity;
    }
}