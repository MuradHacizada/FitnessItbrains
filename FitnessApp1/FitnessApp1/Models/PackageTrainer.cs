namespace FitnessApp1.Models
{
    public class PackageTrainer
    {
        public int Id { get; set; }
        public Package Package { get; set; }
        public int PackageId { get; set; }
        public Trainer Trainer { get; set; }
        public int TrainerId { get; set; }

    }
}
