using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailConfirmationAsync(string email, string callbackUrl);
        Task SendPasswordResetAsync(string email, string callbackUrl);
        Task SendAccountLockedAsync(string email);
    }
}