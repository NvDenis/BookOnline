using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Bill
    {
        [Key]
        public int _id { get; set; }
        public int id_user { get; set; }
        public DateTime order_date { get; set; } = DateTime.Now;
        public decimal total_amount { get; set; }

        public List<BillDetail> BillDetails { get; set; } = new();
    }
}
