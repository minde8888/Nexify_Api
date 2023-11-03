
namespace Nexify.Domain.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string plainTextContent);
    }
}
