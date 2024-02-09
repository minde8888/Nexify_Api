using IntegrasjonTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Nexify.Domain.Entities.Auth;
using System.Net;
using System.Text;

namespace IntegrasjonTests.ControllerTests
{
    public class AuthControllerTests : IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JwtToken _jwtToken;

        public AuthControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                 .AddUserSecrets<Program>().Build();
            _factory = new CustomWebApplicationFactory<Program>(configuration);
            _jwtToken = new JwtToken(configuration);
            _client = _factory.CreateClient();
            _baseUrl = "/api/v1/Auth";
        }

        [Fact]
        public async Task Signup_ReturnsOkResult()
        {
            // Arrange
            var user = new Signup
            {
                UserId = new Guid(),
                Name = "John",
                Surname = "Doe",
                PhoneNumber = "12345678",
                Email = "johndoe@example.com",
                Password = "Password123!",
                Roles = "User"
            };

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            _factory.AuthRepositoryMock.Setup(x => x
                .CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var response = await _client.PostAsync($"{_baseUrl}/signup", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SignupResponse>(responseContent);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task Login_ReturnsOkResult()
        {
            // Arrange
            var login = new Login
            {
                Email = "johndoe@example.com",
                Password = "Password123!"
            };

            _factory.AuthRepositoryMock.Setup(x => x.AuthUserAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) => Mock.Of<ApplicationUser>(u => u.Email == email));

            _factory.AuthRepositoryMock.Setup(x => x.PasswordValidatorAsync(It.IsAny<ApplicationUser>(),
                It.IsAny<string>())).ReturnsAsync(true);

            _factory.TokenApiMock.Setup(x => x.RolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User", "Admin" });

            _factory.TokenServiceMock.Setup(x => x.GenerateJwtTokenAsync(It.IsAny<ApplicationUser>()))
               .ReturnsAsync(new AuthResults
               {
                   Token = _jwtToken.GenerateJwtToken(login.Email, "User"),
                   RefreshToken = "sampleRefreshToken",
                   Success = true,
                   Errors = new List<string>()
               });

            var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{_baseUrl}/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = JsonConvert.DeserializeObject<AuthResults>(responseContent);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.Empty(result.Errors);
        }

        //[Fact]
        //public async Task RefreshToken_ReturnsOkResult()
        //{
        //    // Arrange
        //    var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
        //    var requestToken = new RequestToken
        //    {
        //        Token = token,
        //        RefreshToken = "validRefreshToken"
        //    };

        //    _factory.TokenRepositoryMock.Setup(x => x.GetToken(It.IsAny<string>()))
        //        .ReturnsAsync((string token) => new RefreshToken
        //        {
        //            Id = 1,
        //            UserId = Guid.NewGuid(),
        //            Token = token,
        //            Expires = DateTime.UtcNow.AddDays(1),
        //            JwtId = Guid.NewGuid().ToString(),
        //            IsUsed = false,
        //            IsRevoked = false,
        //            AddedDate = DateTime.UtcNow,
        //            ExpiryDate = DateTime.UtcNow.AddDays(30)
        //        });

        //    _factory.TokenRepositoryMock.Setup(x => x.Update(It.IsAny<RefreshToken>()))
        //        .Returns(Task.CompletedTask);

        //    var authResults = new AuthResults
        //    {
        //        Token = "newToken",
        //        RefreshToken = "newRefreshToken",
        //        Success = true,
        //        Errors = new List<string>()
        //    };

        //    var content = new StringContent(JsonConvert.SerializeObject(requestToken),
        //        Encoding.UTF8, "application/json");

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    // Act
        //    var response = await _client.PostAsync($"{_baseUrl}/refreshToken", content);
        //    var responseContent = await response.Content.ReadAsStringAsync();

        //    // Assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var result = JsonConvert.DeserializeObject<AuthResults>(responseContent);
        //    Assert.NotNull(result);
        //    Assert.True(result.Success);
        //    Assert.NotNull(result.Token);
        //    Assert.NotNull(result.RefreshToken);
        //    Assert.Empty(result.Errors);
        //}

        [Fact]
        public async Task ForgotPassword_ReturnsOkResult()
        {
            // Arrange
            var forgotPassword = new ForgotPassword
            {
                Email = "test@example.com"
            };

            var createdUser = new ApplicationUser
            {
                Roles = "Basic",
                Email = "johndoe@example.com",
                UserName = "John"
            };

            _factory.AuthServiceWrapMock.Setup(x => x.FindUserAsync(forgotPassword.Email)).ReturnsAsync(createdUser);

            _factory.AuthServiceWrapMock.Setup(x => x.TokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("token");

            _factory.AuthServiceWrapMock.Setup(x => x.UpdateUserAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.CompletedTask);

            var content = new StringContent(JsonConvert.SerializeObject(forgotPassword), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{_baseUrl}/forgot-password", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_ReturnsOkResult()
        {
            // Arrange
            var resetPassword = new ResetPassword
            {
                Email = "test@example.com",
                Token = "test-token",
                Password = "newPassword123!"
            };

            var createdUser = new ApplicationUser
            {
                Roles = "Basic",
                Email = "johndoe@example.com",
                UserName = "John"
            };

            _factory.AuthServiceWrapMock.Setup(x => x.FindUserAsync(resetPassword.Email)).ReturnsAsync(createdUser);
            _factory.AuthServiceWrapMock.Setup(x =>
            x.NewPasswordAsync(createdUser, resetPassword.Token, resetPassword.Password))
                .ReturnsAsync(IdentityResult.Success);
            _factory.AuthServiceWrapMock.Setup(x => x.UpdateUserAsync(It.IsAny<ApplicationUser>()))
             .Returns(Task.CompletedTask);

            var content = new StringContent(JsonConvert.SerializeObject(resetPassword), Encoding.UTF8, "application/json");

            // Act  
            var response = await _client.PostAsync($"{_baseUrl}/reset-password", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthResults>(responseContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
