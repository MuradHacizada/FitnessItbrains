using EntityFramework_Slider.Services.Interfaces;
using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Services
{
    //public class CategoryService : ICategoryService
    //{
    //    private readonly AppDbContext _context;
    //    public CategoryService(AppDbContext context)
    //    {
    //        _context = context;
    //    }
    //    public async Task<IEnumerable<Category>> GetAll()
    //    {
    //        return await _context.Categories.ToListAsync();
    //    }


    //    public async Task<List<Category>> GetPaginatedDatas(int page, int take) => await _context.Categories.Skip((page * take) - take).Take(take).ToListAsync();

    //    public async Task<int> GetCountAsync() => await _context.Categories.CountAsync();


    //}
}
