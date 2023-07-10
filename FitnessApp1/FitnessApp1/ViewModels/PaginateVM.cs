namespace FitnessApp1.ViewModels
{
    public class PaginateVM<T>
    {
        public List<T> Items { get; set; }
        public decimal CurrentPage { get; set; }
        public decimal TotalPage { get; set; }
    }
}
