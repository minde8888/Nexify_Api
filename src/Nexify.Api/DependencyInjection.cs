using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nexify.Data.Configuration;
using Nexify.Data.Context;
using Nexify.Data.Repositories;
using Nexify.Domain.Entities.Auth;
using Nexify.Domain.Entities.Email;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Interfaces;
using Nexify.Service.MapperProfile;
using Nexify.Service.Services;
using Nexify.Service.Validators;
using Nexify.Service.WrapServices;
using System.Text;

namespace Nexify.Api
{
    public static class DependencyInjection
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MapperProfile));

            services.AddTransient<AuthService>();
            services.AddTransient<CategoryService>();
            services.AddTransient<DiscountService>();
            services.AddTransient<PostService>();
            services.AddTransient<ProductsService>();
            services.AddTransient<SubcategoryService>();
            services.AddTransient<BlogCategoryService>();

            services.AddTransient<IImagesService, ImagesService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddTransient<ITokenServiceWrap, TokenServiceWrap>();
            services.AddTransient<IAuthRepository, AuthRepository>();
            services.AddTransient<ITokenServiceWrap, TokenServiceWrap>();
            services.AddTransient<IAuthServiceWrap, AuthServiceWrap>();

            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IItemCategoryRepository, ItemCategoryRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IProductsRepository, ProductRepository>();
            services.AddTransient<ISubcategoryRepository, SubcategoryRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IBlogCategoryRepository, BlogCategoryRepository>();

            services.AddTransient<UserManager<ApplicationUser>>();

            services.AddTransient<IValidator<Login>, LoginValidator>();
            services.AddTransient<IValidator<RequestToken>, RequestTokenValidator>();
            services.AddTransient<IValidator<Signup>, SignupValidator>();
            services.AddTransient<IValidator<ForgotPassword>, ForgotPasswordValidator>();
            services.AddTransient<IValidator<ResetPassword>, ResetPasswordValidator>();
            services.AddTransient<IValidator<CategoryDto>, CategoryValidator>();
            services.AddTransient<IValidator<PagedParams<Product>>, PagedParamsValidator<Product>>();
            services.AddTransient<IValidator<PaginationFilter>, PaginationFilterValidator>();
            services.AddTransient<IValidator<PostCategories>, PostCategoriesValidator>();
            services.AddTransient<IValidator<PostRequest>, PostRequestValidator>();
            services.AddTransient<IValidator<ProductCategories>, ProductCategoriesValidator>();
            services.AddTransient<IValidator<ProductRequest>, ProductRequestValidator>();
            services.AddTransient<IValidator<ProductUpdate>, ProductUpdateValidator>();
            services.AddTransient<IValidator<RequestToken>, RequestTokenValidator>();
            services.AddTransient<IValidator<SubcategoryDto>, SubcategoryValidator>();
            services.AddTransient<IValidator<BlogCategoryDto>, BlogCategoryDtoValidator>();

            services.AddSingleton<IUriService>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var request = httpContextAccessor.HttpContext?.Request;
                var uri = request != null ? $"{request.Scheme}://{request.Host.ToUriComponent()}" : null;
                return new UriService(uri);
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>(o => o.SignIn.RequireConfirmedAccount = true)
             .AddRoles<ApplicationRole>()
             .AddRoleManager<RoleManager<ApplicationRole>>()
             .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidAudience = configuration["JwtConfig:Audience"],
                ClockSkew = TimeSpan.Zero,
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddSingleton(tokenValidationParameters);

            var connectionString = Environment.GetEnvironmentVariable("DockerCommandsConnectionString");
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

            services.AddHttpsRedirection(options => options.HttpsPort = 9002);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins("http://localhost:3000", "https://localhost:9002")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }
    }
}
