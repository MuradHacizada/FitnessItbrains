using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Controllers
{
    public class PackageController : Controller
    {
        private readonly AppDbContext _context;
        private UserManager<AppUser> _userManager;

        public PackageController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.PackagesCount = await _context.Packages.Include(p => p.PackageTags).ThenInclude(pt => pt.Tag).CountAsync();
            PackageVM packageVM = new PackageVM()
            {
                Packages = await _context.Packages.Include(p => p.PackageTags).ThenInclude(pt => pt.Tag).Take(6).ToListAsync()
            };
            return View(packageVM);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Package package = await _context.Packages.Include(p => p.PackageTags).ThenInclude(p => p.Tag).Include(t => t.PackageTrainers).ThenInclude(t => t.Trainer).ThenInclude(t => t.Position).Where(p => p.Id == id).FirstOrDefaultAsync();
            if (package == null)
            {
                return RedirectToAction("index", "error");
            }
            ViewBag.Tags = _context.Tags.Include(t => t.PackageTags).ThenInclude(t => t.Package).ToList();

            return View(package);
        }
        public IActionResult Packagetag(int id)
        {
            List<Package> blogs = _context.Packages.Include(p => p.PackageTags).ThenInclude(p => p.Tag).Where(c => c.PackageTags.Any(bt => bt.TagId == id)).ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View(blogs);
        }
        public async Task<IActionResult> Add(int id, int val)
        {
            #region MyRegion
            //Package package = _context.Packages.FirstOrDefault(f => f.Id == id);


            //if (User.Identity.IsAuthenticated)
            //{
            //    AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            //    if (user == null)
            //    {
            //        return RedirectToAction("login", "account");
            //    }
            //    BasketItem basketItem = _context.BasketItems.FirstOrDefault(b => b.PackageId == package.Id && b.AppUserId == user.Id);
            //    if (basketItem == null)
            //    {


            //        basketItem = new BasketItem
            //        {
            //            AppUserId = user.Id,
            //            PackageId = package.Id,
            //            Count = 1,
            //            PackagPrice = val,
            //        };
            //        _context.BasketItems.Add(basketItem);

            //    }
            //    _context.SaveChanges();
            //    return RedirectToAction("Detail", "package", new { id = package.Id });
            //} 
            #endregion


            Package package = _context.Packages.FirstOrDefault(f => f.Id == id);


            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("login", "account");
                }
                BasketItem basketItem = _context.BasketItems.FirstOrDefault(b => b.PackageId == package.Id && b.AppUserId == user.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = user.Id,
                        PackageId = package.Id,
                        Count = 1,
                        PackagPrice = val
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count = 1;
                    basketItem.PackagPrice = val;

                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            return RedirectToAction("login", "account");
        }
        public async Task<IActionResult> LoadMore(int skipCount)
        {
            int count = await _context.Packages.Include(p => p.PackageTags).ThenInclude(pt => pt.Tag).CountAsync();
            if (count <= skipCount)
            {
                return Content("Go to Hell broooo !!!");
            }
            List<Package> packages = await _context.Packages.Include(p => p.PackageTags).ThenInclude(pt => pt.Tag).Skip(skipCount).Take(6).ToListAsync();
            return PartialView("_LoadMorePackagesPartial", packages);
        }

    }
}
