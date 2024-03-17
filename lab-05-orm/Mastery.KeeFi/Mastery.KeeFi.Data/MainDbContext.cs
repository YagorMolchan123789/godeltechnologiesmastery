using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Mastery.KeeFi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;


namespace Mastery.KeeFi.Data
{
    public class MainDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        public DbSet<DocumentMetadata> Documents { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .Property(c => c.Tags)
                .HasConversion(new ValueConverter<List<string>, string>(
                 ValueConverter => JsonConvert.SerializeObject(ValueConverter, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore }),
                 ValueConverter => JsonConvert.DeserializeObject<List<string>>(ValueConverter, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore })
            ));            

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Client)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentMetadata>()
                .HasMany(d => d.Properties)
                .WithOne(p => p.Document)
                .HasForeignKey(p => p.DocumentId);

            modelBuilder.Entity<Client>()
                .HasData(new Client
                {
                    Id = 1,
                    FirstName = "Valiantsina",
                    LastName = "Chekan",
                    DateOfBirth = DateOnly.Parse("1974-06-28"),
                    Tags = new List<string>() { "string", "integer" }
                });

            modelBuilder.Entity<DocumentMetadata>()
                .HasData(new DocumentMetadata
                {
                    Id = 1,
                    ClientId = 1,
                    FileName = "security.docx",
                    Title = "Security",
                    ContentLength = 12000,
                    ContentType = "application/msword",
                    ContentMd5 = "1dfeb3a910fd3976c30f85214be7a9ff"
                });

            modelBuilder.Entity<DocumentMetadataProperty>()
                .HasData(new DocumentMetadataProperty
                {
                    DocumentId = 1,
                    Id = 1,
                    Key = "Country",
                    Value = "Belarus"
                });

            modelBuilder.Entity<DocumentMetadataProperty>()
                .HasData(new DocumentMetadataProperty
                {
                    DocumentId = 1,
                    Id = 2,
                    Key = "City",
                    Value = "Minsk"
                });
        }
    }
}
