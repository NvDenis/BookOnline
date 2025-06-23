using bookstore.Data;
using bookstore.Models;
using Microsoft.AspNetCore.Mvc;

[Route("admin/publisher")]
public class PublisherController : Controller
{
    private readonly AppDbContext _context;

    public PublisherController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var books = _context.Publishers.ToList();
        return View(books);
    }

    [HttpGet("edit/{id}")]
    public IActionResult Edit(int id)
    {
        Console.WriteLine($"Gọi POST Edit: {id}"); // Viết trong action C#, không viết trong view Razor
        var book = _context.Publishers.Find(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost("edit/{id}")]
    public IActionResult Edit(int id, Publisher model)
    {
       

        try
        {
            var book = _context.Publishers.Find(id);
            if (book == null)
            {
                Console.WriteLine("Book not found");
                return NotFound();
            }

            book.name = model.name;
            var changes = _context.SaveChanges();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database error: {ex.Message}");
            ModelState.AddModelError("", "Lỗi khi lưu dữ liệu");
            return View(model);
        }
    }

    [HttpGet("delete/{id}")]
    public IActionResult Delete(int id)
    {
        var hasBooks = _context.Books.Any(b => b.id_publisher == id);
        if (hasBooks)
        {
            TempData["ErrorMessage"] = "Không thể xoá vì nhà xuất bản này đang được sử dụng bởi một số sách.";
            return RedirectToAction("Index");
        }

        var book = _context.Publishers.Find(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost("delete/{id}")]
    public IActionResult ConfirmDelete(int id)
    {
        var book = _context.Publishers.Find(id);
        if (book == null) return NotFound();

        _context.Publishers.Remove(book);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // Thêm các action như: Create, Edit, Delete nếu cần
}
