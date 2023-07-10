using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContext = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<BasketVM> ShowBasket()
        {

            BasketVM basketData = new BasketVM
            {
                TotalPrice = 0,
                BasketItems = new List<BasketItemVM>(),
                Count = 0
            };
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(_httpContext.HttpContext.User.Identity.Name);
                List<BasketItem> basketItems = _context.BasketItems.Include(b => b.AppUser).Where(b => b.AppUserId == user.Id).ToList();
                foreach (BasketItem item in basketItems)
                {
                    Product book = _context.Products.Include(f => f.Discount).Include(p=>p.ProductImages).FirstOrDefault(f => f.Id == item.ProductId);
                    if (book != null)
                    {
                        BasketItemVM basketItemVM = new BasketItemVM
                        {
                            Product = book,
                            Count = item.Count
                        };
                        basketItemVM.Price = basketItemVM.Product.DiscountId == null ? basketItemVM.Product.Price : basketItemVM.Product.Price * (100 - basketItemVM.Product.Discount.DiscountPercent) / 100;

                        basketData.BasketItems.Add(basketItemVM);
                        basketData.Count++;
                        basketData.TotalPrice += basketItemVM.Price * basketItemVM.Count;
                    }
                }
            }


            return basketData;

        }
    }
}
