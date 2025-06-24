using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Book
    {
        [Key]
        public int _id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal price_original { get; set; }
        // Bỏ [Required] để không bắt buộc khi sửa
        public string imgs { get; set; } = "[]"; // ✅ Gán mặc định mảng rỗng nếu cần
        public string description { get; set; }
        public int sold { get; set; }
        public int id_publisher { get; set; }
        public int id_topic { get; set; }
        public DateTime created_at { get; set; }
    }
}
