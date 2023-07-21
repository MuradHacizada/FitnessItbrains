using FitnessApp1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.DAL
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<İncome> İncomes { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Revenue> Revenue { get; set; }
        public DbSet<Change> Changes { get; set; }
        public DbSet<PackageTrainer> PackageTrainers { get; set; }
        public DbSet<Slider>Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<Service> Icons { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PackageTag> PackageTags { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Bio> Bio { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
