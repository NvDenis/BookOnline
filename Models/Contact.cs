using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    [Table("contact")] // Chỉ định tên bảng chính xác
    public class Contact
    {
        [Key]
        public int _id { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Phone { get; set; }

        public string Subject { get; set; }

        public string Note { get; set; }

        [Column("attachment_name")]
        public string AttachmentName { get; set; }

        [Column("send_date")]
        public DateTime SendDate { get; set; }
    }
}
