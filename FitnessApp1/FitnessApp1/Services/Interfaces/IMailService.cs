using FitnessApp1.ViewModels.MailSender;

namespace FitnessApp1.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestVM mailRequest);
    }
}
