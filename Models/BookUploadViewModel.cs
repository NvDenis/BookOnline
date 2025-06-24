using Microsoft.AspNetCore.Http;
using bookstore.Models;

public class BookUploadViewModel
{
    public Book Book { get; set; }
    // Sửa chỗ này: dùng List để cho phép upload nhiều ảnh
    public List<IFormFile> ImageFiles { get; set; }
}