using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.ViewModels;
using MailKit.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Controllers
{
    public class BasketController : Controller
    {
        private AppDbContext _context;
        private UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        public BasketController(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContext = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("login", "account");
            }
            OrderVM model = new OrderVM
            {

                BasketItems = _context.BasketItems.Include(b => b.Package).Include(b => b.Product).ThenInclude(b => b.Discount).Include(b => b.Product.ProductImages).Where(b => b.AppUserId == user.Id && b.PackageId == null).ToList(),

            };


            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> Plus(int Id)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            BasketItem basket = _context.BasketItems.Include(b => b.Package).Include(b => b.Product).ThenInclude(b => b.Discount).FirstOrDefault(b => b.ProductId == Id && b.AppUserId == user.Id/*&&b.PackageId==null*/);

            basket.Count++;
            _context.SaveChanges();
            decimal TotalPrice = 0;
            decimal Price = basket.Count * (basket.Product.DiscountId == null ? basket.Product.Price : basket.Product.Price * (100 - basket.Product.Discount.DiscountPercent) / 100);

            List<BasketItem> basketItems = _context.BasketItems.Include(b => b.AppUser).Include(b => b.Product).Include(b => b.Package).Where(b => b.AppUserId == user.Id).ToList();
            foreach (BasketItem item in basketItems)
            {
                Product product = _context.Products.Include(b => b.Discount).FirstOrDefault(b => b.Id == item.ProductId);

                BasketItemVM basketItemVM = new BasketItemVM
                {
                    Product = product,
                    Package = item.Package,
                    PackagePrice = item.PackagPrice,
                    Count = item.Count
                };
                if (item.PackageId == null)
                {
                    basketItemVM.Price = product.DiscountId == null ? product.Price : product.Price * (100 - product.Discount.DiscountPercent) / 100;
                    TotalPrice += basketItemVM.Price * basketItemVM.Count;
                }
            }

            return Json(new { totalPrice = TotalPrice, Price });
        }
        public async Task<IActionResult> Minus(int Id)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            BasketItem basket = _context.BasketItems.Include(b => b.Package).Include(b => b.Product).ThenInclude(b => b.Discount).FirstOrDefault(b => b.ProductId == Id && b.AppUserId == user.Id);
            if (basket.Count == 1)
            {
                basket.Count = 1;
            }
            else
            {
                basket.Count--;
            }
            _context.SaveChanges();
            decimal TotalPrice = 0;
            decimal Price = basket.Count * (basket.Product.DiscountId == null ? basket.Product.Price : basket.Product.Price * (100 - basket.Product.Discount.DiscountPercent) / 100);

            List<BasketItem> basketItems = _context.BasketItems.Include(b => b.AppUser).Include(b => b.Package).Include(b => b.Product).Where(b => b.AppUserId == user.Id).ToList();
            foreach (BasketItem item in basketItems)
            {
                Product product = _context.Products.Include(b => b.Discount).FirstOrDefault(b => b.Id == item.ProductId);

                BasketItemVM basketItemVM = new BasketItemVM
                {
                    Product = product,
                    Package = item.Package,
                    PackagePrice = item.PackagPrice,
                    Count = item.Count
                };
                if (item.PackageId == null)
                {

                    basketItemVM.Price = product.DiscountId == null ? product.Price : product.Price * (100 - product.Discount.DiscountPercent) / 100;
                    TotalPrice += basketItemVM.Price * basketItemVM.Count;
                }



            }

            return Json(new { totalPrice = TotalPrice, Price });
        }


        public async Task<IActionResult> AddBasket(int id)
        {
            Product product = _context.Products.FirstOrDefault(f => f.Id == id);


            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("login", "account");
                }
                BasketItem basketItem = _context.BasketItems.FirstOrDefault(b => b.ProductId == product.Id && b.AppUserId == user.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Count = 1
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

                //return View("_basketPartial");
            }



            return RedirectToAction("login", "account");
        }
        public async Task<IActionResult> Add(int id, int count)
        {
            Product product = _context.Products.FirstOrDefault(f => f.Id == id);


            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("login", "account");
                }
                BasketItem basketItem = _context.BasketItems.FirstOrDefault(b => b.ProductId == product.Id && b.AppUserId == user.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Count = count
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count += count;
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }



            return RedirectToAction("login", "account");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("login", "account");
                }
                List<BasketItem> basketItems = _context.BasketItems.Where(b => b.ProductId == Id && b.AppUserId == user.Id).ToList();
                if (basketItems == null) return Json(new { status = 404 });
                foreach (var item in basketItems)
                {

                    _context.BasketItems.Remove(item);
                }
            }

            _context.SaveChanges();


            return Json(new { status = 200 });
        }

        public async Task<IActionResult> RemoveAll()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                List<BasketItem> basketItems = _context.BasketItems.Where(b => b.AppUserId == user.Id).ToList();
                if (basketItems == null) return Json(new { status = 404 });
                foreach (var item in basketItems)
                {

                    _context.BasketItems.Remove(item);
                }
            }

            _context.SaveChanges();


            return RedirectToAction("index", "shop");
        }

        public async Task<IActionResult> Checkout()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            OrderVM model = new OrderVM
            {
                Firstname = user.UserName,
                Email = user.Email,
                BasketItems = _context.BasketItems.Include(b => b.Package).Include(b => b.Product).ThenInclude(b => b.Discount).Include(b => b.Product.ProductImages).Include(b => b.AppUser).Where(b => b.AppUserId == user.Id).ToList()

            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            OrderVM model = new OrderVM
            {
                Firstname = user.UserName,
                Email = user.Email,
                BasketItems = _context.BasketItems.Include(b => b.Package).Include(b => b.Product).ThenInclude(b => b.Discount).Include(b => b.Product.ProductImages).Where(b => b.AppUserId == user.Id).ToList()
            };

            if (model.BasketItems.Count == 0) return RedirectToAction("index", "home");
            Order order = new Order
            {
                CountryRegion = orderVM.CountryRegion,
                Address = orderVM.Address,
                City = orderVM.City,
                ZipCode = orderVM.ZipCode,
                Message = " ",
                TotalPrice = 0,
                Date = DateTime.Now,
                AppUserId = user.Id
            };

            foreach (BasketItem item in model.BasketItems)
            {
                if (item.ProductId != null)
                {

                    order.TotalPrice += item.Product.DiscountId == null ? item.Count * item.Product.Price : item.Count * item.Product.Price * (100 - item.Product.Discount.DiscountPercent) / 100;
                }
                if (item.PackagPrice == 1 && item.PackageId != null)
                {
                    order.TotalPrice += item.Package.Price;
                }
                if (item.PackagPrice == 2 && item.PackageId != null)
                {
                    order.TotalPrice += item.Package.PriceYear;
                }
                if (item.PackagPrice == 3 && item.PackageId != null)
                {
                    order.TotalPrice += item.Package.PriceLife;
                }
                if (item.PackageId != null && item.ProductId != null)
                {

                    OrderItem orderItem = new OrderItem
                    {
                        PackageName = item.Package.Title,
                        Name = item.Product.Name,
                        Price = item.Product.DiscountId == null ? item.Count * item.Product.Price : item.Count * item.Product.Price * (100 - item.Product.Discount.DiscountPercent) / 100,
                        AppUserId = user.Id,
                        ProductId = item.Product.Id,
                        PackageId = item.Package.Id,
                        Order = order
                    };
                    item.Product.Count -= item.Count;
                    if (item.Product.Count == 0)
                    {
                        item.Product.IsDeleted = true;
                    }
                    else
                    {
                        item.Product.IsDeleted = false;
                    }
                    if (item.PackagPrice == 1)
                    {
                        orderItem.PacPrice = item.Package.Price;
                    }
                    if (item.PackagPrice == 2)
                    {
                        orderItem.PacPrice = item.Package.PriceYear;
                    }
                    if (item.PackagPrice == 3)
                    {
                        orderItem.PacPrice = item.Package.PriceLife;
                    }
                    _context.OrderItem.Add(orderItem);
                }
                else if (item.PackageId == null && item.ProductId != null)
                {
                    OrderItem orderItem = new OrderItem
                    {
                        Name = item.Product.Name,
                        Price = item.Product.DiscountId == null ? item.Count * item.Product.Price : item.Count * item.Product.Price * (100 - item.Product.Discount.DiscountPercent) / 100,
                        AppUserId = user.Id,
                        ProductId = item.Product.Id,
                        Order = order
                    };
                    item.Product.Count -= item.Count;
                    if (item.Product.Count == 0)
                    {
                        item.Product.IsDeleted = true;
                    }
                    else
                    {
                        item.Product.IsDeleted = false;
                    }
                    _context.OrderItem.Add(orderItem);
                }
                else if (item.PackageId != null && item.ProductId == null)
                {
                    OrderItem orderItem = new OrderItem
                    {
                        PackageName = item.Package.Title,
                        AppUserId = user.Id,
                        PackageId = item.Package.Id,
                        Order = order
                    };
                    if (item.PackagPrice == 1)
                    {
                        orderItem.PacPrice = item.Package.Price;
                    }
                    if (item.PackagPrice == 2)
                    {
                        orderItem.PacPrice = item.Package.PriceYear;
                    }
                    if (item.PackagPrice == 3)
                    {
                        orderItem.PacPrice = item.Package.PriceLife;
                    }
                    _context.OrderItem.Add(orderItem);
                }
            }
            _context.BasketItems.RemoveRange(model.BasketItems);
            _context.Order.Add(order);
            _context.SaveChanges();

            return RedirectToAction("index", "home");
        }
    }
}
