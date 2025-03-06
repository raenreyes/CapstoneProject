using CapstoneProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProject.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed initial books
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    ProductName = "C# in Depth",
                    ProductDescription = "A deep dive into the C# programming language.",
                    ProductPrice = 39.99m,
                    ProductImage = "/images/csharp-depth.jpg"
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Clean Code",
                    ProductDescription = "A guide to writing clean, maintainable, and efficient code.",
                    ProductPrice = 34.99m,
                    ProductImage = "/images/clean-code.jpg"
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "The Pragmatic Programmer",
                    ProductDescription = "Classic book on software craftsmanship and best practices.",
                    ProductPrice = 44.99m,
                    ProductImage = "/images/pragmatic-programmer.jpg"
                },
                new Product
                {
                    ProductId = 4,
                    ProductName = "Design Patterns: Elements of Reusable Object-Oriented Software",
                    ProductDescription = "A foundational book on design patterns in software development.",
                    ProductPrice = 49.99m,
                    ProductImage = "/images/design-patterns.jpg"
                }
            );
        }
    }
}
