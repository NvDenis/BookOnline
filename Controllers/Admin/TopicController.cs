using bookstore.Data;
using bookstore.Models;
using Microsoft.AspNetCore.Mvc;

[Route("admin/topic")]
public class TopicController : Controller
{
    private readonly AppDbContext _context;

    public TopicController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var books = _context.Topics.ToList();
        return View(books);
    }

    [HttpGet("edit/{id}")]
    public IActionResult Edit(int id)
    {
        Console.WriteLine($"Gọi POST Edit: {id}"); // Viết trong action C#, không viết trong view Razor
        var book = _context.Topics.Find(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost("edit/{id}")]
    public IActionResult Edit(int id, Topic model)
    {

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState errors:");
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                }
            }
            return View(model);
        }

        try
        {
            var book = _context.Topics.Find(id);
            if (book == null)
            {
                Console.WriteLine("Book not found");
                return NotFound();
            }

            book.name = model.name;

            var changes = _context.SaveChanges();
            Console.WriteLine($"Saved changes: {changes} row(s) affected");

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
        var book = _context.Topics.Find(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost("delete/{id}")]
    public IActionResult ConfirmDelete(int id)
    {
        var book = _context.Topics.Find(id);
        if (book == null) return NotFound();

        _context.Topics.Remove(book);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // Thêm các action như: Create, Edit, Delete nếu cần
}
