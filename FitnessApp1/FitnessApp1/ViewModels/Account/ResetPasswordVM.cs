using System.ComponentModel.DataAnnotations;

namespace FitnessApp1.ViewModels.Account
{
    public class ResetPasswordVM
    {
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}
