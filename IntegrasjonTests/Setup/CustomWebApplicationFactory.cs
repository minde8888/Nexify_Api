using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Nexify.Domain.Entities.Auth;
using Nexify.Domain.Interfaces;
using Nexify.Service.Interfaces;


namespace IntegrasjonTests.Setup
{
    public class CustomWebApplicationFactory<TStartup> :
        WebApplicationFactory<TStartup>
        where TStartup : class
    {
        public Mock<IAuthRepository> AuthRepositoryMock { get; } = new();
        public Mock<IBlogCategoryRepository> BlogCategoryRepositoryMock { get; } = new();
        public Mock<IBlogRepository> BlogRepositoryMock { get; } = new();
        public Mock<ICategoryRepository> CategoryRepositoryMock { get; } = new();
        public Mock<IPostCategoryRepository> PostCategoryRepository { get; } = new();
        public Mock<IProductsRepository> ProductsRepositoryMock { get; } = new();
        public Mock<ISubcategoryRepository> SubcategoryRepository { get; } = new();
        public Mock<ITokenRepository> TokenRepositoryMock { get; } = new();
        public Mock<IUserRepository> UserRepositoryMock { get; } = new();
        public Mock<IEmailService> EmailService { get; } = new();        
        public Mock<ITokenServiceWrap> TokenApiMock { get; } = new();
        public Mock<IOptionsMonitor<JwtConfig>> JwtConfigMock { get; } = new();
        public Mock<ITokenService> TokenServiceMock { get; } = new();
        public Mock<IAuthServiceWrap> AuthServiceWrapMock { get; } = new();
        public Mock<IAttributesRepository> AttributesRepositoryMock { get; } = new();

        private readonly IConfiguration _configuration;

        public CustomWebApplicationFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                services.AddSingleton(AuthRepositoryMock.Object);
                services.AddSingleton(BlogCategoryRepositoryMock.Object);
                services.AddSingleton(CategoryRepositoryMock.Object);            
                services.AddSingleton(BlogRepositoryMock.Object);
                services.AddSingleton(PostCategoryRepository.Object);
                services.AddSingleton(ProductsRepositoryMock.Object);
                services.AddSingleton(SubcategoryRepository.Object);
                services.AddSingleton(TokenRepositoryMock.Object);
                services.AddSingleton(UserRepositoryMock.Object);
                services.AddSingleton(TokenApiMock.Object);
                services.AddSingleton(JwtConfigMock.Object);
                services.Configure<JwtConfig>(_configuration.GetSection("JwtConfig"));
                services.AddSingleton(TokenServiceMock.Object);
                services.AddSingleton(AuthServiceWrapMock.Object);
                services.AddSingleton(AttributesRepositoryMock.Object);
            });
        }
    }
}
