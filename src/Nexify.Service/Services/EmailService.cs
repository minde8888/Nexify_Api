using Microsoft.Extensions.Options;
using Nexify.Domain.Entities.Email;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Nexify.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ISendGridClientWrapper _sendGridClientWrapper;

        public EmailService(IOptions<EmailSettings> emailSettings, ISendGridClientWrapper sendGridClientWrapper)
        {
            _emailSettings = emailSettings.Value;
            _sendGridClientWrapper = sendGridClientWrapper;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string plainTextContent)
        {
            var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlMessage);

            try
            {
                var response = await _sendGridClientWrapper.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    throw new EmailException($"Failed to send email. Status Code: {response.StatusCode}, Response: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                throw new EmailException($"Failed to send email: {ex.Message}");
            }
        }
    }
}
