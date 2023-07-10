using FitnessApp1.Models;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp1.ViewModels.Account
{
    public class AccountVM
    {
        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required,DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required,DataType(DataType.Password),Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}
