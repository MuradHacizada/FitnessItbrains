using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

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
                    Product book = _context.Products.Include(f => f.Discount).Include(f => f.Comments).Include(p => p.ProductImages).FirstOrDefault(f => f.Id == item.ProductId);
                    BasketItemVM basketItemVM = new BasketItemVM();
                    if (book != null)
                    {
                        basketItemVM.Product = book;
                        basketItemVM.Count = item.Count;
                        basketItemVM.Price = basketItemVM.Product.DiscountId == null ? basketItemVM.Product.Price : basketItemVM.Product.Price * (100 - basketItemVM.Product.Discount.DiscountPercent) / 100;
                        basketData.Count++;
                        basketData.TotalPrice += basketItemVM.Price * basketItemVM.Count;
                    }
                    Package package = _context.Packages.FirstOrDefault(f => f.Id == item.PackageId);
                    if (package != null)
                    {

                        basketItemVM.Package = package;
                        basketItemVM.Count = item.Count;

                        if (item.PackagPrice == 1)
                        {
                            basketItemVM.Price += basketItemVM.Package.Price;

                        }
                        if (item.PackagPrice == 2)
                        {
                            basketItemVM.Price += basketItemVM.Package.PriceYear;

                        }
                        if (item.PackagPrice == 3)
                        {
                            basketItemVM.Price += basketItemVM.Package.PriceLife;

                        }

                        basketData.Count++;
                        basketData.TotalPrice += basketItemVM.Price;
                    }
                    basketData.BasketItems.Add(basketItemVM);

                }
            }


            return basketData;

        }
    }
}
