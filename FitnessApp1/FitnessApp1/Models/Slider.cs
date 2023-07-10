using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp1.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Image { get; set; }
        public bool IsDeactive { get; set; }
        [NotMapped]

        public IFormFile Photo { get; set; }
    }
}
