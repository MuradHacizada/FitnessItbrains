namespace FitnessApp1.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Trainer> Trainers { get; set; }
    }
}
