using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    [Table("bill_details")] // 👈 thêm dòng này
    public class BillDetail
    {
        [Key]
        public int _id { get; set; }
        public int id_bill { get; set; }
        public int id_book { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }


        // Navigation properties
        [ForeignKey("id_bill")]
        public Bill Bill { get; set; }

    }
}
