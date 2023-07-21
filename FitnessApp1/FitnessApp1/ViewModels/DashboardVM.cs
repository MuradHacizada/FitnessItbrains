using FitnessApp1.Models;

namespace FitnessApp1.ViewModels
{
    public class DashboardVM
    {
        public Revenue Revenue { get; set; }
        public List<Order> Orders { get; set; }
        public Change Change { get; set; }
    }
}
