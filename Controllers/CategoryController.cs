using bookstore.Data;
using Microsoft.AspNetCore.Mvc;

namespace bookstore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Detail(int id)
        {
            var topic = _context.Topics.FirstOrDefault(t => t._id == id);
            if (topic == null) return NotFound();

            var books = _context.Books.Where(b => b.id_topic == id).ToList();

            ViewBag.TopicName = topic.name;
            return View(books); // View ở Views/Category/Detail.cshtml
        }
    }
}
