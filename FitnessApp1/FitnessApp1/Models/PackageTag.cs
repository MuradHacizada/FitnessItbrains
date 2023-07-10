namespace FitnessApp1.Models
{
    public class PackageTag
    {
        public int Id { get; set; }

        public int PackageId { get; set; }
        public int TagId { get; set; }

        public Package Package { get; set; }
        public Tag Tag { get; set; }
    }
}
