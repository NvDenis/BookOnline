using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Publisher
    {
        [Key]
        public int _id { get; set; }
        public string name { get; set; }
    }
}
