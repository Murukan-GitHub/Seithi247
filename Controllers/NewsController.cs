using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Seithi247.Data;
using Seithi247.Models;

namespace Seithi247.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NewsController(ApplicationDbContext context) => _context =
        context;
        // GET: /News OR /

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var items = await _context.News.Include(i => i.Images).Include(i => i.NewsMedias).Include(c=>c.Comments)
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
            var news = await _context.News.Include(i => i.Images).Include(i => i.NewsMedias).Include(i => i.Comments)
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
        public async Task<IActionResult> LoadMore(int skip, int take = 10)
        {
            var news = await _context.News.Include(i => i.Images).Include(i => i.NewsMedias)
                .OrderByDescending(n => n.PublishedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return PartialView("_NewsCardsPartial", news);
        }
        [HttpPost]
        public IActionResult Like(int id)
        {
            var news = _context.News.Find(id);
            if (news == null) return NotFound();

            news.Likes += 1;
            _context.SaveChanges();

            return Json(new { success = true, likes = news.Likes });
        }

        [HttpPost]
        public IActionResult AddComment(int newsId, string authorName, string text,string reaction)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Json(new { success = false, message = "Comment cannot be empty." });

            var comment = new Comment
            {
                NewsId = newsId,
                AuthorName = string.IsNullOrWhiteSpace(authorName) ? "Anonymous" : authorName,
                Text = text,
                PostedDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return PartialView("_CommentsList", _context.Comments.Where(c => c.NewsId == newsId).OrderByDescending(c => c.PostedDate).ToList());
        }

        [HttpPost]
        public IActionResult ReactToComment(int commentId, string emoji)
        {
            var comment = _context.Comments.Include(c => c.Reactions).FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
                return NotFound();

            // Optional: Identify user by IP or GUID
            var userKey = Request.Cookies["uid"];
            if (string.IsNullOrEmpty(userKey))
            {
                userKey = Guid.NewGuid().ToString();
                Response.Cookies.Append("uid", userKey, new CookieOptions { Expires = DateTime.Now.AddYears(1) });
            }

            // Check if user already reacted with same emoji
            var existing = comment.Reactions.FirstOrDefault(r => r.UserIdentifier == userKey);
            if (existing != null)
            {
                // Toggle off if same emoji clicked again
                if (existing.Emoji == emoji)
                    _context.CommentReactions.Remove(existing);
                else
                    existing.Emoji = emoji;
            }
            else
            {
                _context.CommentReactions.Add(new CommentReaction
                {
                    CommentId = commentId,
                    Emoji = emoji,
                    UserIdentifier = userKey,
                    ReactedOn = DateTime.Now
                });
            }

            _context.SaveChanges();

            var reactionSummary = comment.Reactions
                .GroupBy(r => r.Emoji)
                .Select(g => new { emoji = g.Key, count = g.Count() })
                .ToList();

            return Json(reactionSummary);
        }

        // NEW: AJAX endpoint for category filtering
        [HttpGet]
        public async Task<IActionResult> FilterByCategory(string category)
        {
            var filtered =  _context.News.AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "All")
                filtered = filtered.Where(n => n.NewsCategory.ToString() == category);

            var list = await filtered.Include(i => i.Images).Include(i => i.NewsMedias).OrderByDescending(n => n.PublishedAt).ToListAsync();

            // Return a partial with just the cards
            return PartialView("_NewsCardsPartial", list);
        }
    }
}
