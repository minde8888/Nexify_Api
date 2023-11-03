using Microsoft.Extensions.Options;
using Nexify.Domain.Entities.Email;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Nexify.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly SendGridClient _sendGridClient;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _sendGridClient = new SendGridClient(_emailSettings.ApiKey);
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage, string plainTextContent)
        {
            var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlMessage);

            try
            {
                var response = await _sendGridClient.SendEmailAsync(msg);

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
