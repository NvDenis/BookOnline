using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    public class EmailFormModel
    {
        [Key]
        [Column("_id")]
        public int Id { get; set; }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }

        [Column("attachment_name")]
        public string AttachmentName { get; set; }

        [Column("send_date")]
        public DateTime SendDate { get; set; }

        [NotMapped] // không lưu vào DB, chỉ để nhận file từ form
        public IFormFile Attachment { get; set; }
    }
}
