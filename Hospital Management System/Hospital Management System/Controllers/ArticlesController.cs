using System;
using System.Linq;
using System.Web.Mvc;
using Hospital_Management_System.Models;
using PagedList;

namespace Hospital_Management_System.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: /Articles
        public ActionResult Index(int page = 1)
        {
            const int pageSize = 5;
            ViewBag.ActiveNav = "articles";
            var articles = _db.Posts
                .Where(a => a.Status)
                .OrderByDescending(a => a.PublishedDate)
                .ToPagedList(page, pageSize);

            return View(articles);
        }

        // GET: /Articles/{slug}
        public ActionResult Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return RedirectToAction("Index");
            }

            ViewBag.ActiveNav = "articles";
            var article = _db.Posts.FirstOrDefault(a => a.Slug == slug && a.Status);
            if (article == null)
            {
                return HttpNotFound();
            }

            return View(article);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
