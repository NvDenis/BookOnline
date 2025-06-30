using bookstore.Data;
using Microsoft.AspNetCore.Mvc;

[Route("admin/order")]
public class BillController : Controller
{
    private readonly AppDbContext _context;

    public BillController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var books = _context.Bills.ToList();
        return View(books);
    }

    [HttpPost("update-status")] // Thêm route cụ thể
    public IActionResult UpdateBillStatus([FromForm] int id, [FromForm] string field, [FromForm] string value)
    {
        try
        {
            var bill = _context.Bills.FirstOrDefault(b => b._id == id);
            if (bill == null)
            {
                return NotFound();
            }

            // Validate input
            if (field == "shipping_status")
            {
                if (!new[] { "chờ xác nhận", "đang giao", "đã giao" }.Contains(value))
                {
                    return BadRequest("Trạng thái không hợp lệ");
                }
                bill.shipping_status = value;
            }
            else if (field == "payment_status")
            {
                if (!new[] { "chưa thanh toán", "đã thanh toán" }.Contains(value))
                {
                    return BadRequest("Trạng thái thanh toán không hợp lệ");
                }
                bill.payment_status = value;
            }
            else
            {
                return BadRequest("Trường không hợp lệ");
            }

            _context.SaveChanges();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


}
