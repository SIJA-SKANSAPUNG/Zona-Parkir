namespace Parking_Zone.Services
{
    public interface IEmailTemplateService
    {
        string GetEmailConfirmationTemplate(string callbackUrl);
        string GetPasswordResetTemplate(string callbackUrl);
        string GetPasswordResetTemplate(string callbackUrl, string userName);
        string GetAccountLockedTemplate();
        string GetWelcomeTemplate(string userName);
    }
}