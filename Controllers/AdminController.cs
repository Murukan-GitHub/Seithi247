using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seithi247.Data;
using Seithi247.Models;

namespace Seithi247.Controllers
{
    //[Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/News
        public async Task<IActionResult> Index()
        {
            var items = await _context.News.OrderByDescending(n =>
            n.PublishedAt).ToListAsync();
            return View(items);
        }
        // GET: Admin/News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }
        // GET: Admin/News/Create
        public IActionResult Create() => View();

        // POST: Admin/News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Create([Bind("Title,Summary,Content,Author,IsPublished,PublishedAt,VideoUrl,NewsType")] News news, List<IFormFile> ImageFiles, List<IFormFile> ThumbNailImages)
        {
            if (ModelState.IsValid)
            {
                if (news.Content == null) { news.Content = " "; }

                var sanitizer = new HtmlSanitizer();
                news.Content = sanitizer.Sanitize(news.Content);

                news.Content=news.Content.Replace("../uploads/", "/uploads/");
                _context.News.Add(news);
                await _context.SaveChangesAsync();

                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    foreach (var file in ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var newsImage = new NewsImage
                            {
                                FilePath = "/uploads/" + uniqueFileName,
                                NewsId = news.Id
                            };
                            _context.NewsImages.Add(newsImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                if (ThumbNailImages != null && ThumbNailImages.Count > 0)
                {
                    var VideoUrl = news.VideoUrl.Split(",").ToList();
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    var i = 0;
                    foreach (var file in ThumbNailImages)
                    {
                        if (file.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var newsMedia = new NewsMedia
                            {
                                IsVideo=true,
                                MediaUrl= VideoUrl[0],
                                ThumbnailUrl = "/uploads/" + uniqueFileName,
                                NewsId = news.Id
                            };
                            _context.NewsMedias.Add(newsMedia);
                        }
                        i++;
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var r = new { location = "/uploads/" + fileName };
                // ✅ TinyMCE expects JSON { location: "url" }
                return Json(new { location = "/uploads/" + fileName });
            }

            return Json(new { location = "" });
        }
        // GET: Admin/News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }
        // POST: Admin/News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
        [Bind("Id,Title,Summary,Content,Author,IsPublished,PublishedAt")] News news)
        {
            if (id != news.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }
        // GET: Admin/News/Delete/5
        public async Task<IActionResult> Delete(int? id)

        {
            if (id == null) return NotFound();
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }
        // POST: Admin/News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        private bool NewsExists(int id) => _context.News.Any(e => e.Id == id);
    }
}
