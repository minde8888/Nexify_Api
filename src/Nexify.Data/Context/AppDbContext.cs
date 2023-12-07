using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexify.Domain.Entities.Auth;
using Nexify.Data.Configuration;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.User;
using Nexify.Domain.Entities.Subcategories;
using Nexify.Domain.Entities.Posts;
using System.Reflection.Emit;

namespace Nexify.Data.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<IdentityUserClaim<Guid>> IdentityUserClaims { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Subcategory> Subcategory { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<CategoriesProducts> CategoriesProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("Identity");

            builder.Entity<User>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<Product>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<Post>().HasQueryFilter(p => p.IsDeleted == false);

            builder.Entity<CategoriesProducts>()
                .HasKey(i => new { i.ProductsId, i.CategoriesId });

            builder.Entity<Category>()
                .HasMany(x => x.Products)
                .WithMany(x => x.Categories)
                .UsingEntity<CategoriesProducts>(
                    x => x.HasOne(x => x.Products)
                        .WithMany()
                        .HasForeignKey(x => x.ProductsId),
                    x => x.HasOne(x => x.Categories)
                        .WithMany()
                        .HasForeignKey(x => x.CategoriesId));

            builder.Entity<SubcategoriesProducts>()
                .HasKey(i => new { i.ProductsId, i.SubcategoriesId });

            builder.Entity<Subcategory>()
                .HasMany(x => x.Products)
                .WithMany(x => x.Subcategories)
                .UsingEntity<SubcategoriesProducts>(
                    x => x.HasOne(x => x.Products)
                        .WithMany()
                        .HasForeignKey(x => x.ProductsId),
                    x => x.HasOne(x => x.Subcategories)
                        .WithMany()
                        .HasForeignKey(x => x.SubcategoriesId));

            builder.Entity<Category>()
                .HasMany(c => c.Subcategories)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
