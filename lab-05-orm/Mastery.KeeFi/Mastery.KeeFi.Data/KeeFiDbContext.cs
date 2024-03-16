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
    public class KeeFiDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        public DbSet<DocumentMetadata> Documents { get; set; }

        public KeeFiDbContext(DbContextOptions<KeeFiDbContext> options) : base(options) 
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

            modelBuilder.Entity<DocumentMetadata>()
                .Property(d => d.Properties)
                .HasConversion(
                    ValueConverter => JsonConvert.SerializeObject(ValueConverter, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    ValueConverter => JsonConvert.DeserializeObject<Dictionary<string, string>>(ValueConverter, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
            );

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Client)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade);                           

        }
    }
}
