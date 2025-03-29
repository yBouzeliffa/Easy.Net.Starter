using Easy.Net.Starter.Models;

namespace Easy.Net.Starter.Services
{
    public interface IEmailService
    {
        Task<OpeResult<bool>> SendEmailValidationAsync(string toEmail, string firstName, string verificationLink);
        Task<OpeResult<bool>> SendWelcomeEmailAsync(string toEmail, string firstName, string mobileAppLink);
    }
}
