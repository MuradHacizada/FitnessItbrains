using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp1.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public List<IFormFile> Photo { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public bool IsMain { get; set; }
    }
}
