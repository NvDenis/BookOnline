using bookstore.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Đăng ký AppDbContext với MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) // hoặc phiên bản MySQL bạn đang dùng
    )
);

builder.Services.AddControllersWithViews(); // Quan trọng: phải có dòng này để đăng ký MVC

// Add services to the container.
builder.Services.AddSession(); // 👈 Thêm dòng này trước khi build
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

builder.Services.AddSession(); // Add this
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "custom-news",
    pattern: "tin-tuc",
    defaults: new { controller = "News", action = "Index" });

app.MapControllerRoute(
    name: "gio-hang",
    pattern: "gio-hang",
    defaults: new { controller = "Cart", action = "Index" });

app.MapControllerRoute(
    name: "dat-hang",
    pattern: "dat-hang",
    defaults: new { controller = "Cart", action = "Checkout" });

app.MapControllerRoute(
    name: "login",
    pattern: "dang-nhap",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "contact",
    pattern: "lien-he",
    defaults: new { controller = "Contact", action = "Index" });

app.MapControllerRoute(
    name: "admin_contact",
    pattern: "admin/lien-he",
    defaults: new { controller = "Admin", action = "Contact" });

app.MapControllerRoute(
    name: "register",
    pattern: "dang-ky",
    defaults: new { controller = "Account", action = "Register" });

app.MapControllerRoute(
    name: "category-detail",
    pattern: "category-detail/{id}",
    defaults: new { controller = "Category", action = "Detail" });

app.MapControllerRoute(
    name: "book",
    pattern: "book/{id}",
    defaults: new { controller = "Book", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
