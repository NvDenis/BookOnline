using Microsoft.AspNetCore.Mvc;
using bookstore.Data;
using System.Linq;

namespace bookstore.Components
{
    public class TopicMenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public TopicMenuViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var topics = _context.Topics.ToList();
            return View(topics);
        }
    }
}
