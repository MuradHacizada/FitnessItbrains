using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsDeleted { get; set; } = false;
        public int Count { get; set; }
        public decimal Price { get; set; } 
        [NotMapped]
        public List<IFormFile> Photo { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }
        [NotMapped]
        public List<int>? CategoryIds { get; set; }
        public List<Comment> Comments { get; set; }
        public int? DiscountId { get; set; }
        public Discount? Discount { get; set; }
    }
}
