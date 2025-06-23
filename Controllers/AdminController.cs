using bookstore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("user_email");
            var name = HttpContext.Session.GetString("user_name");
            var role = HttpContext.Session.GetString("role");
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role) || role != "admin")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        //// Route: /admin
        //public IActionResult Index()
        //{
        //    var role = HttpContext.Session.GetString("role");
        //    var email = HttpContext.Session.GetString("user_email");

        //    if (!string.IsNullOrEmpty(email) && role == "admin")
        //    {
        //        return View("AdminHome"); // Giao diện admin
        //    }

        //    return RedirectToAction("Login");
        //}

        // GET: /admin/login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // GET: /admin/lien-he
        [HttpGet]
        public IActionResult Contact()
        {
            // Kiểm tra có đăng nhập chưa
            var role = HttpContext.Session.GetString("role");
            if (string.IsNullOrEmpty(role) || role != "admin")
            {
                return RedirectToAction("Index", "Home"); // hoặc "Login", "User"
            }
            var contacts = _context.Contacts.ToList(); // Giả sử DbSet tên là Contacts
            return View(contacts);
        }

        // POST: /admin/login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.email == email && u.password == password);

            if (user != null && user.role == "admin")
            {
                HttpContext.Session.SetString("user_email", user.email);
                HttpContext.Session.SetString("user_name", user.name);
                HttpContext.Session.SetString("role", user.role);
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Sai thông tin hoặc không có quyền admin";
            return View();
        }
    }
}
