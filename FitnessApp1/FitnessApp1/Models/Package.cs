using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp1.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public int Session { get; set; }
        public string Training { get; set; }
        public decimal Price { get; set; }
        public decimal PriceYear { get; set; }
        public decimal PriceLife { get; set; }
        public string? Image { get; set; }
        [NotMapped]

        public IFormFile? Photo { get; set; }
        public List<PackageTag> PackageTags { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }
        [NotMapped]
        public List<int> TrainerIds { get; set; }
        public List<PackageTrainer> PackageTrainers { get; set; }
    }
}
