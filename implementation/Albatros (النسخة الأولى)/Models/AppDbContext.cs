using Albatros.Models;
using Microsoft.EntityFrameworkCore;

namespace Albatros.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<VisitRequest> VisitRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User -> Properties
            modelBuilder.Entity<Property>()
                .HasOne(p => p.User)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Favorites
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> VisitRequests
            modelBuilder.Entity<VisitRequest>()
                .HasOne(v => v.User)
                .WithMany(u => u.VisitRequests)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property -> Images
            modelBuilder.Entity<PropertyImage>()
                .HasOne(i => i.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.PropertyId);

            // Property -> Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PropertyId);

            // Property -> Favorites
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Property)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.PropertyId);

            // Property -> VisitRequests
            modelBuilder.Entity<VisitRequest>()
                .HasOne(v => v.Property)
                .WithMany(p => p.VisitRequests)
                .HasForeignKey(v => v.PropertyId);

            // Unique index: one favorite per user per property
            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.PropertyId })
                .IsUnique();
        }
    }
}