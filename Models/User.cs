using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class User
    {
        [Key]
        public int _id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string role { get; set; } // <--- thêm dòng này
    }

}
