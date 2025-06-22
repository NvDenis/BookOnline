using bookstore.Data;
using bookstore.Models;
// Quan trọng: thêm các using cho MailKit
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System.IO;
using System.Threading.Tasks;

namespace bookstore.Controllers
{
    public class ContactController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ContactController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Send() => View();

        [HttpPost]
        public async Task<IActionResult> Send(EmailFormModel model)
        {
            if (model.Attachment != null)
            {
                // Lưu file vào thư mục wwwroot/uploads
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder); // tạo nếu chưa có

                string fileName = Path.GetFileName(model.Attachment.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Attachment.CopyToAsync(stream);
                }

                model.AttachmentName = fileName;
            }

            model.SendDate = DateTime.Now;

            _context.Add(model);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Đã lưu thông tin liên hệ thành công!";
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

       
    }
}
