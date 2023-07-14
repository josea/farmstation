﻿// <auto-generated />
using System;
using FarmStationDb.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FarmStationDb.Migrations
{
    [DbContext(typeof(FarmrContext))]
    [Migration("20230726174802_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("FarmStation.Models.Balance", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Balance1")
                        .HasColumnType("longtext")
                        .HasColumnName("Balance");

                    b.HasKey("Id");

                    b.ToTable("Balances");
                });

            modelBuilder.Entity("FarmStation.Models.Db.Config", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Config1")
                        .HasMaxLength(60000)
                        .HasColumnType("longtext")
                        .HasColumnName("Config");

                    b.HasKey("Id");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("FarmStation.Models.Db.Drive", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<float?>("Drives")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Drives");
                });

            modelBuilder.Entity("FarmStation.Models.Db.Lastplot", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Lastplot1")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("Lastplot");

                    b.HasKey("Id");

                    b.ToTable("Lastplots");
                });

            modelBuilder.Entity("FarmStation.Models.Db.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Type")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("User")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.HasKey("NotificationId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("FarmStation.Models.Db.Offline", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Name")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int?>("Notify")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Offline");
                });

            modelBuilder.Entity("FarmStation.Models.Db.Status", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int?>("Isfarming")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("FarmStation.Models.Farm", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Data")
                        .HasColumnType("longtext");

                    b.Property<string>("FarmingStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasDefaultValueSql("'OFFLINE'");

                    b.Property<DateTime?>("FarmingStatusTimestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("LastStatusNotificationTimestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastUpdated")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValue(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<bool>("NotifyWhenOffline")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("true");

                    b.Property<bool?>("PublicApi")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("User")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasDefaultValueSql("'(none)'");

                    b.HasKey("Id");

                    b.ToTable("Farms");
                });
#pragma warning restore 612, 618
        }
    }
}
