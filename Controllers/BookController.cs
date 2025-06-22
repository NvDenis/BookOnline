using bookstore.Data;
using Microsoft.AspNetCore.Mvc;

namespace bookstore.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            var book = _context.Books.FirstOrDefault(t => t._id == id);
            if (book == null) return NotFound();
            return View(book); // View ở Views/Book/Detail.cshtml
        }
    }
}
