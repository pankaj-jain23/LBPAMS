﻿// <auto-generated />
using System;
using EAMS_DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EAMS_DAL.Migrations
{
    [DbContext(typeof(EamsContext))]
    [Migration("20231117113257_BoothMasterModelChanges")]
    partial class BoothMasterModelChanges
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EAMS_ACore.AssemblyMaster", b =>
                {
                    b.Property<int>("AssemblyMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AssemblyMasterId"));

                    b.Property<int>("AssemblyCode")
                        .HasColumnType("integer");

                    b.Property<string>("AssemblyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AssemblyType")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DistrictMasterId")
                        .HasColumnType("integer");

                    b.Property<int>("PCMasterId")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("AssemblyMasterId");

                    b.HasIndex("DistrictMasterId");

                    b.HasIndex("PCMasterId");

                    b.ToTable("AssemblyMaster");
                });

            modelBuilder.Entity("EAMS_ACore.BoothMaster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AssemblyMasterId")
                        .HasColumnType("integer");

                    b.Property<string>("BoothCode_No")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BoothName")
                        .HasColumnType("text");

                    b.Property<string>("BoothNoAuxy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DistrictMasterId")
                        .HasColumnType("integer");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("TotalVoters")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AssemblyMasterId");

                    b.HasIndex("DistrictMasterId");

                    b.HasIndex("StateMasterId");

                    b.ToTable("BoothMaster");
                });

            modelBuilder.Entity("EAMS_ACore.DistrictMaster", b =>
                {
                    b.Property<int>("DistrictMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DistrictMasterId"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DistrictCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("DistrictMasterId");

                    b.HasIndex("StateMasterId");

                    b.ToTable("DistrictMaster");
                });

            modelBuilder.Entity("EAMS_ACore.EventMaster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("EndDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("EventSequence")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("StartDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("EventMaster");
                });

            modelBuilder.Entity("EAMS_ACore.ParliamentConstituencyMaster", b =>
                {
                    b.Property<int>("PCMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PCMasterId"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PcCodeNo")
                        .HasColumnType("text");

                    b.Property<string>("PcName")
                        .HasColumnType("text");

                    b.Property<string>("PcType")
                        .HasColumnType("text");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("PCMasterId");

                    b.HasIndex("StateMasterId");

                    b.ToTable("ParliamentConstituencyMaster");
                });

            modelBuilder.Entity("EAMS_ACore.StateMaster", b =>
                {
                    b.Property<int>("StateMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("StateMasterId"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StateCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("StateMasterId");

                    b.ToTable("StateMaster");
                });

            modelBuilder.Entity("EAMS_ACore.AssemblyMaster", b =>
                {
                    b.HasOne("EAMS_ACore.DistrictMaster", "DistrictMaster")
                        .WithMany("AssemblyMaster")
                        .HasForeignKey("DistrictMasterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EAMS_ACore.ParliamentConstituencyMaster", "ParliamentConstituencyMaster")
                        .WithMany("AssemblyMaster")
                        .HasForeignKey("PCMasterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DistrictMaster");

                    b.Navigation("ParliamentConstituencyMaster");
                });

            modelBuilder.Entity("EAMS_ACore.BoothMaster", b =>
                {
                    b.HasOne("EAMS_ACore.AssemblyMaster", "AssemblyMaster")
                        .WithMany("BoothMaster")
                        .HasForeignKey("AssemblyMasterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EAMS_ACore.DistrictMaster", "DistrictMaster")
                        .WithMany("BoothMaster")
                        .HasForeignKey("DistrictMasterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EAMS_ACore.StateMaster", "StateMaster")
                        .WithMany("BoothMaster")
                        .HasForeignKey("StateMasterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssemblyMaster");

                    b.Navigation("DistrictMaster");

                    b.Navigation("StateMaster");
                });

            modelBuilder.Entity("EAMS_ACore.DistrictMaster", b =>
                {
                    b.HasOne("EAMS_ACore.StateMaster", "StateMaster")
                        .WithMany("DistrictMasters")
                        .HasForeignKey("StateMasterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StateMaster");
                });

            modelBuilder.Entity("EAMS_ACore.ParliamentConstituencyMaster", b =>
                {
                    b.HasOne("EAMS_ACore.StateMaster", "StateMaster")
                        .WithMany("ParliamentConstituencyMaster")
                        .HasForeignKey("StateMasterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StateMaster");
                });

            modelBuilder.Entity("EAMS_ACore.AssemblyMaster", b =>
                {
                    b.Navigation("BoothMaster");
                });

            modelBuilder.Entity("EAMS_ACore.DistrictMaster", b =>
                {
                    b.Navigation("AssemblyMaster");

                    b.Navigation("BoothMaster");
                });

            modelBuilder.Entity("EAMS_ACore.ParliamentConstituencyMaster", b =>
                {
                    b.Navigation("AssemblyMaster");
                });

            modelBuilder.Entity("EAMS_ACore.StateMaster", b =>
                {
                    b.Navigation("BoothMaster");

                    b.Navigation("DistrictMasters");

                    b.Navigation("ParliamentConstituencyMaster");
                });
#pragma warning restore 612, 618
        }
    }
}
