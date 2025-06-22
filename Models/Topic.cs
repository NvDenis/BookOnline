using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Topic
    {
        [Key]
        public int _id { get; set; }
        public string name { get; set; }
    }
}
