using EntityFramework_Slider.Services.Interfaces;
using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAll()
        {
            var categoriesWithProducts = await _context.Categories
         .Where(c => !c.IsDeleted)
         .Select(c => new Category
         {
             Id = c.Id,
             Name = c.Name,
             IsDeleted = c.IsDeleted,
             ProductCategories = c.ProductCategories
                 .Where(pc => !pc.Product.IsDeleted && !pc.Category.IsDeleted)
                 .Select(pc => new ProductCategory
                 {
                     CategoryId = pc.CategoryId,
                     ProductId = pc.ProductId,
                     Product = pc.Product
                 })
                 .ToList()
         })
         .ToListAsync();
            return categoriesWithProducts;
        }
        public async Task<IEnumerable<Package>> GetPackage()
        {
            return await _context.Packages.Include(c => c.PackageTrainers).ToListAsync();
        }


        public async Task<List<Category>> GetPaginatedDatas(int page, int take) => await _context.Categories.Skip((page * take) - take).Take(take).ToListAsync();

        public async Task<int> GetCountAsync() => await _context.Categories.CountAsync();


    }
}
