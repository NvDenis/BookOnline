using bookstore.Data;
using bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json; // nhớ thêm namespace

[Route("admin/book")]
public class BookController : Controller
{
    private readonly AppDbContext _context;

    public BookController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var books = _context.Books.ToList();
        return View(books);
    }

    [HttpGet("add")]
    public IActionResult Add()
    {
        ViewBag.Publishers = _context.Publishers
        .Select(p => new SelectListItem { Value = p._id.ToString(), Text = p.name })
        .ToList();

        ViewBag.Topics = _context.Topics
            .Select(t => new SelectListItem { Value = t._id.ToString(), Text = t.name })
            .ToList();

        var model = new BookUploadViewModel
        {
            Book = new Book()
        };

        return View(model); // ✅ đúng kiểu
    }

    [HttpPost("add")]
    public IActionResult Add(BookUploadViewModel model)
    {
        // Thêm sách mới
           
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


            List<string> imgPaths = new List<string>();

            // Xử lý nhiều file upload
            if (model.ImageFiles != null && model.ImageFiles.Count > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                foreach (var file in model.ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        imgPaths.Add("/uploads/" + fileName); // lưu đường dẫn web
                    }
                }
            }

            var imgsJson = System.Text.Json.JsonSerializer.Serialize(imgPaths);


            var book = new Book
            {
                name = model.Book.name,
                price = model.Book.price,
                price_original = model.Book.price_original,
                description = model.Book.description,
                id_publisher = model.Book.id_publisher,
                id_topic = model.Book.id_topic,
                imgs = imgsJson
            };
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database error: {ex.Message}");
            ModelState.AddModelError("", "Lỗi khi lưu dữ liệu");
            return View(model);
        }
    }

    [HttpGet("edit/{id}")]
    public IActionResult Edit(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null) return NotFound();

        ViewBag.Publishers = _context.Publishers
       .Select(p => new SelectListItem { Value = p._id.ToString(), Text = p.name })
       .ToList();

        ViewBag.Topics = _context.Topics
            .Select(t => new SelectListItem { Value = t._id.ToString(), Text = t.name })
            .ToList();

        var model = new BookUploadViewModel
        {
            Book = book,

        };

        return View(model); // ✅ đúng kiểu
    }

    [HttpPost("edit/{id}")]
    public IActionResult Edit(int id, BookUploadViewModel model)
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
            List<string> imgPaths = new List<string>();

            // Xử lý nhiều file upload
            if (model.ImageFiles != null && model.ImageFiles.Count > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                foreach (var file in model.ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        imgPaths.Add("/uploads/" + fileName); // lưu đường dẫn web
                    }
                }
            }

            var imgsJson = System.Text.Json.JsonSerializer.Serialize(imgPaths);

            var book = _context.Books.Find(id);
            if (book == null)
            {
                Console.WriteLine("Book not found");
                return NotFound();
            }

            book.name = model.Book.name;
            book.price = model.Book.price;
            book.price_original = model.Book.price_original;
            book.description = model.Book.description;
            book.id_publisher = model.Book.id_publisher;
            book.id_topic = model.Book.id_topic;
            book.imgs = imgsJson; // Cập nhật ảnh mới

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
        var book = _context.Books.Find(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost("delete/{id}")]
    public IActionResult ConfirmDelete(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null) return NotFound();

        _context.Books.Remove(book);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // Thêm các action như: Create, Edit, Delete nếu cần
}
