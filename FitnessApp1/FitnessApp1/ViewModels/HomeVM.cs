using FitnessApp1.Models;

namespace FitnessApp1.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Benefit> Benefits { get; set; } 
        public List<Package> Packages { get; set; } 
        public List<Trainer> Trainers { get; set; } 
        public List<Blog> Blogs { get; set; }
    }
}
