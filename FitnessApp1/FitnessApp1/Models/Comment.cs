namespace FitnessApp1.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public AppUser AppUser { get; set; }
        public bool IsAccess { get; set; }
        public string AppUserId { get; set; }
        public int? Star { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int? BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
