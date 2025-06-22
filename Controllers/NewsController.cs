using Microsoft.AspNetCore.Mvc;

namespace bookstore.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
