namespace FitnessApp1.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PackageName { get; set; }
        public decimal? PacPrice { get; set; }

        public decimal? Price { get; set; }
        public int? ProductId { get; set; }
        public int? PackageId { get; set; }
        public int OrderId { get; set; }
        public Product? Product { get; set; }
        public Package? Package { get; set; }
        public AppUser AppUser { get; set; }
        public string? AppUserId { get; set; }
        public Order Order { get; set; }
    }
}
