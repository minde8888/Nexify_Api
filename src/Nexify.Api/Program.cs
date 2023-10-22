using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Nexify.Api;
using Nexify.Api.Middleware;
using Nexify.Data.Configuration;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Auth;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureServices(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        using (var serviceScope = app.Services.CreateScope())
        {
            var loggerFactory = serviceScope.ServiceProvider.GetService<ILoggerFactory>();
            try
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.Migrate();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                await ContextSeed.SeedEssentialsAsync(userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Internal server error occurred");
            }
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                  Path.Combine(Directory.GetCurrentDirectory(), "Images")),
            RequestPath = "/Images"
        });

        app.UseMiddleware<ExceptionMiddleware>();

        //app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.UseCors();

        app.Run();
    }
}
