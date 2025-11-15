using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPostsController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(string search, int page = 1)
        {
            const int pageSize = 10;
            ViewBag.Title = "Quản lý bài viết";
            ViewBag.Search = search;

            var posts = _context.Posts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                posts = posts.Where(p =>
                    p.Title.Contains(keyword) ||
                    (p.Author ?? "").Contains(keyword) ||
                    (p.Slug ?? "").Contains(keyword));
            }

            var pagedPosts = posts
                .OrderByDescending(p => p.PublishedDate)
                .ToPagedList(page, pageSize);

            return View(pagedPosts);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Title = "Tạo bài viết";
            return View(new Post { PublishedDate = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post model)
        {
            ModelState.Remove("Slug");

            model.PublishedDate = model.PublishedDate == default(DateTime)
                ? DateTime.UtcNow
                : model.PublishedDate;

            var baseSlug = GenerateSlug(model.Title);
            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                ModelState.AddModelError("Title", "Không thể tạo slug hợp lệ từ tiêu đề. Vui lòng kiểm tra lại.");
            }
            else
            {
                model.Slug = EnsureUniqueSlug(baseSlug);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Tạo bài viết";
                return View(model);
            }

            _context.Posts.Add(model);
            _context.SaveChanges();

            TempData["PostCreated"] = "Bài viết đã được tạo thành công.";
            return RedirectToAction("Index", "Admin");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;

            // Convert to lowercase, remove accents, replace spaces with hyphens
            string slug = title.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
                .Replace("ă", "a").Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
                .Replace("â", "a").Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
                .Replace("đ", "d")
                .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
                .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
                .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
                .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
                .Replace("ô", "o").Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
                .Replace("ơ", "o").Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
                .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
                .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
                .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y");

            // Remove non-alphanumeric characters except hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");

            // Trim hyphens from start and end
            slug = slug.Trim('-');

            return slug;
        }

        private string EnsureUniqueSlug(string baseSlug)
        {
            var uniqueSlug = baseSlug;
            var suffix = 2;

            while (_context.Posts.Any(p => p.Slug == uniqueSlug))
            {
                uniqueSlug = $"{baseSlug}-{suffix}";
                suffix++;
            }

            return uniqueSlug;
        }
    }
}
