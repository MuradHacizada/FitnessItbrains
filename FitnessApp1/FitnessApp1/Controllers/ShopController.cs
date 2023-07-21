using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private UserManager<AppUser> _userManager;

        public ShopController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(int sortId, int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Products.Where(a => !a.IsDeleted).Count() / 3);
            List<Product> model = _context.Products.Include(b => b.Discount).Include(a => a.ProductCategories).ThenInclude(a => a.Category).Include(a => a.ProductImages).Where(a => !a.IsDeleted).Skip((page - 1) * 3).ToList();
            ViewBag.Count = _context.Products.Where(a => !a.IsDeleted).ToList().Count();
            ViewBag.id = sortId;

            switch (sortId)
            {
                case 1:
                    model = _context.Products.Include(a => a.ProductImages).Include(b => b.Discount).Where(a => !a.IsDeleted).Skip((page - 1) * 3).ToList();
                    break;
                case 2:
                    model = _context.Products.Include(a => a.ProductImages).Include(b => b.Discount).Where(a => !a.IsDeleted).Skip((page - 1) * 3).OrderByDescending(s => s.Name).ToList();
                    break;
                case 3:
                    model = _context.Products.Include(a => a.ProductImages).Include(b => b.Discount).Where(a => !a.IsDeleted).Skip((page - 1) * 3).OrderBy(s => s.Name).ToList();
                    break;
                case 4:
                    model = _context.Products.Include(a => a.ProductImages).Include(b => b.Discount).Where(a => !a.IsDeleted).Skip((page - 1) * 3).OrderByDescending(s => s.Price).ToList();
                    break;
                case 5:
                    model = _context.Products.Include(a => a.ProductImages).Include(b => b.Discount).Where(a => !a.IsDeleted).Skip((page - 1) * 3).OrderBy(s => s.Price).ToList();
                    break;
                default:

                    break;
            }
            return View(model);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products.Include(a => a.ProductImages).Include(b => b.Discount).Include(a => a.ProductCategories).ThenInclude(a => a.Category).Where(b => !b.IsDeleted && b.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return RedirectToAction("index", "error");
            }

            List<Comment> comments = _context.Comments.Where(c => c.ProductId == id).ToList();
            var averageStar = comments.Where(c => c.Star != null).Average(c => c.Star);
            if (averageStar==null)
            {
                ViewBag.StarCount = 5;

            }
            else
            {
            ViewBag.StarCount =(int)averageStar;
                            }
            List<Category> categories = _context.Categories.Include(c => c.ProductCategories).ThenInclude(bc => bc.Product).Where(b => b.ProductCategories.Any(bc => bc.ProductId == id)).ToList();

            List<Product> relatedProducts = new List<Product>();
            foreach (var item in categories)
            {
                relatedProducts = _context.Products.Include(b => b.Discount).Include(a => a.ProductImages).Include(b => b.ProductCategories).ThenInclude(bt => bt.Category).Where(b => !b.IsDeleted && b.ProductCategories.Any(bc => bc.CategoryId == item.Id)).ToList();
            }
            DetailsVM detailsVM = new DetailsVM()
            {
                RelatedProducts = relatedProducts,
                Categories = categories,
                Product = product,
            };
            ViewBag.Comments = _context.Comments.Include(c => c.Product).Include(c => c.AppUser).Where(c => c.ProductId == id && c.IsAccess == true).ToList();
            ViewBag.Count = _context.Comments.Include(c => c.Product).Include(c => c.AppUser).Where(c => c.ProductId == id && c.IsAccess == true).ToList().Count();

            ViewBag.RelatedProducts = relatedProducts;
            return View(detailsVM);
        }
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            if (!User.Identity.IsAuthenticated)
            {

                return RedirectToAction("login", "account");
            }
            else
            {

                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null && user.IsActive == false) { return RedirectToAction("login", "account"); }
                if (!_context.Products.Any(f => f.Id == comment.ProductId)) return NotFound();
                Comment newComment = new Comment
                {
                    Message = comment.Message,
                    ProductId = comment.ProductId,
                    Star = comment.Star,
                    CreatedAt = DateTime.Now,
                    AppUserId = user.Id,
                    IsAccess = true,
                };
                _context.Comments.Add(newComment);
                _context.SaveChanges();
                return RedirectToAction("Detail", "Shop", new { id = comment.ProductId });
            }
        }

        public IActionResult Search(string search)
        {
            List<Product> products = _context.Products.Where(p => !p.IsDeleted && p.Name.ToLower().Contains(search.ToLower())).ToList();
            return PartialView("_SearchPartial", products);
        }
        public async Task<IActionResult> DeleteComment(int id)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null && user.IsActive == false) { return RedirectToAction("login", "account"); }

            if (!ModelState.IsValid) return RedirectToAction("Index", "Shop");
            if (!_context.Comments.Any(c => c.Id == id && c.IsAccess == true && c.AppUserId == user.Id)) return NotFound();
            Comment comment = _context.Comments.FirstOrDefault(c => c.Id == id && c.AppUserId == user.Id);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return RedirectToAction("Detail", "Shop", new { id = comment.ProductId });
        }

        public IActionResult ProductCategory(int id)
        {
            List<Product> products = _context.Products.Include(b => b.ProductImages).Include(b => b.Discount).Include(b => b.ProductCategories).ThenInclude(b => b.Category).Include(b => b.Comments).Where(c => !c.IsDeleted && c.ProductCategories.Any(bt => bt.CategoryId == id)).ToList();
            ViewBag.Categories = _context.Categories.Include(c => c.ProductCategories).ThenInclude(c => c.Product).ToList();
            return View(products);
        }
    }
}
