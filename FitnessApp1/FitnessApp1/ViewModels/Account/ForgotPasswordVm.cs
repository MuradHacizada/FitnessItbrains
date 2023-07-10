using System.ComponentModel.DataAnnotations;

namespace FitnessApp1.ViewModels.Account
{
    public class ForgotPasswordVm
    {
        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
    }
}
