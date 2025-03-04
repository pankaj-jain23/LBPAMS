﻿    
using EAMS_ACore;
using EAMS_ACore.AuthModels;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.CountingDayModels;
using EAMS_ACore.Models.ElectionType;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.Models.QueueModel;
using EAMS_ACore.NotificationModels;
using EAMS_ACore.SignalRModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace EAMS_DAL.DBContext;

public partial class EamsContext : IdentityDbContext<UserRegistration>
{ 

    public EamsContext(DbContextOptions<EamsContext> options)
        : base(options)
    {
    }

   
    public virtual DbSet<EventMaster> EventMaster { get; set; }
    public virtual DbSet<EventAbbr> EventAbbr { get; set; }
    public virtual DbSet<StateMaster> StateMaster { get; set; }
    public virtual DbSet<DistrictMaster> DistrictMaster { get; set; }
    public virtual DbSet<AssemblyMaster> AssemblyMaster { get; set; }
    public virtual DbSet<FourthLevelH> FourthLevelH { get; set; }
    public virtual DbSet<PSZonePanchayat> PSZonePanchayat { get; set; }
    public virtual DbSet<GPPanchayatWards> GPPanchayatWards { get; set; }
    public virtual DbSet<BoothMaster> BoothMaster { get; set; }
    public virtual DbSet<ParliamentConstituencyMaster> ParliamentConstituencyMaster { get; set; }

    public virtual DbSet<FieldOfficerMaster> FieldOfficerMaster { get; set; }
    public virtual DbSet<ElectionInfoMaster> ElectionInfoMaster { get; set; }
    public virtual DbSet<SlotManagementMaster> SlotManagementMaster { get; set; }
    public virtual DbSet<PollDetail> PollDetails { get; set; }
    public virtual DbSet<PollInterruption> PollInterruptions { get; set; }
    public virtual DbSet<PollInterruptionHistory> PollInterruptionHistory { get; set; }
    public virtual DbSet<Notification> Notification { get; set; }

    public virtual DbSet<SMSTemplate> SMSTemplate { get; set; }
    public virtual DbSet<SMSSent> SMSSent { get; set; }
    public virtual DbSet<ElectionTypeMaster> ElectionTypeMaster { get; set; }
    public virtual DbSet<ElectionConductedMaster> ElectionConductedMaster { get; set; }
    public virtual DbSet<CountingTypeMaster> CountingTypeMaster { get; set; }
    public virtual DbSet<CountingBasicInfoMaster> CountingBasicInfoMaster { get; set; }
    public virtual DbSet<CountingLocationMaster> CountingLocationMaster { get; set; }
    public virtual DbSet<CountingVenueMaster> CountingVenueMaster { get; set; }

    public virtual DbSet<PollingStationMaster> PollingStationMaster { get; set; }
    public virtual DbSet<PollingStationGender> PollingStationGender { get; set; }
    public virtual DbSet<PollingLocationMaster> PollingLocationMaster { get; set; }
    public virtual DbSet<DashboardConnectedUser> DashboardConnectedUser { get; set; }
    public virtual DbSet<HelpDeskDetail> HelpDeskDetail { get; set; }
    public virtual DbSet<QIS> QIS { get; set; }
    public virtual DbSet<PPR> PPR { get; set; }
    public virtual DbSet<RandomizationTaskDetail> RandomizationTaskDetail { get; set; }
    public virtual DbSet<BLOMaster> BLOMaster { get; set; }
    public virtual DbSet<MobileVersion> MobileVersion { get; set; }
    public virtual DbSet<Kyc> Kyc { get; set; }
    public virtual DbSet<UnOpposed> UnOpposed { get; set; }
    public virtual DbSet<GPVoter> GPVoter { get; set; }
    public virtual DbSet<ResultDeclaration> ResultDeclaration { get; set; }
    public virtual DbSet<AROResultMaster> AROResultMaster { get; set; }
    public virtual DbSet<ResultDeclarationHistory> ResultDeclarationHistory { get; set; }
    public virtual DbSet<VoterType> VoterType { get; set; }
    //  public virtual DbSet<BLOBoothMaster> BLOBoothMaster { get; set; } 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Add your custom model configurations here
    }
}