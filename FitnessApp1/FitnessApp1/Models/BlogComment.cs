namespace FitnessApp1.Models
{
    public class BlogComment
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public int CommentId { get; set; }

        public Comment Comment { get; set; }
        public Blog Blog { get; set; }
    }
}
