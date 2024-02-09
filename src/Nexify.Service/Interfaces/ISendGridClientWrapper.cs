using SendGrid;
using SendGrid.Helpers.Mail;

namespace Nexify.Service.Interfaces
{
    public interface ISendGridClientWrapper
    {
        Task<Response> SendEmailAsync(SendGridMessage msg);
    }
}