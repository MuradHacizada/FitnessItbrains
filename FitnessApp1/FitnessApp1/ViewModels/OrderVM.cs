using FitnessApp1.Models;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp1.ViewModels
{
    public class OrderVM
    {
        [Required]
        [StringLength(maximumLength: 50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(maximumLength: 40)]
        public string CountryRegion { get; set; }
        [StringLength(maximumLength: 25)]
        public string Firstname { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        public string Address { get; set; } 

        [Required]
        [StringLength(maximumLength: 30)]
        public string City { get; set; }
     
        [Required]
        [StringLength(maximumLength: 30)]
        public string ZipCode { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public List<BasketItem> BasketItems { get; set; }
    }
}
