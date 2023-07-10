namespace FitnessApp1.Models
{
    public class ProductComment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CommentId { get; set; }

        public Comment Comment { get; set; }
        public Product Product { get; set; }
    }
}
