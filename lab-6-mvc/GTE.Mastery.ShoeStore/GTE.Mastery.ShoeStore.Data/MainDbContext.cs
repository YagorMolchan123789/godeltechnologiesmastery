using GTE.Mastery.ShoeStore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Color = GTE.Mastery.ShoeStore.Domain.Entities.Color;
using Size = GTE.Mastery.ShoeStore.Domain.Entities.Size;
using Microsoft.AspNetCore.Identity;
using GTE.Mastery.ShoeStore.Domain;
using GTE.Mastery.ShoeStore.Domain.Enums;

namespace GTE.Mastery.ShoeStore.Data
{
    public class MainDbContext : IdentityDbContext<User>
    {
        public DbSet<Shoe> Shoes { get; set; }

        public DbSet<Size> Sizes { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options):base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .Property(u => u.FirstName)
                .HasColumnType("nvarchar(50)")
                .IsRequired();

            builder.Entity<User>()
                .Property(u => u.LastName)
                .HasColumnType("nvarchar(50)")
                .IsRequired();

            builder.Entity<User>()
                .Property(u => u.Country)
                .HasColumnType("nvarchar(20)")
                .IsRequired();

            builder.Entity<User>()
                .Property(u => u.City)
                .HasColumnType("nvarchar(20)")
                .IsRequired();

            builder.Entity<Shoe>()
                .Property(s=> s.Name)
                .HasColumnType("nvarchar(50)")
                .IsRequired();

            builder.Entity<Shoe>()
                .Property(s => s.Vendor)
                .HasColumnType("nvarchar(20)")
                .IsRequired();

            builder.Entity<Brand>()
                .Property(b => b.Name)
                .HasColumnType("nvarchar(20)")
                .IsRequired();

            builder.Entity<Category>()
                .Property(c => c.Name)
                .HasColumnType("nvarchar(20)")
                .IsRequired();

            builder.Entity<Color>()
                .Property(c => c.Name)
                .HasColumnType("nvarchar(20)")
                .IsRequired();

            builder.Entity<Shoe>()
                .HasOne(s => s.Color)
                .WithMany(c => c.Shoes)
                .HasForeignKey(s => s.ColorId);

            builder.Entity<Shoe>()
                .HasOne(s => s.Size)
                .WithMany(s => s.Shoes)
                .HasForeignKey(s => s.SizeId);

            builder.Entity<Shoe>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Shoes)
                .HasForeignKey(s => s.CategoryId);

            builder.Entity<Shoe>()
                .HasOne(s => s.Brand)
                .WithMany(b => b.Shoes)
                .HasForeignKey(s => s.BrandId);

            base.OnModelCreating(builder);
        }
    }
}
