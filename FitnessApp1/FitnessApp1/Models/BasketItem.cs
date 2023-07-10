namespace FitnessApp1.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public int? PackagPrice { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int? PackageId { get; set; }
        public Package? Package { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
