using FitnessApp1.Models;

namespace FitnessApp1.ViewModels
{
    public class BasketItemVM
    {
        public Product Product { get; set; }
        public Package? Package { get; set; }
        public decimal Price { get; set; }
        public decimal? PackagePrice { get; set; }
        public int? PacageCount { get; set; }
        public int Count { get; set; }
    }
}
