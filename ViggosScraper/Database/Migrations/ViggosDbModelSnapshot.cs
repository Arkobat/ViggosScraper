﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ViggosScraper.Database;

#nullable disable

namespace ViggosScraper.Database.Migrations
{
    [DbContext(typeof(ViggosDb))]
    partial class ViggosDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ViggosScraper.Database.BattleResult", b =>
                {
                    b.Property<int>("ResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ResultId"));

                    b.Property<int>("BattleId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<bool>("Won")
                        .HasColumnType("boolean");

                    b.HasKey("ResultId");

                    b.HasIndex("BattleId");

                    b.HasIndex("UserId");

                    b.ToTable("BattleResults");
                });

            modelBuilder.Entity("ViggosScraper.Database.BeerPongBattle", b =>
                {
                    b.Property<int>("BattleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BattleId"));

                    b.Property<bool>("Confirmed")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("BattleId");

                    b.ToTable("BeerPongBattles");
                });

            modelBuilder.Entity("ViggosScraper.Database.DbDato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("DbDato");
                });

            modelBuilder.Entity("ViggosScraper.Database.DbLogo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<bool>("Private")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("LogosDates");
                });

            modelBuilder.Entity("ViggosScraper.Database.DbLogoGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Symbol")
                        .IsUnique();

                    b.ToTable("LogoGroups");
                });

            modelBuilder.Entity("ViggosScraper.Database.DbUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("text");

                    b.Property<string>("Glass")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProfileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RealName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ViggosScraper.Database.Permission", b =>
                {
                    b.Property<int>("PermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PermissionId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("PermissionId");

                    b.HasIndex("UserId");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("ViggosScraper.Database.BattleResult", b =>
                {
                    b.HasOne("ViggosScraper.Database.BeerPongBattle", "Battle")
                        .WithMany("Results")
                        .HasForeignKey("BattleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ViggosScraper.Database.DbUser", "User")
                        .WithMany("Battles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Battle");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ViggosScraper.Database.DbDato", b =>
                {
                    b.HasOne("ViggosScraper.Database.DbUser", "User")
                        .WithMany("Datoer")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ViggosScraper.Database.DbLogo", b =>
                {
                    b.HasOne("ViggosScraper.Database.DbLogoGroup", "Group")
                        .WithMany("Dates")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("ViggosScraper.Database.Permission", b =>
                {
                    b.HasOne("ViggosScraper.Database.DbUser", "User")
                        .WithMany("Permissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ViggosScraper.Database.BeerPongBattle", b =>
                {
                    b.Navigation("Results");
                });

            modelBuilder.Entity("ViggosScraper.Database.DbLogoGroup", b =>
                {
                    b.Navigation("Dates");
                });

            modelBuilder.Entity("ViggosScraper.Database.DbUser", b =>
                {
                    b.Navigation("Battles");

                    b.Navigation("Datoer");

                    b.Navigation("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}
