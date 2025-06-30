// Controllers/CartController.cs
using bookstore.Data;
using bookstore.Models;
using bookstore.Models.Vnpay;
using bookstore.Services.Vnpay;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace bookstore.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IVnPayService _vnPayService;

        public CartController(AppDbContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }
        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success && response.VnPayResponseCode == "00")
            {
                // TODO: ở đây bạn có thể cập nhật status đơn hàng trong DB
                return RedirectToAction("OrderSuccess");
            }
            else
            {
                // thanh toán thất bại
                TempData["Message"] = "Thanh toán thất bại hoặc bị hủy!";
                return RedirectToAction("Index", "Cart");
            }
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


        public IActionResult OrderSuccess()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            var book = _context.Books.FirstOrDefault(b => b._id == id);
            if (book == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.BookId == id);
            if (item != null)
            {
                item.Quantity += quantity; // tăng số lượng
            }
            else
            {
                cart.Add(new CartItem
                {
                    BookId = id,
                    Name = book.name,
                    Img = book.imgs,
                    Price = book.price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult VnPayReturn()
        {
            var responseCode = Request.Query["vnp_ResponseCode"];
            var amount = Request.Query["vnp_Amount"];

            if (responseCode == "00")
            {
                // Thành công: bạn có thể xử lý đơn hàng ở đây
                // Hoặc hiển thị đơn hàng đã lưu ở chỗ khác
                return RedirectToAction("OrderSuccess");
            }

            TempData["Message"] = "Thanh toán thất bại hoặc bị hủy!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutPost(string name, string address, string paymentMethod)
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCart();
            if (cart == null || !cart.Any())
                return RedirectToAction("Index");
            // ✅ Kiểm tra tên và địa chỉ
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tên người nhận và địa chỉ.";
                return RedirectToAction("Checkout");
            }

            var total = cart.Sum(i => i.TotalPrice);

            var bill = new Bill
            {
                id_user = userId.Value,
                name = name,
                address = address,
                order_date = DateTime.Now,
                total_amount = total,
                payment_method = paymentMethod
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

            // Nếu chọn VNPAY thì chuyển hướng đến VNPay
            if (paymentMethod == "VNPAY")
            {
                var paymentModel = new PaymentInformationModel
                {
                    Name = "Duy",
                    Amount = (double)total, // Sử dụng tổng tiền thực tế từ giỏ hàng
                    OrderDescription = "Thanh toan don hang",
                    OrderType = "other",
                };

                var url = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
                return Redirect(url); // Thay đổi từ Content(url) sang Redirect(url)
            }

            // Nếu chọn COD
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



    }






}
