using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Seithi247.Data;

namespace Seithi247.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NewsController(ApplicationDbContext context) => _context =
        context;
        // GET: /News OR /

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var items = await _context.News.Include(i => i.Images).Include(i => i.NewsMedias)
            .Where(n => n.IsPublished)
            .OrderByDescending(n => n.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return View(items);
        }
        // GET: /News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News.Include(i => i.Images)
                            .FirstOrDefaultAsync(o => o.Id == id);
            if (news == null || !news.IsPublished) return NotFound();
            return View(news);
        }
        public async Task<IActionResult> NewsListPartial()
        {
            var news = await _context.News.Include(i => i.Images).Include(i => i.NewsMedias)
           .Where(n => n.IsPublished)
           .OrderByDescending(n => n.PublishedAt)
           .Take(20)
           .ToListAsync();

            return PartialView("_NewsListPartial", news);
        }

        [HttpGet]
        public IActionResult LatestTimestamp()
        {
            var latest = _context.News
                .OrderByDescending(n => n.PublishedAt)
                .Select(n => n.PublishedAt)
                .FirstOrDefault();

            return Json(new { latest });
        }
        [HttpGet]
        public async Task<IActionResult> LoadMore(int skip, int take = 6)
        {
            var news = await _context.News.Include(i => i.Images).Include(i => i.NewsMedias)
                .OrderByDescending(n => n.PublishedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return PartialView("_NewsCardsPartial", news);
        }
    }
}
