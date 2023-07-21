using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp1.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public decimal Salary { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }

        public string FacebookURL { get; set; }
        public string InstagramURL { get; set; }
        public string TwitterURL { get; set; }
        public string YoutubeURL { get; set; }

        public int? PositionId { get; set; }
        public Position Position { get; set; }
        public List<PackageTrainer> PackageTrainers { get; set; }

    }
}
