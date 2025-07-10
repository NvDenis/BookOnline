using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
