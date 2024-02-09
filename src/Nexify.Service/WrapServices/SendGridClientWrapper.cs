using Nexify.Service.Interfaces;
using SendGrid.Helpers.Mail;
using SendGrid;
using Nexify.Domain.Entities.Email;
using Microsoft.Extensions.Options;

namespace Nexify.Service.WrapServices
{
    public class SendGridClientWrapper : ISendGridClientWrapper
    {
        private readonly SendGridClient _sendGridClient;
        private readonly EmailSettings _emailSettings;

        public SendGridClientWrapper(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _sendGridClient = new SendGridClient(_emailSettings.ApiKey);
        }

        public Task<Response> SendEmailAsync(SendGridMessage msg)
        {
            return _sendGridClient.SendEmailAsync(msg);
        }
    }

}
