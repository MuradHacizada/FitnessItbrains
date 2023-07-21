namespace FitnessApp1.Models
{
    public class Change
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public decimal Amount { get; set; }
        public int? TrainerId { get; set; }
        public Trainer? Trainer { get; set; }
        public DateTime ChangedTime { get; set; }
        public string? Role { get; set; }
    }
}
