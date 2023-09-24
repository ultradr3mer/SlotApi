﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SlotApi.Database;

#nullable disable

namespace SlotApi.Migrations
{
    [DbContext(typeof(SlotsContext))]
    [Migration("20230924115647_RewardsTableRwmoved")]
    partial class RewardsTableRwmoved
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SlotApi.Database.DiscordUser", b =>
                {
                    b.Property<string>("DiscordId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Balance")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("DailyRewardLast")
                        .HasColumnType("datetime2");

                    b.Property<int>("DailyRewardStreak")
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DiscordId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("SlotApi.Database.SlotSpin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Cost")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("DiscordUserDiscordId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ResultJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DiscordUserDiscordId");

                    b.ToTable("SlotSpins");
                });

            modelBuilder.Entity("SlotApi.Database.SlotSpin", b =>
                {
                    b.HasOne("SlotApi.Database.DiscordUser", "DiscordUser")
                        .WithMany()
                        .HasForeignKey("DiscordUserDiscordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiscordUser");
                });
#pragma warning restore 612, 618
        }
    }
}
