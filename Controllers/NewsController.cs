using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Seithi247.Data;
using Seithi247.Models;
using Seithi247.Views.ViewModel;
using System.Collections.Generic;

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
            var items = await _context.News
                                        .AsNoTracking()
                                        .Include(m => m.NewsMedias)
                                        .Include(i => i.Images)
                                        .Include(c => c.Comments)
                                        .Where(n => n.IsPublished)
                                        .OrderByDescending(n => n.PublishedAt)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            List<NewsVM> newsList = MapToNewsVM(items);

            return View(newsList);
        }
        // GET: /News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News
                                     .AsNoTracking()
                                     .Include(m => m.NewsMedias)
                                     .Include(i => i.Images)
                                     .Include(c => c.Comments)
                                     .FirstOrDefaultAsync(o => o.Id == id);

            if (news == null || !news.IsPublished) return NotFound();

            var newsVM = new NewsVM
            {
                Id = news.Id,
                NewsCategory = news.NewsCategory,
                Content = news.Content,
                Author = news.Author,
                Country = news.Country,
                Summary = news.Summary,
                Likes = news.Likes,
                Images = news.Images,
                Comments = news.Comments,
                NewsMedias = news.NewsMedias,
                IsPublished = news.IsPublished,
                PublishedAt = news.PublishedAt
            };

            return View(newsVM);
        }
        public async Task<IActionResult> NewsListPartial()
        {
            var news = await _context.News
                           .AsNoTracking()
                           .Include(m => m.NewsMedias)
                           .Include(i => i.Images)
                           .Include(c => c.Comments)
                           .Where(n => n.IsPublished)
                           .OrderByDescending(n => n.PublishedAt)
                           .Take(20)
                           .ToListAsync();

            List<NewsVM> newsList = MapToNewsVM(news);

            return PartialView("_NewsListPartial", newsList);
        }

        [HttpGet]
        public IActionResult LatestTimestamp()
        {
            var latest = _context.News
                .AsNoTracking()
                .OrderByDescending(n => n.PublishedAt)
                .Select(n => n.PublishedAt)
                .FirstOrDefault();

            return Json(new { latest });
        }
        [HttpGet]
        public async Task<IActionResult> LoadMore(int skip, int take = 20)
        {
            var news = await _context.News
                            .AsNoTracking()
                            .Include(m => m.NewsMedias)
                            .Include(i => i.Images)
                            .Include(c => c.Comments)
                            .OrderByDescending(n => n.PublishedAt)
                            .Skip(skip)
                            .Take(take)
                            .ToListAsync();

            List<NewsVM> newsList = MapToNewsVM(news);

            return PartialView("_NewsCardsPartial", newsList);
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
        public IActionResult AddComment(int newsId, string authorName, string text, string reaction)
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
        public async Task<IActionResult> FilterByCategory(string category, int skip, int take = 20)
        {
            //var query = _context.News.AsQueryable();
            List<News> list = null;
            if (!string.IsNullOrEmpty(category) && category == "Home")
            {
                list = await _context.News
                                .Include(m => m.NewsMedias)
                                .Include(i => i.Images)
                                .Include(c => c.Comments)
                                .AsNoTracking()
                                .OrderByDescending(n => n.PublishedAt)
                                .Skip(skip)
                                .Take(take)
                                .ToListAsync();
            }
            else
            {
                //var total = query.Count();
                var categoryEnum = Enum.Parse<NewsCategory>(category);
                list = await _context.News
                                   .Include(m => m.NewsMedias)
                                   .Include(i => i.Images)
                                   .Include(c => c.Comments)
                                   .AsNoTracking()
                                   .Where(n => n.NewsCategory == categoryEnum)
                                   .OrderByDescending(n => n.PublishedAt)
                                   .Skip(skip)
                                   .Take(take)
                                   .ToListAsync();
            }
            List<NewsVM> newsList = MapToNewsVM(list);

            return PartialView("_NewsListPartial", newsList);
        }


        public async Task<ActionResult> DetailsPartial(int id)
        {

            var news = await _context.News
                                     .AsNoTracking()
                                     .Include(m => m.NewsMedias)
                                     .Include(i => i.Images)
                                     .Include(c => c.Comments)
                                     .FirstOrDefaultAsync(o => o.Id == id);

            if (news == null || !news.IsPublished) return NotFound();

            var newsVM = new NewsVM
            {
                Id = news.Id,
                NewsCategory = news.NewsCategory,
                Content = news.Content,
                Author = news.Author,
                Country = news.Country,
                Summary = news.Summary,
                Likes = news.Likes,
                Images = news.Images,
                Comments = news.Comments,
                NewsMedias = news.NewsMedias,
                IsPublished = news.IsPublished,
                PublishedAt = news.PublishedAt
            };

            return PartialView("_DetailsPartial", newsVM);
        }
        private static List<NewsVM> MapToNewsVM(List<News> list)
        {
            return list
                .Select(i => new NewsVM
                {
                    Id = i.Id,
                    NewsCategory = i.NewsCategory,
                    Content = i.Content,
                    Author = i.Author,
                    Country = i.Country,
                    Summary = i.Summary,
                    Likes = i.Likes,
                    Images = i.Images,
                    Comments = i.Comments,
                    NewsMedias = i.NewsMedias,
                    IsPublished = i.IsPublished,
                    PublishedAt = i.PublishedAt,
                    NewsType = i.NewsType
                })
                .ToList();
        }
    }
}

