using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp1.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
