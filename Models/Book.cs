using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Book
    {
        [Key]
        public int _id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public string img { get; set; }
        public string description { get; set; }
        public int sold { get; set; }
        public int id_publisher { get; set; }
        public int id_topic { get; set; }
        public DateTime created_at { get; set; }
    }
}
