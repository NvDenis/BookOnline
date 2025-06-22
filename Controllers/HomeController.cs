using bookstore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;

namespace bookstore.Controllers
{

    public class HomeController : Controller
    {

        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page, string sortOrder)
        {
            int pageSize = 6; // Số sách mỗi trang
            int pageNumber = page ?? 1;
            var query = _context.Books.AsQueryable();

            // Sắp xếp theo lựa chọn
            switch (sortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(b => b.price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(b => b.price);
                    break;
                case "name_asc":
                    query = query.OrderBy(b => b.name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(b => b.name);
                    break;
                case "oldest":
                    query = query.OrderBy(b => b.created_at); // hoặc b.id nếu không có created_at
                    break;
                default: // "newest" hoặc không có gì
                    query = query.OrderByDescending(b => b.created_at);
                    break;
            }

            // lấy 6 quyển đầu tiên trong danh sách sách
            var books = query.ToPagedList(pageNumber, pageSize);
            ViewBag.Topics = _context.Topics.ToList(); // 👈 Gửi danh sách chủ đề
            ViewBag.Publishers = _context.Publishers.ToList(); // 👈 Gửi danh sách chủ đề
            return View(books);
        }
    }
}
