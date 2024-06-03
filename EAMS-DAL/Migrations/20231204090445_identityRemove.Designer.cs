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
    [Migration("20231204090445_identityRemove")]
    partial class identityRemove
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
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

                    b.Property<DateTime?>("AssemblyCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("AssemblyDeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AssemblyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("AssemblyStatus")
                        .HasColumnType("boolean");

                    b.Property<string>("AssemblyType")
                        .HasColumnType("text");

                    b.Property<DateTime?>("AssemblyUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DistrictMasterId")
                        .HasColumnType("integer");

                    b.Property<int>("PCMasterId")
                        .HasColumnType("integer");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.HasKey("AssemblyMasterId");

                    b.HasIndex("DistrictMasterId");

                    b.HasIndex("PCMasterId");

                    b.HasIndex("StateMasterId");

                    b.ToTable("AssemblyMaster");
                });

            modelBuilder.Entity("EAMS_ACore.AuthModels.UserRegistration", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<int>("AssemblyMasterId")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<int>("DistrictMasterId")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("EAMS_ACore.BoothMaster", b =>
                {
                    b.Property<int>("BoothMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BoothMasterId"));

                    b.Property<int>("AssemblyMasterId")
                        .HasColumnType("integer");

                    b.Property<string>("AssignedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("AssignedOnTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AssignedTo")
                        .HasColumnType("text");

                    b.Property<string>("BoothCode_No")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("BoothCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("BoothDeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("BoothName")
                        .HasColumnType("text");

                    b.Property<string>("BoothNoAuxy")
                        .HasColumnType("text");

                    b.Property<bool>("BoothStatus")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("BoothUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DistrictMasterId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsAssigned")
                        .HasColumnType("boolean");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.Property<int?>("TotalVoters")
                        .HasColumnType("integer");

                    b.HasKey("BoothMasterId");

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

                    b.Property<string>("DistrictCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("DistrictCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DistrictDeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DistrictName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("DistrictStatus")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("DistrictUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.HasKey("DistrictMasterId");

                    b.HasIndex("StateMasterId");

                    b.ToTable("DistrictMaster");
                });

            modelBuilder.Entity("EAMS_ACore.EventMaster", b =>
                {
                    b.Property<int>("EventMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EventMasterId"));

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

                    b.HasKey("EventMasterId");

                    b.ToTable("EventMaster");
                });

            modelBuilder.Entity("EAMS_ACore.Models.ElectionInfoMaster", b =>
                {
                    b.Property<int>("ElectionInfoMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ElectionInfoMasterId"));

                    b.Property<string>("AROUserId")
                        .HasColumnType("text");

                    b.Property<int>("AssemblyMasterId")
                        .HasColumnType("integer");

                    b.Property<int>("BoothMasterId")
                        .HasColumnType("integer");

                    b.Property<int>("DistrictMasterId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("EVMDepositedLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ElectionInfoCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ElectionInfoDeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("ElectionInfoStatus")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ElectionInfoUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EventMasterId")
                        .HasColumnType("integer");

                    b.Property<int?>("FinalTVote")
                        .HasColumnType("integer");

                    b.Property<bool?>("IsEVMDeposited")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsMCESwitchOff")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsMockPollDone")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsPartyDeparted")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsPartyDispatched")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsPartyReached")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsPollEnded")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("IsPollEndedLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("IsPollStarted")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsSetupOfPolling")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("MCESwitchOffLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("MockPollDoneLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("PartyDepartedLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("PartyDispatchedLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("PartyReachedLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("PollStartedLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SOUserId")
                        .HasColumnType("text");

                    b.Property<DateTime?>("SetupOfPollingLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.Property<int?>("VoterInQueue")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("VoterInQueueLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("VotingLastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ElectionInfoMasterId");

                    b.ToTable("ElectionInfoMaster");
                });

            modelBuilder.Entity("EAMS_ACore.Models.SectorOfficerMaster", b =>
                {
                    b.Property<int>("SOMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SOMasterId"));

                    b.Property<bool>("IsLocked")
                        .HasColumnType("boolean");

                    b.Property<string>("OTP")
                        .HasColumnType("text");

                    b.Property<int>("OTPAttempts")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("OTPExpireTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("OTPGeneratedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("SOUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("SoAssemblyCode")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("SoCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SoDesignation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SoMobile")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SoName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SoOfficeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("SoStatus")
                        .HasColumnType("boolean");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

                    b.HasKey("SOMasterId");

                    b.ToTable("SectorOfficerMaster");
                });

            modelBuilder.Entity("EAMS_ACore.ParliamentConstituencyMaster", b =>
                {
                    b.Property<int>("PCMasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PCMasterId"));

                    b.Property<string>("PcCodeNo")
                        .HasColumnType("text");

                    b.Property<DateTime?>("PcCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("PcDeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PcName")
                        .HasColumnType("text");

                    b.Property<bool>("PcStatus")
                        .HasColumnType("boolean");

                    b.Property<string>("PcType")
                        .HasColumnType("text");

                    b.Property<DateTime?>("PcUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("StateMasterId")
                        .HasColumnType("integer");

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

                    b.Property<string>("StateCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("StateCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("StateDeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("StateName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("StateStatus")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("StateUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("StateMasterId");

                    b.ToTable("StateMaster");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
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

                    b.HasOne("EAMS_ACore.StateMaster", "StateMaster")
                        .WithMany("AssemblyMaster")
                        .HasForeignKey("StateMasterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DistrictMaster");

                    b.Navigation("ParliamentConstituencyMaster");

                    b.Navigation("StateMaster");
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("EAMS_ACore.AuthModels.UserRegistration", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("EAMS_ACore.AuthModels.UserRegistration", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EAMS_ACore.AuthModels.UserRegistration", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("EAMS_ACore.AuthModels.UserRegistration", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
                    b.Navigation("AssemblyMaster");

                    b.Navigation("BoothMaster");

                    b.Navigation("DistrictMasters");

                    b.Navigation("ParliamentConstituencyMaster");
                });
#pragma warning restore 612, 618
        }
    }
}
