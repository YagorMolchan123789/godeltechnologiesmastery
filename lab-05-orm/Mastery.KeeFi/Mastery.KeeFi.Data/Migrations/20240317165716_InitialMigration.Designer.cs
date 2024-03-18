﻿// <auto-generated />
using System;
using Mastery.KeeFi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Mastery.KeeFi.Data.Migrations
{
    [DbContext(typeof(MainDbContext))]
    [Migration("20240317165716_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Mastery.KeeFi.Domain.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly?>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Clients");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DateOfBirth = new DateOnly(1974, 6, 28),
                            FirstName = "Valiantsina",
                            LastName = "Chekan",
                            Tags = "[\"string\",\"integer\"]"
                        });
                });

            modelBuilder.Entity("Mastery.KeeFi.Domain.Entities.DocumentMetadata", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<int>("ContentLength")
                        .HasColumnType("int");

                    b.Property<string>("ContentMd5")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Documents");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ClientId = 1,
                            ContentLength = 12000,
                            ContentMd5 = "1dfeb3a910fd3976c30f85214be7a9ff",
                            ContentType = "application/msword",
                            FileName = "security.docx",
                            Title = "Security"
                        });
                });

            modelBuilder.Entity("Mastery.KeeFi.Domain.Entities.DocumentMetadataProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DocumentId")
                        .HasColumnType("int");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.ToTable("DocumentMetadataProperty");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DocumentId = 1,
                            Key = "Country",
                            Value = "Belarus"
                        },
                        new
                        {
                            Id = 2,
                            DocumentId = 1,
                            Key = "City",
                            Value = "Minsk"
                        });
                });

            modelBuilder.Entity("Mastery.KeeFi.Domain.Entities.DocumentMetadata", b =>
                {
                    b.HasOne("Mastery.KeeFi.Domain.Entities.Client", "Client")
                        .WithMany("Documents")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Mastery.KeeFi.Domain.Entities.DocumentMetadataProperty", b =>
                {
                    b.HasOne("Mastery.KeeFi.Domain.Entities.DocumentMetadata", "Document")
                        .WithMany("Properties")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");
                });

            modelBuilder.Entity("Mastery.KeeFi.Domain.Entities.Client", b =>
                {
                    b.Navigation("Documents");
                });

            modelBuilder.Entity("Mastery.KeeFi.Domain.Entities.DocumentMetadata", b =>
                {
                    b.Navigation("Properties");
                });
#pragma warning restore 612, 618
        }
    }
}
