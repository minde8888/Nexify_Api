using Microsoft.Extensions.Options;
using Moq;
using Nexify.Domain.Entities.Email;
using Nexify.Domain.Exceptions;
using Nexify.Service.Interfaces;
using Nexify.Service.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace Tests.Services
{
    public class EmailServiceTests
    {
        private readonly Mock<IOptions<EmailSettings>> _emailSettingsMock;
        private readonly Mock<ISendGridClientWrapper> _sendGridClientWrapperMock;
        private EmailService _emailService;

        public EmailServiceTests()
        {
            _emailSettingsMock = new Mock<IOptions<EmailSettings>>();
            _sendGridClientWrapperMock = new Mock<ISendGridClientWrapper>();

            var emailSettings = new EmailSettings
            {
                ApiKey = "SG.someapikey",
                SenderEmail = "noreply@example.com",
                SenderName = "Example"
            };

            _emailSettingsMock.Setup(x => x.Value).Returns(emailSettings);

            _emailService = new EmailService(_emailSettingsMock.Object, _sendGridClientWrapperMock.Object);
        }

        [Fact]
        public async Task SendEmailAsync_Success_ReturnsTrue()
        {
            // Arrange
            var email = "test@example.com";
            var subject = "Test Subject";
            var htmlMessage = "<p>This is a test email message.</p>";
            var plainTextContent = "This is a test email message.";

            _sendGridClientWrapperMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>()))
                .ReturnsAsync(new Response(HttpStatusCode.OK, null, null));

            // Act
            var result = await _emailService.SendEmailAsync(email, subject, htmlMessage, plainTextContent);

            // Assert
            Assert.True(result);
            _sendGridClientWrapperMock.Verify(x => x.SendEmailAsync(It.IsAny<SendGridMessage>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailAsync_Failure_ThrowsEmailException()
        {
            // Arrange
            _sendGridClientWrapperMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>()))
                .ReturnsAsync(new Response(HttpStatusCode.BadRequest, new StringContent("{\"errors\":[{\"message\":\"Bad Request\"}]}"), null));

            // Act & Assert
            await Assert.ThrowsAsync<EmailException>(() => _emailService.SendEmailAsync("fail@example.com", "Fail Subject", "<p>Failure</p>", "Failure"));
        }
    }
}
