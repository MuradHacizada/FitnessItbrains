using FitnessApp1.Models;
using System.Collections.Generic;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface ICategoryService
    {
        
        Task<IEnumerable<Package>> GetPackage();
        Task<IEnumerable<Category>> GetAll();
        Task<int> GetCountAsync();
        Task<List<Category>> GetPaginatedDatas(int page, int take);

    }
}
