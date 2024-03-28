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
            var password = "MemCx35ecx8abc_";
            var passwordHasher = new PasswordHasher<User>();

            var adminRole = new IdentityRole(RoleTypes.Admin.ToString());
            adminRole.NormalizedName = adminRole?.Name?.ToLower();
            var userRole = new IdentityRole(RoleTypes.User.ToString());
            userRole.NormalizedName = userRole?.Name?.ToLower();

            List<IdentityRole> roles = new List<IdentityRole>() { adminRole, userRole};

            builder.Entity<IdentityRole>()
                .HasData(roles);

            var user = new User("Yagor", "Molchan", "Poland", "Bialystok","yagormolchan@gmail.com", "+48796147133");

            user.NormalizedUserName = user?.UserName?.ToLower();
            user.NormalizedEmail = user?.Email?.ToLower();

            user.PasswordHash = passwordHasher.HashPassword(user, password);

            builder.Entity<User>()
                .HasData(user);

            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();

            userRoles.Add(new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = roles.First(q => q.Name == RoleTypes.Admin.ToString()).Id
            });

            userRoles.Add(new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = roles.First(q => q.Name == RoleTypes.User.ToString()).Id
            });

            builder.Entity<IdentityUserRole<string>>()
                .HasData(userRoles);

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

            builder.Entity<Brand>()
                .HasData(new Brand
                {
                    Id = 1,
                    Name = "Adidas"
                });

            builder.Entity<Brand>()
                .HasData(new Brand
                {
                    Id = 2,
                    Name = "Calvin Klein"
                });

            builder.Entity<Brand>()
                .HasData(new Brand
                {
                    Id = 3,
                    Name = "Nike"
                });

            builder.Entity<Brand>()
                .HasData(new Brand
                {
                    Id = 4,
                    Name = "Marko"
                });

            builder.Entity<Brand>()
                .HasData(new Brand
                {
                    Id = 5,
                    Name = "Belwest"
                });

            builder.Entity<Brand>()
                .HasData(new Brand
                {
                    Id = 6,
                    Name = "Vans"
                });

            builder.Entity<Brand>()
                .HasData(new Brand
                {
                    Id = 7,
                    Name = "Converse"
                });

            builder.Entity<Brand>()
                .HasData(new Brand
                {
                    Id = 8,
                    Name = "Tommi Hilfliger"
                });

            builder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 1,
                    Name = "Boots"
                });

            builder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 2,
                    Name = "Sneakers"
                });

            builder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 3,
                    Name = "Gumshoes"
                });

            builder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 4,    
                    Name = "Shoes"
                });

            builder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 5,
                    Name = "Slippers"
                });


            builder.Entity<Color>()
               .HasData(new Color
               {
                   Id = 1,
                   Name = "Dark Blue"
               });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 2,
                    Name = "Yellow"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 3,
                    Name = "Orrange"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {   
                    Id = 4,
                    Name = "Dark Khaki"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 5,
                    Name = "Olive"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 6,
                    Name = "Green"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 7,
                    Name = "Orange Red"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 8,
                    Name = "Dark Gray"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 9,
                    Name = "Brown"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 10,
                    Name = "Chocolate"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 11,
                    Name = "Red"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 12,
                    Name = "Black"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 13,
                    Name = "White"
                });

            builder.Entity<Color>()
                .HasData(new Color
                {
                    Id = 14,
                    Name = "Beige"
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 1,
                    Value = 33
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 2,
                    Value = 34
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 3,
                    Value = 35
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 4,
                    Value = 36
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 5,
                    Value = 37
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 6,
                    Value = 38
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 7,
                    Value = 39
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 8,
                    Value = 40
                });

            builder.Entity<Size>()
                .HasData(new Size
                {   
                    Id = 9,
                    Value = 41
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 10,
                    Value = 42
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 11,
                    Value = 43
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 12,
                    Value = 44
                });

            builder.Entity<Size>()
                .HasData(new Size
                { 
                    Id = 13,
                    Value = 45
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 14,
                    Value = 46
                });

            builder.Entity<Size>()
                .HasData(new Size
                {
                    Id = 15,
                    Value = 47
                });

            base.OnModelCreating(builder);
        }
    }
}
