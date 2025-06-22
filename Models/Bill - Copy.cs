using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class HocVien
    {
        public string MaHocVien { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public decimal HocPhi { get; set; }
        public string Hinh { get; set; }
        public string GhiChu { get; set; }
    }
}
