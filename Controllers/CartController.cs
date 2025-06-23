// Controllers/CartController.cs
using bookstore.Data;
using bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace bookstore.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }
        private const string CART_KEY = "cart";

        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString(CART_KEY);
            if (sessionData != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(sessionData)!;
            }
            return new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var sessionData = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CART_KEY, sessionData);
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int bookId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.BookId == bookId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult AddToCart(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b._id == id); // hoặc inject _context

            if (book == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.BookId == id);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    BookId = id,
                    Name = book.name,
                    Img = book.imgs,
                    Price = book.price,
                    Quantity = 1
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.BookId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            SaveCart(new List<CartItem>());
            return RedirectToAction("Index");
        }


        public IActionResult PlaceOrder()
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCart();
            if (cart.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng đang trống!";
                return RedirectToAction("Index");
            }

            // Tính tổng tiền
            var total = cart.Sum(item => item.Price * item.Quantity);

            // Tạo đơn hàng (bill)
            var bill = new Bill
            {
                // Convert string to int, userId to number type
                id_user = userId.Value,
                order_date = DateTime.Now,
                total_amount = total
            };

            _context.Bills.Add(bill);
            _context.SaveChanges(); // Để lấy bill._id

            // Tạo chi tiết đơn hàng
            foreach (var item in cart)
            {
                var detail = new BillDetail
                {
                    id_bill = bill._id,
                    id_book = item.BookId,
                    quantity = item.Quantity,
                    price = item.Price
                };
                _context.BillDetails.Add(detail);
            }

            _context.SaveChanges();

            // Xóa giỏ hàng
            SaveCart(new List<CartItem>());

            return RedirectToAction("OrderSuccess");
        }


        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }

            // Kiểm tra đăng nhập
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutPost()
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }

            var total = cart.Sum(i => i.TotalPrice);

            var bill = new Bill
            {
                id_user = userId.Value,
                order_date = DateTime.Now,
                total_amount = total
            };

            _context.Bills.Add(bill);
            _context.SaveChanges();

            foreach (var item in cart)
            {
                var detail = new BillDetail
                {
                    id_bill = bill._id,
                    id_book = item.BookId,
                    quantity = item.Quantity,
                    price = item.Price
                };
                _context.BillDetails.Add(detail);
            }

            _context.SaveChanges();

            // Gửi email xác nhận
            var user = _context.Users.FirstOrDefault(u => u._id == userId.Value);
            if (user != null)
            {
                var emailService = new EmailSender();
                var subject = "Xác nhận đơn hàng của bạn";

                var html = $@"
        <h3>Xin chào {user.name},</h3>
        <p>Cảm ơn bạn đã đặt hàng vào ngày {bill.order_date:dd/MM/yyyy}.</p>
        <p><strong>Chi tiết đơn hàng:</strong></p>
        <table border='1' cellpadding='8' cellspacing='0'>
            <tr><th>Sách</th><th>Giá</th><th>Số lượng</th><th>Thành tiền</th></tr>";

                foreach (var item in cart)
                {
                    html += $"<tr><td>{item.Name}</td><td>{item.Price:N0} ₫</td><td>{item.Quantity}</td><td>{item.TotalPrice:N0} ₫</td></tr>";
                }

                html += $@"
        </table>
        <p><strong>Tổng cộng:</strong> {total:N0} ₫</p>
        <p>Chúng tôi sẽ giao hàng trong vòng 3-5 ngày tới.</p>
        <p>Trân trọng,</p>
        <p>Đội ngũ MoziBookstore</p>";

                await emailService.SendEmailAsync(user.email, subject, html);
            }

            SaveCart(new List<CartItem>());

            return RedirectToAction("OrderSuccess");
        }

        // POST: /dat-hang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(List<CartItem> model)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tính tổng
            var total = model.Sum(i => i.TotalPrice);

            // Tạo đơn hàng
            var bill = new Bill
            {
                id_user = userId.Value,
                order_date = DateTime.Now,
                total_amount = total
            };

            _context.Bills.Add(bill);
            _context.SaveChanges();

            // Lưu chi tiết từng sản phẩm
            foreach (var item in model)
            {
                var detail = new BillDetail
                {
                    id_bill = bill._id,
                    id_book = item.BookId,
                    quantity = item.Quantity,
                    price = item.Price
                };
                _context.BillDetails.Add(detail);
            }
            _context.SaveChanges();

            // Xoá giỏ hàng
            HttpContext.Session.Remove("cart");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
