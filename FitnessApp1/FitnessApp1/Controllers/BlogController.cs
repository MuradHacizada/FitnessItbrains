using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private UserManager<AppUser> _userManager;


        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<Blog> blogs = await _context.Blogs.ToListAsync();
            return View(blogs);
        }
        public IActionResult Detail(int id)
        {
            Blog blog=_context.Blogs.FirstOrDefault(b => b.Id == id);
            ViewBag.Comments = _context.Comments.Include(c => c.Blog).Include(c => c.AppUser).Where(c => c.BlogId == id).ToList();
            ViewBag.Count = _context.Comments.Include(c => c.Blog).Include(c => c.AppUser).Where(c => c.BlogId == id).ToList().Count();

            return View(blog);
        }
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) { return RedirectToAction("login", "account"); }
            //if (!ModelState.IsValid) return RedirectToAction("Detail", "Shop", new { id = comment.ProductId });
            if (!_context.Blogs.Any(f => f.Id == comment.BlogId)) return NotFound();
            Comment newComment = new Comment
            {
                Message = comment.Message,
                BlogId = comment.BlogId,
                CreatedAt = DateTime.Now,
                AppUserId = user.Id,
                IsAccess = true,
            };
            _context.Comments.Add(newComment);
            _context.SaveChanges();
            return RedirectToAction("Detail", "blog", new { id = comment.BlogId });
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) { return RedirectToAction("login", "account"); }

            if (!ModelState.IsValid) return RedirectToAction("Index", "blog");
            if (!_context.Comments.Any(c => c.Id == id && c.IsAccess == true && c.AppUserId == user.Id)) return NotFound();
            Comment comment = _context.Comments.FirstOrDefault(c => c.Id == id && c.AppUserId == user.Id);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return RedirectToAction("Detail", "Blog", new { id = comment.BlogId });
        }
    }
}
