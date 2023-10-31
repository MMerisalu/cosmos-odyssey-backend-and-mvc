﻿// <auto-generated />
using System;
using App.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace App.DAL.EF.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231022142106_RemovedTheCompanyNamesProperty")]
    partial class RemovedTheCompanyNamesProperty
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("App.Domain.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("App.Domain.FlightRoute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProviderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ReservationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.HasIndex("ReservationId");

                    b.ToTable("FlightRoute");
                });

            modelBuilder.Entity("App.Domain.PriceList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ValidUntil")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("PriceLists");
                });

            modelBuilder.Entity("App.Domain.Provider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("FlightEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("FlightStart")
                        .HasColumnType("datetimeoffset");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("RouteInfoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TravelTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(48)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("RouteInfoId");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("App.Domain.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Companies")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("LayOvers")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PriceListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TotalFlightTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(48)");

                    b.Property<decimal>("TotalPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("PriceListId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("App.Domain.RouteInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Distance")
                        .HasColumnType("bigint");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("FromId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PriceListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("To")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ToId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PriceListId");

                    b.ToTable("RouteInfos");
                });

            modelBuilder.Entity("App.Domain.FlightRoute", b =>
                {
                    b.HasOne("App.Domain.Provider", "Provider")
                        .WithMany("Flights")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("App.Domain.Reservation", "Reservation")
                        .WithMany("Routes")
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("App.Domain.Provider", b =>
                {
                    b.HasOne("App.Domain.Company", "Company")
                        .WithMany("Providers")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Domain.RouteInfo", "RouteInfo")
                        .WithMany("Providers")
                        .HasForeignKey("RouteInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("RouteInfo");
                });

            modelBuilder.Entity("App.Domain.Reservation", b =>
                {
                    b.HasOne("App.Domain.PriceList", "PriceList")
                        .WithMany("Reservations")
                        .HasForeignKey("PriceListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PriceList");
                });

            modelBuilder.Entity("App.Domain.RouteInfo", b =>
                {
                    b.HasOne("App.Domain.PriceList", "PriceList")
                        .WithMany("Legs")
                        .HasForeignKey("PriceListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PriceList");
                });

            modelBuilder.Entity("App.Domain.Company", b =>
                {
                    b.Navigation("Providers");
                });

            modelBuilder.Entity("App.Domain.PriceList", b =>
                {
                    b.Navigation("Legs");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("App.Domain.Provider", b =>
                {
                    b.Navigation("Flights");
                });

            modelBuilder.Entity("App.Domain.Reservation", b =>
                {
                    b.Navigation("Routes");
                });

            modelBuilder.Entity("App.Domain.RouteInfo", b =>
                {
                    b.Navigation("Providers");
                });
#pragma warning restore 612, 618
        }
    }
}
