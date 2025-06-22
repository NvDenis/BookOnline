using Microsoft.EntityFrameworkCore;
using bookstore.Models; // hoặc namespace đúng với thư mục Models của bạn

namespace bookstore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<EmailFormModel> Contact { get; set; } // 👈 Thêm dòng này
    }
}
