using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Bill
    {
        [Key]
        public int _id { get; set; }
        public int id_user { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string payment_method { get; set; } = "COD";
        public string shipping_status { get; set; } = "chờ xác nhận";
        public string payment_status { get; set; } = "chưa thanh toán";
        public DateTime order_date { get; set; } = DateTime.Now;
        public decimal total_amount { get; set; }


        public List<BillDetail> BillDetails { get; set; } = new();
    }
}
