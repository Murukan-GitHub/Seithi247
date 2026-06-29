using Seithi247.Services;
using Seithi247.Views.ViewModel;
using Seithi247.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Seithi247.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommentService _commentService;

        public NewsController(ApplicationDbContext context, ICommentService commentService)
        {
            _context = context;
            _commentService = commentService;
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var news = await _context.News
                .Include(n => n.Images)
                .Include(n => n.NewsMedias)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (news == null)
                return NotFound();

            // Load comments with reactions
            var comments = await _commentService.GetCommentsForNewsAsync(id.Value);

            // Create ViewModel
            var newsVM = new NewsVM
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                Author = news.Author,
                PublishedAt = news.PublishedAt,
                Likes = news.Likes,
                ShareCount = news.ShareCount ?? 0,
                FeatureImage = news.FeatureImage,
                Images = news.Images.ToList(),
                NewsMedias = news.NewsMedias.ToList(),
                Comments = comments
            };

            return View(newsVM);
        }

        // POST: News/AddComment
        [HttpPost]
        public async Task<IActionResult> AddComment(int newsId, string authorName, string text)
        {
            if (string.IsNullOrWhiteSpace(authorName) || string.IsNullOrWhiteSpace(text))
                return BadRequest(new { success = false, message = "Invalid input" });

            try
            {
                var comment = await _commentService.AddCommentAsync(newsId, authorName, text);

                // Return partial view or JSON for AJAX
                return Json(new 
                { 
                    success = true, 
                    commentId = comment.Id,
                    authorName = comment.AuthorName,
                    text = comment.Text,
                    postedDate = comment.PostedDate
                });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Error adding comment" });
            }
        }

        // POST: News/ReactToComment
        [HttpPost]
        public async Task<IActionResult> ReactToComment(int commentId, string emoji)
        {
            try
            {
                var userIdentifier = HttpContext.Connection.RemoteIpAddress?.ToString();

                await _commentService.AddReactionAsync(commentId, userIdentifier, emoji);
                var summary = await _commentService.GetReactionSummaryAsync(commentId);

                return Json(new { success = true, reactions = summary });
            }
            catch
            {
                return StatusCode(500, new { success = false });
            }
        }

        // POST: News/Like
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
                return NotFound();

            news.Likes++;
            await _context.SaveChangesAsync();

            return Json(new { success = true, likes = news.Likes });
        }
    }
}
