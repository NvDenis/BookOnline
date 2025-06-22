using bookstore.Data;
using bookstore.Models;
using Microsoft.AspNetCore.Mvc;

namespace bookstore.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.email == model.Email && u.password == model.Password);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("user_id", user._id);
                    HttpContext.Session.SetString("user_name", user.name);
                    HttpContext.Session.SetString("user_email", user.email);
                    ViewBag.SuccessMessage = "Đăng nhập thành công!";
                    ModelState.Clear(); // Xóa form
                    // Trả về trang chủ sau khi đăng nhập thành công
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Sai Email hoặc mật khẩu");
            }
            return View(model);
        }


        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.email == model.Email);
                if (existingUser == null)
                {
                    var user = new User
                    {
                        name = model.Username,
                        password = model.Password,
                        email = model.Email,
                        address = model.Address,
                        role = "user" // Mặc định là user, có thể thay đổi sau này
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    // Nếu bạn muốn ở lại trang Register để hiện thông báo:
                    ViewBag.SuccessMessage = "Đăng ký thành công!";

                    // Clear model
                    ModelState.Clear();
                    return View();
                }

                ModelState.AddModelError("", "Email đã tồn tại");
            }
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user_id");
            HttpContext.Session.Remove("user_name");
            HttpContext.Session.Remove("user_email");
            HttpContext.Session.Remove("role");
            return RedirectToAction("Login");
        }
    }
}
