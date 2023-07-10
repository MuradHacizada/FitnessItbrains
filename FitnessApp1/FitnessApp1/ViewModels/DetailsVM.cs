using FitnessApp1.Models;

namespace FitnessApp1.ViewModels
{
    public class DetailsVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; } 
        public List<Category> Categories { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
