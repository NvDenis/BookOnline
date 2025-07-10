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

        public IActionResult Detail(int id, string sortOrder, int? page)
        {
            var topic = _context.Topics.FirstOrDefault(t => t._id == id);
            if (topic == null) return NotFound();

            var books = _context.Books.Where(b => b.id_topic == id).ToList();

            ViewBag.Topics = _context.Topics.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();

            ViewBag.TopicName = topic.name;

            switch (sortOrder)
            {
                case "price_asc":
                    books = books.OrderBy(b => b.price).ToList();
                    break;
                case "price_desc":
                    books = books.OrderByDescending(b => b.price).ToList();
                    break;
                case "name_asc":
                    books = books.OrderBy(b => b.name).ToList();
                    break;
                case "name_desc":
                    books = books.OrderByDescending(b => b.name).ToList();
                    break;
                case "newest":
                    books = books.OrderByDescending(b => b._id).ToList(); // hoặc b.createdAt nếu có
                    break;
                case "oldest":
                    books = books.OrderBy(b => b._id).ToList();
                    break;
            }

            return View(books); // View ở Views/Category/Detail.cshtml
        }
    }
}
