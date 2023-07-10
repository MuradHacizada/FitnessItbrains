using System.ComponentModel.DataAnnotations;

namespace FitnessApp1.Models
{
    public class Discount
    {
        public int Id { get; set; }
        [Required]
        public int DiscountPercent { get; set; }
        public List<Product> Products { get; set; }
    }
}
