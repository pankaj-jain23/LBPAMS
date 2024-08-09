using AutoMapper;
using EAMS.AuthViewModels;
using EAMS.ViewModels;
using EAMS.ViewModels.BLOMaster;
using EAMS.ViewModels.ChartViewModel;
using EAMS.ViewModels.Polling_Personal_Randomization_ViewModel;
using EAMS.ViewModels.PSFormViewModel;
using EAMS.ViewModels.PublicModels;
using EAMS.ViewModels.QueueViewModel;
using EAMS.ViewModels.ReportViewModel;
using EAMS_ACore;
using EAMS_ACore.AuthModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.Models.QueueModel;
using EAMS_ACore.NotificationModels;
using EAMS_ACore.ReportModels;
using LBPAMS.ViewModels;

namespace EAMS.Helper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region LoginViewModel Login  
            CreateMap<LoginViewModel, Login>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ReverseMap();
            #endregion

            #region UserRegistration UserRegistration  
            CreateMap<UserRegistrationViewModel, UserRegistration>()
            .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
             .ForMember(dest => dest.UserStates, opt => opt.MapFrom(src => src.UserStates));

            CreateMap<StateViewModel, UserState>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.UserDistrict, opt => opt.MapFrom(src => src.Districts))
                .ForMember(dest => dest.UserPCConstituency, opt => opt.MapFrom(src => src.PCConstituencies));

            CreateMap<DistrictViewModel, UserDistrict>()
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.UserAssembly, opt => opt.MapFrom(src => src.Assemblies));

            CreateMap<PCConstituencyViewModel, UserPCConstituency>()
                .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => src.PCMasterId))
                .ForMember(dest => dest.UserAssembly, opt => opt.MapFrom(src => src.Assemblies));

            CreateMap<AssemblyViewModel, UserAssembly>()
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.UserPSZone, opt => opt.MapFrom(src => src.PSZone));
            CreateMap<PSZoneViewModel, UserPSZone>()
                .ForMember(dest => dest.PSZoneMasterId, opt => opt.MapFrom(src => src.PSZoneMasterId));


            #endregion

            #region RoleViewModel Roles

            CreateMap<RolesViewModel, Role>()
                 .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role))
                .ReverseMap();
            #endregion

            #region ValidateMobileViewModel ValidteMobile

            CreateMap<ValidateMobileViewModel, ValidateMobile>().ReverseMap();
            #endregion

            #region CreateSOPinViewModel CreateSOPin
            CreateMap<CreateSOPinViewModel, CreateSOPin>().ReverseMap();
            #endregion

            #region TokenViewModel Token
            CreateMap<TokenViewModel, Token>().ReverseMap();
            #endregion

            #region GetUserViewModel GetUser
            CreateMap<GetUserViewModel, GetUser>().ReverseMap();
            #endregion

            #region NotificationViewModel NotificationModel
            CreateMap<NotificationViewModel, Notification>().ReverseMap();
            #endregion


            #region SMSTemplateViewModel SMSTemplateModel

            CreateMap<SMSTemplateViewModel, SMSTemplate>()
               .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsStatus))
               .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
               .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
               .ForMember(dest => dest.SMSName, opt => opt.MapFrom(src => src.SMSName))
                 .ForMember(dest => dest.SMSTemplateMasterId, opt => opt.MapFrom(src => src.SMSTemplateMasterId))
               //.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.Now.ToUniversalTime()))
               //.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTimeOffset.Now.ToUniversalTime()))

               .ReverseMap();
            #endregion

            #region SMSSentViewModel SMSSentModel
            CreateMap<SMSSentModel, SMSSent>().ReverseMap();
            #endregion

            #region ForgotPasswordViewModel ForgotPassword
            CreateMap<ForgetPasswordViewModel, ForgetPasswordModel>().ReverseMap();
            #endregion

            #region ResetPasswordViewModel ResetPassword
            CreateMap<ResetPasswordViewModel, ResetPasswordModel>().ReverseMap();
            #endregion


            #region GetRefreshTokenViewModel GetRefreshToken
            CreateMap<GetAccessTokenViewModel, GetAccessToken>().ReverseMap(); // Add this line to handle nested mapping

            CreateMap<GetRefreshTokenViewModel, GetRefreshToken>()
                .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.AccessToken))
                .ReverseMap();
            #endregion

            #region UpdateMasterStatusViewModel UpdateMasterStatus

            CreateMap<UpdateMasterStatusViewModel, UpdateMasterStatus>().ReverseMap();
            #endregion

            #region DeleteStatusViewModel DeleteMasterStatus

            CreateMap<DeleteMasterStatusViewModel, DeleteMasterStatus>().ReverseMap();
            #endregion

            #region StateMasterViewModel and  StateMaster 
            CreateMap<StateMasterViewModel, StateMaster>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateId))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateName))
                .ForMember(dest => dest.SecondLanguage, opt => opt.MapFrom(src => src.SecondLanguage))
                .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => src.StateCode))
                .ForMember(dest => dest.StateStatus, opt => opt.MapFrom(src => src.IsStatus))
                .ForMember(dest => dest.IsGenderCapturedinVoterTurnOut, opt => opt.MapFrom(src => src.IsGenderCapturedinVoterTurnOut))

                .ReverseMap();

            #endregion

            #region AddStateMasterViewModel and  StateMaster 
            CreateMap<AddStateMasterViewModel, StateMaster>()
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateName))
                .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => src.StateCode))

                .ForMember(dest => dest.StateStatus, opt => opt.MapFrom(src => src.IsStatus))
                .ForMember(dest => dest.IsGenderCapturedinVoterTurnOut, opt => opt.MapFrom(src => src.IsGenderCapturedinVoterTurnOut))

                .ReverseMap();

            #endregion

            #region CombinedMaster and DistrictMasterViewModel  

            CreateMap<CombinedMaster, DistrictMasterViewModel>()
               .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateId))
               .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictId))
               .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName))
                 .ForMember(dest => dest.SecondLanguage, opt => opt.MapFrom(src => src.SecondLanguage))
               .ForMember(dest => dest.DistrictCode, opt => opt.MapFrom(src => src.DistrictCode))
               .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.DistrictStatus))

               .ReverseMap();

            #endregion

            #region AssemblyMasterViewModel AssemblyMaster
            //CreateMap<AssemblyMasterViewModel, AssemblyMaster>()
            //    .ReverseMap();
            CreateMap<AssemblyMasterViewModel, AssemblyMaster>()
    .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
    .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
    .ForMember(dest => dest.AssemblyName, opt => opt.MapFrom(src => src.AssemblyName))
    .ForMember(dest => dest.AssemblyCode, opt => opt.MapFrom(src => src.AssemblyCode))
    .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => src.PCMasterId))
    .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))

    .ForMember(dest => dest.AssemblyType, opt => opt.MapFrom(src => src.AssemblyType))
    .ForMember(dest => dest.AssemblyStatus, opt => opt.MapFrom(src => src.IsStatus))
        .ForMember(dest => dest.TotalBooths, opt => opt.MapFrom(src => src.TotalBooths))
     .ReverseMap();


            CreateMap<AddAssemblyMasterViewModel, AssemblyMaster>()
   .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
   .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
   .ForMember(dest => dest.AssemblyName, opt => opt.MapFrom(src => src.AssemblyName))
   .ForMember(dest => dest.AssemblyCode, opt => opt.MapFrom(src => src.AssemblyCode))
   .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => src.PCMasterId))
   .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
   .ForMember(dest => dest.AssemblyType, opt => opt.MapFrom(src => src.AssemblyType))
   .ForMember(dest => dest.AssemblyStatus, opt => opt.MapFrom(src => src.IsStatus))
            .ForMember(dest => dest.TotalBooths, opt => opt.MapFrom(src => src.TotalBooths));
            #endregion

            #region DistrictMasterViewModel and DistrictMaster 
            //CreateMap<DistrictMasterViewModel, DistrictMaster>()
            // .ReverseMap();
            CreateMap<DistrictMasterViewModel, DistrictMaster>()
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
              .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName))
              .ForMember(dest => dest.DistrictCode, opt => opt.MapFrom(src => src.DistrictCode))
              .ForMember(dest => dest.DistrictStatus, opt => opt.MapFrom(src => src.IsStatus))
              .ReverseMap();

            #endregion

            #region AddDistrictMasterViewModel and DistrictMaster 
            //CreateMap<AddDistrictMasterViewModel, DistrictMaster>()
            // .ReverseMap();

            CreateMap<AddDistrictMasterViewModel, DistrictMaster>()
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName))
              .ForMember(dest => dest.DistrictCode, opt => opt.MapFrom(src => src.DistrictCode))
              .ForMember(dest => dest.DistrictStatus, opt => opt.MapFrom(src => src.IsStatus))
              .ReverseMap();
            #endregion

            #region  BoothMasterViewModel and BoothMaster



            CreateMap<BoothMasterViewModel, BoothMaster>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                .ForMember(dest => dest.BoothName, opt => opt.MapFrom(src => src.BoothName))
                .ForMember(dest => dest.BoothCode_No, opt => opt.MapFrom(src => src.BoothCode_No))
                .ForMember(dest => dest.BoothNoAuxy, opt => opt.MapFrom(src => src.BoothNoAuxy))
                .ForMember(dest => dest.BoothStatus, opt => opt.MapFrom(src => src.IsStatus))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.TotalVoters, opt => opt.MapFrom(src => src.TotalVoters))
                  .ForMember(dest => dest.Male, opt => opt.MapFrom(src => src.Male))
                  .ForMember(dest => dest.Female, opt => opt.MapFrom(src => src.Female))
                  .ForMember(dest => dest.Transgender, opt => opt.MapFrom(src => src.Transgender))
                  .ForMember(dest => dest.LocationMasterId, opt => opt.MapFrom(src => src.LocationMasterId))

             .ReverseMap();


            #endregion

            #region BoothMappingViewModel BoothMaster

            CreateMap<BoothViewModel, BoothMaster>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
                 .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.AssignedBy))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo))
                .ForMember(dest => dest.IsAssigned, opt => opt.MapFrom(src => src.IsAssigned))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))


             .ReverseMap();

            CreateMap<BLOBoothViewModel, BoothMaster>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
                 .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.AssignedBy))
                .ForMember(dest => dest.AssignedToBLO, opt => opt.MapFrom(src => src.AssignedTo))
                .ForMember(dest => dest.IsAssigned, opt => opt.MapFrom(src => src.IsAssigned))


             .ReverseMap();
            #endregion

            #region BoothReleaseViewModel BoothMaster

            CreateMap<BoothReleaseViewModel, BoothMaster>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
                .ForMember(dest => dest.IsAssigned, opt => opt.MapFrom(src => src.IsAssigned))


             .ReverseMap();
            #endregion

            #region EventMasterViewModel and EventMaster

            CreateMap<EventMasterViewModel, EventMaster>()
                .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
                .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsStatus))
                .ReverseMap();

            #endregion

            #region UpdateEventStatusViewModel and EventMaster

            CreateMap<UpdateEventStatusViewModel, EventMaster>()
                .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsStatus))
                .ReverseMap();

            #endregion

            #region SectorOfficerViewModel SectorOfficerMaster
            CreateMap<SectorOfficerViewModel, SectorOfficerMaster>()
              .ForMember(dest => dest.SOMasterId, opt => opt.MapFrom(src => src.SoId))
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.SoOfficeName, opt => opt.MapFrom(src => src.SoOfficeName))
              .ForMember(dest => dest.SoName, opt => opt.MapFrom(src => src.SoName))
              .ForMember(dest => dest.SoMobile, opt => opt.MapFrom(src => src.SoMobile))
              .ForMember(dest => dest.SoDesignation, opt => opt.MapFrom(src => src.SoDesignation))
              .ForMember(dest => dest.SoAssemblyCode, opt => opt.MapFrom(src => src.SoAssemblyCode))
              .ForMember(dest => dest.SoStatus, opt => opt.MapFrom(src => src.IsStatus))
              .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
              .ReverseMap();
            #endregion

            #region AddSectorOfficerViewModel SectorOfficerMaster
            CreateMap<AddSectorOfficerViewModel, SectorOfficerMaster>()
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.SoOfficeName, opt => opt.MapFrom(src => src.SoOfficeName))
              .ForMember(dest => dest.SoName, opt => opt.MapFrom(src => src.SoName))
              .ForMember(dest => dest.SoMobile, opt => opt.MapFrom(src => src.SoMobile))
              .ForMember(dest => dest.SoDesignation, opt => opt.MapFrom(src => src.SoDesignation))
              .ForMember(dest => dest.SoAssemblyCode, opt => opt.MapFrom(src => src.SoAssemblyCode))
              .ForMember(dest => dest.SoStatus, opt => opt.MapFrom(src => src.IsStatus))
              .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
              .ReverseMap();
            #endregion

            #region SectorOfficerBoothViewModel CombinedMaster
            CreateMap<SectorOfficerBoothViewModel, CombinedMaster>()
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                  .ForMember(dest => dest.soMasterId, opt => opt.MapFrom(src => src.SoId))
                 .ForMember(dest => dest.BoothName, opt => opt.MapFrom(src => src.BoothName))
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
                .ForMember(dest => dest.IsAssigned, opt => opt.MapFrom(src => src.IsAssigned))
                .ForMember(dest => dest.BoothAuxy, opt => opt.MapFrom(src => src.BoothAuxy))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))


             .ReverseMap();
            #endregion

            #region CombineMasterViewModel CombinedMaster
            CreateMap<CombinedMasterViewModel, CombinedMaster>()
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                 .ForMember(dest => dest.BoothName, opt => opt.MapFrom(src => src.BoothName))
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
                .ForMember(dest => dest.IsAssigned, opt => opt.MapFrom(src => src.IsAssigned))
                .ForMember(dest => dest.BoothAuxy, opt => opt.MapFrom(src => src.BoothAuxy))

                           .ForMember(dest => dest.SecondLanguage, opt => opt.MapFrom(src => src.SecondLanguage))
             .ReverseMap();
            #endregion

            #region ParliamentConstituencyMaster  PCViewModel
            CreateMap<ParliamentConstituencyMaster, PCViewModel>()
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.PcName, opt => opt.MapFrom(src => src.PcName))
                .ForMember(dest => dest.SecondLanguage, opt => opt.MapFrom(src => src.SecondLanguage))
              .ForMember(dest => dest.PcCodeNo, opt => opt.MapFrom(src => src.PcCodeNo))
              .ForMember(dest => dest.PcType, opt => opt.MapFrom(src => src.PcType))
              .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.PcStatus))
              .ReverseMap();


            #endregion

            #region ParliamentConstituencyMaster PCViewModel
            CreateMap<ParliamentConstituencyMaster, PCViewModel>()
           .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
           .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => src.PCMasterId))
           .ForMember(dest => dest.PcName, opt => opt.MapFrom(src => src.PcName))
           .ForMember(dest => dest.PcCodeNo, opt => opt.MapFrom(src => src.PcCodeNo))
           .ForMember(dest => dest.PcType, opt => opt.MapFrom(src => src.PcType))
           .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.PcStatus))
           .ReverseMap();
            #endregion

            #region Event Activity

            CreateMap<ElectionInfoViewModel, ElectionInfoMaster>()
         .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
         .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
         .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
         .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
         .ForMember(dest => dest.IsPartyDispatched, opt => opt.MapFrom(src => src.EventStatus))
         .ForMember(dest => dest.IsPartyReached, opt => opt.MapFrom(src => src.EventStatus))
         .ForMember(dest => dest.FinalTVoteStatus, opt => opt.MapFrom(src => src.EventStatus))
         .ForMember(dest => dest.NoOfPollingAgents, opt => opt.MapFrom(src => src.NoOfPollingAgents))
         .ForMember(dest => dest.Male, opt => opt.MapFrom(src => src.Male))
         .ForMember(dest => dest.Female, opt => opt.MapFrom(src => src.Female))
        .ForMember(dest => dest.Transgender, opt => opt.MapFrom(src => src.Transgender))
        .ForMember(dest => dest.EDC, opt => opt.MapFrom(src => src.EDC))
        .ForMember(dest => dest.IsQueueUndo, opt => opt.MapFrom(src => src.IsQueueUndo))
        .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))

         .ReverseMap();
            #endregion

            #region SlotManagement
            CreateMap<SlotTimeViewModel, SlotManagementMaster>()
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeOnly.Parse(src.StartTime)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.EndTime) ? null : (TimeOnly?)TimeOnly.Parse(src.EndTime)))
            .ForMember(dest => dest.LockTime, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.LockTime) ? null : (TimeOnly?)TimeOnly.Parse(src.LockTime)))
            .ForMember(dest => dest.SlotCreatedTime, opt => opt.MapFrom(src => DateTimeOffset.Now.ToUniversalTime()))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.Parse(src.StartDate))); // Add this line for StartDate mapping

            CreateMap<SlotManagementViewModel, List<SlotManagementMaster>>()
        .ConvertUsing((src, dest, context) =>
        {
            var slotManagements = src.slotTimes
                .Select(slotTime => context.Mapper.Map<SlotManagementMaster>(slotTime))
                .ToList();

            foreach (var slotManagement in slotManagements)
            {
                slotManagement.StateMasterId = src.StateMasterId;
                slotManagement.EventMasterId = src.EventMasterId;
            }

            return slotManagements;
        });
            #endregion



            #region InterruptionViewModel PollInterruption
            CreateMap<InterruptionViewModel, PollInterruption>()
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.boothMasterId))
                .ForMember(dest => dest.InterruptionType, opt => opt.MapFrom(src => src.Reason))

                .ForMember(dest => dest.StopTime, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.stopTime) ? null : (TimeOnly?)TimeOnly.Parse(src.stopTime)))
                 .ForMember(dest => dest.ResumeTime, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ResumeTime) ? null : (TimeOnly?)TimeOnly.Parse(src.ResumeTime)))
                 .ForMember(dest => dest.NewCU, opt => opt.MapFrom(src => src.newCU))
                .ForMember(dest => dest.NewBU, opt => opt.MapFrom(src => src.newBU))
                .ForMember(dest => dest.OldBU, opt => opt.MapFrom(src => src.oldBu))
                .ForMember(dest => dest.OldCU, opt => opt.MapFrom(src => src.oldCu))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
             //.ForMember(dest => dest.IsPollInterrupted, opt => opt.MapFrom(src => src.IsPollInterrupted))
             //.ForMember(dest => dest.Flag, opt => opt.MapFrom(src => src.Flag))


             .ReverseMap();
            #endregion    

            #region AddVoterTurnOutViewModel AddVoterTurnOut
            CreateMap<AddVoterTurnOutViewModel, AddVoterTurnOut>()
                .ForMember(dest => dest.boothMasterId, opt => opt.MapFrom(src => src.boothMasterId))
                .ForMember(dest => dest.voterValue, opt => opt.MapFrom(src => src.voterValue))
                .ForMember(dest => dest.Male, opt => opt.MapFrom(src => src.Male))
                .ForMember(dest => dest.Female, opt => opt.MapFrom(src => src.Female))
                .ForMember(dest => dest.Transgender, opt => opt.MapFrom(src => src.Transgender))
                .ForMember(dest => dest.eventid, opt => opt.MapFrom(src => src.eventid))
             //.ForMember(dest => dest.IsPollInterrupted, opt => opt.MapFrom(src => src.IsPollInterrupted))
             //.ForMember(dest => dest.Flag, opt => opt.MapFrom(src => src.Flag))


             .ReverseMap();
            #endregion


            #region PollingStationViewModel PollingStationMaster

            CreateMap<PollingStationViewModel, PollingStationMaster>()

                .ReverseMap();
            CreateMap<PollingStationGenderViewModel, PollingStationGender>()
               .ReverseMap();
            #endregion

            #region LocationMaster

            #region LocationModel  LocationViewModel
            CreateMap<LocationViewModel, LocationModel>()

                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                  .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => src.PCMasterId))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationName))
                .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.LocationCode))
                .ForMember(dest => dest.LocationLatitude, opt => opt.MapFrom(src => src.LocationLatitude))
                .ForMember(dest => dest.LocationLongitude, opt => opt.MapFrom(src => src.LocationLongitude))
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.IsStatus))

                 .ReverseMap();

            #endregion
            #endregion

            #region BoothReportViewModel BoothReportModel
            CreateMap<BoothReportViewModel, BoothReportModel>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.StateMasterId) ? int.Parse(src.StateMasterId) : 0))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.DistrictMasterId) ? int.Parse(src.DistrictMasterId) : 0))
                .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.PCMasterId) ? int.Parse(src.PCMasterId) : 0))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.AssemblyMasterId) ? int.Parse(src.AssemblyMasterId) : 0))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));


            #endregion

            #region SlotVTReportViewModel SlotVTReportModel
            CreateMap<SlotVTReportViewModel, SlotVTReportModel>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.StateMasterId) ? int.Parse(src.StateMasterId) : 0))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.DistrictMasterId) ? int.Parse(src.DistrictMasterId) : 0))
                .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.PCMasterId) ? int.Parse(src.PCMasterId) : 0))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.AssemblyMasterId) ? int.Parse(src.AssemblyMasterId) : 0))
                .ForMember(dest => dest.SlotMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.SlotMasterId) ? int.Parse(src.SlotMasterId) : 0));



            #endregion

            #region ChartReportViewModel ChartReportModel
            CreateMap<ChartReportViewModel, ChartReportModel>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.StateMasterId) ? int.Parse(src.StateMasterId) : 0))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.DistrictMasterId) ? int.Parse(src.DistrictMasterId) : 0))
                .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.PCMasterId) ? int.Parse(src.PCMasterId) : 0))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.AssemblyMasterId) ? int.Parse(src.AssemblyMasterId) : 0))
                 .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId));


            #endregion

            #region UpdateDashboardViewModel UpdateDashboardProfile
            CreateMap<UpdateDashboardViewModel, UpdateDashboardProfile>()
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNo))

                 .ReverseMap();

            #endregion

            #region SendSMSViewModel SendSMSModel
            CreateMap<SendSMSViewModel, SendSMSModel>()

                  .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
                .ForMember(dest => dest.TemplateMasterId, opt => opt.MapFrom(src => src.TemplateMasterId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId));


            #endregion

            #region HelpDesk

            #region HelpDeskDetail  HelpDeskViewModel
            CreateMap<HelpDeskDetail, HelpDeskViewModel>()
      .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
      .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
      .ForMember(dest => dest.PCMasterId, opt => opt.MapFrom(src => src.PCMasterId))
      .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
      .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
      .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.HelpDeskStatus))
      .ForMember(dest => dest.ContactPersonName, opt => opt.MapFrom(src => src.ContactName))
      .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNumber))
      .ForMember(dest => dest.LandlineNumber, opt => opt.MapFrom(src => src.LandlineNumber.HasValue ? src.LandlineNumber.Value.ToString() : null)) // Handle null values for LandlineNumber

              .ReverseMap();


            #endregion

            #endregion

            #region AddQueue
            CreateMap<QIS, QISViewModel>()
             .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
             .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
             .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
             .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
             .ForMember(dest => dest.BoothCode, opt => opt.MapFrom(src => src.BoothCode))
             .ForMember(dest => dest.BLOMasterId, opt => opt.MapFrom(src => src.BLOMasterId))
             .ForMember<string>(dest => dest.BLOMobileNumber, opt => opt.MapFrom(src => src.BLOMobileNumber))
             .ForMember(dest => dest.QueueCount, opt => opt.MapFrom(src => src.QueueCount))
             .ForMember(dest => dest.QueueEnterTime, opt => opt.MapFrom<string>(src => src.QueueEnterTime))
             .ForMember(dest => dest.QueueEnterDateTime, opt => opt.MapFrom(src => src.QueueEnterDateTime))
             .ReverseMap();
            #endregion

            #region Randomization
            CreateMap<PPRViewModel, PPR>()
       .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
       .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
       .ForMember(dest => dest.RandomizationTaskDetailMasterId, opt => opt.MapFrom(src => src.RandomizationTaskDetailMasterId))
       .ForMember(dest => dest.CurrentRound, opt => opt.MapFrom(src => src.CurrentRound))
       .ForMember(dest => dest.DateOfRound, opt => opt.MapFrom(src => ParseAndConvertToUtc(src.DateOfRound)))
       .ForMember(dest => dest.DateOfPostponedRound, opt => opt.MapFrom(src => ParseAndConvertToUtc(src.DateOfPostponedRound)))
       .ForMember(dest => dest.DateOfCompletedRound, opt => opt.MapFrom(src => ParseAndConvertToUtc(src.DateOfCompletedRound)))
       .ReverseMap();
            CreateMap<PPRViewUpdateModel, PPR>()
     .ForMember(dest => dest.PPRMasterId, opt => opt.MapFrom(src => src.PPRMasterId))
     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
     .ForMember(dest => dest.RandomizationTaskDetailMasterId, opt => opt.MapFrom(src => src.RandomizationTaskDetailMasterId))
     .ForMember(dest => dest.CurrentRound, opt => opt.MapFrom(src => src.CurrentRound))
     .ForMember(dest => dest.DateOfRound, opt => opt.MapFrom(src => ParseAndConvertToUtc(src.DateOfRound)))
     .ForMember(dest => dest.DateOfPostponedRound, opt => opt.MapFrom(src => ParseAndConvertToUtc(src.DateOfPostponedRound)))
     .ForMember(dest => dest.DateOfCompletedRound, opt => opt.MapFrom(src => ParseAndConvertToUtc(src.DateOfCompletedRound)))
     .ReverseMap();


            CreateMap<RandomizationTaskDetailViewModel, RandomizationTaskDetail>()
                  .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
             .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.TaskName))
             .ForMember(dest => dest.NumberOfRound, opt => opt.MapFrom(src => src.NumberOfRound))
                .ReverseMap();
            #endregion

            #region BLOMaster
            CreateMap<BLOMaster, AddBLOViewModel>()
           .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
           .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
           .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
           .ForMember(dest => dest.BLOName, opt => opt.MapFrom(src => src.BLOName))
           .ForMember(dest => dest.BLOMobile, opt => opt.MapFrom(src => src.BLOMobile))
           .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.BLOStatus))
           .ReverseMap();

            CreateMap<BLOMaster, UpdateBLOViewModel>()
             .ForMember(dest => dest.BLOMasterId, opt => opt.MapFrom(src => src.BLOMasterId))
             .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
             .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
             .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
             .ForMember(dest => dest.BLOName, opt => opt.MapFrom(src => src.BLOName))
             .ForMember(dest => dest.BLOMobile, opt => opt.MapFrom(src => src.BLOMobile))
             .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.BLOStatus))
             .ReverseMap();
            #endregion

            #region MobileVersion

            CreateMap<MobileVersion, MobileVersionViewModel>()

         .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
         .ForMember(dest => dest.MobileVersion, opt => opt.MapFrom(src => src.CurrentMobileVersion))

         .ReverseMap();
            #endregion

            #region KYCViewModel KYC
            CreateMap<KycViewModel, Kyc>()
             .ForMember(dest => dest.NominationPdfPath, opt => opt.Ignore())

            .ReverseMap();
            #endregion

            #region AddFourthLevelHViewModel  FourthLevelH 
            CreateMap<AddFourthLevelHViewModel, FourthLevelH>()
                 .ForMember(dest => dest.HierarchyName, opt => opt.MapFrom(src => src.HierarchyName))
                 .ForMember(dest => dest.HierarchyCode, opt => opt.MapFrom(src => src.HierarchyCode))
                 .ForMember(dest => dest.HierarchyType, opt => opt.MapFrom(src => src.HierarchyType))
                 .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                 .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                 .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                 .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                 .ForMember(dest => dest.HierarchyCreatedAt, opt => opt.MapFrom(src => src.HierarchyCreatedAt))
                 .ForMember(dest => dest.HierarchyUpdatedAt, opt => opt.MapFrom(src => src.HierarchyUpdatedAt))
                 .ForMember(dest => dest.HierarchyDeletedAt, opt => opt.MapFrom(src => src.HierarchyDeletedAt))
                 .ForMember(dest => dest.HierarchyStatus, opt => opt.MapFrom(src => src.IsStatus))
                 .ReverseMap();

            CreateMap<UpdateFourthLevelHViewModel, FourthLevelH>()
                .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
               .ForMember(dest => dest.HierarchyName, opt => opt.MapFrom(src => src.HierarchyName))
                 .ForMember(dest => dest.HierarchyCode, opt => opt.MapFrom(src => src.HierarchyCode))
                 .ForMember(dest => dest.HierarchyType, opt => opt.MapFrom(src => src.HierarchyType))
                 .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                 .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                 .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                 .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                 .ForMember(dest => dest.HierarchyCreatedAt, opt => opt.MapFrom(src => src.HierarchyCreatedAt))
                 .ForMember(dest => dest.HierarchyUpdatedAt, opt => opt.MapFrom(src => src.HierarchyUpdatedAt))
                 .ForMember(dest => dest.HierarchyDeletedAt, opt => opt.MapFrom(src => src.HierarchyDeletedAt))
                 .ForMember(dest => dest.HierarchyStatus, opt => opt.MapFrom(src => src.IsStatus))

                .ReverseMap();
            #endregion

            #region BlockPanchayatViewModel BlockPanchayat
            CreateMap<AddBlockPanchayatViewModel, BlockZonePanchayat>()
                     .ForMember(dest => dest.BlockZonePanchayatName, opt => opt.MapFrom(src => src.BlockZonePanchayatName))
                     .ForMember(dest => dest.BlockZonePanchayatCode, opt => opt.MapFrom(src => src.BlockZonePanchayatCode))
                     .ForMember(dest => dest.BlockZonePanchayatType, opt => opt.MapFrom(src => src.BlockZonePanchayatType))
                     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                     .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                     .ForMember(dest => dest.BlockZonePanchayatBooths, opt => opt.MapFrom(src => src.BlockZonePanchayatBooths))
                     .ForMember(dest => dest.BlockZonePanchayatCategory, opt => opt.MapFrom(src => src.BlockZonePanchayatCategory))
                     .ForMember(dest => dest.BlockZonePanchayatCreatedAt, opt => opt.MapFrom(src => src.BlockZonePanchayatCreatedAt))
                     .ForMember(dest => dest.BlockZonePanchayatUpdatedAt, opt => opt.MapFrom(src => src.BlockZonePanchayatUpdatedAt))
                     .ForMember(dest => dest.BlockZonePanchayatDeletedAt, opt => opt.MapFrom(src => src.BlockZonePanchayatDeletedAt))
                     .ForMember(dest => dest.BlockZonePanchayatStatus, opt => opt.MapFrom(src => src.IsStatus))
     .ReverseMap();

            CreateMap<UpdateBlockPanchayatViewModel, BlockZonePanchayat>()
                     .ForMember(dest => dest.BlockZonePanchayatMasterId, opt => opt.MapFrom(src => src.BlockZonePanchayatMasterId))
                     .ForMember(dest => dest.BlockZonePanchayatName, opt => opt.MapFrom(src => src.BlockZonePanchayatName))
                     .ForMember(dest => dest.BlockZonePanchayatCode, opt => opt.MapFrom(src => src.BlockZonePanchayatCode))
                     .ForMember(dest => dest.BlockZonePanchayatType, opt => opt.MapFrom(src => src.BlockZonePanchayatType))
                     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                     .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                     .ForMember(dest => dest.BlockZonePanchayatBooths, opt => opt.MapFrom(src => src.BlockZonePanchayatBooths))
                     .ForMember(dest => dest.BlockZonePanchayatCategory, opt => opt.MapFrom(src => src.BlockZonePanchayatCategory))
                     .ForMember(dest => dest.BlockZonePanchayatCreatedAt, opt => opt.MapFrom(src => src.BlockZonePanchayatCreatedAt))
                     .ForMember(dest => dest.BlockZonePanchayatUpdatedAt, opt => opt.MapFrom(src => src.BlockZonePanchayatUpdatedAt))
                     .ForMember(dest => dest.BlockZonePanchayatDeletedAt, opt => opt.MapFrom(src => src.BlockZonePanchayatDeletedAt))
                     .ForMember(dest => dest.BlockZonePanchayatStatus, opt => opt.MapFrom(src => src.IsStatus))

                .ReverseMap();
            #endregion

            #region AddSarpanchWardsViewModel
            CreateMap<AddSarpanchWardsViewModel, SarpanchWards>()
                     .ForMember(dest => dest.SarpanchWardsName, opt => opt.MapFrom(src => src.SarpanchWardsName))
                     .ForMember(dest => dest.SarpanchWardsCode, opt => opt.MapFrom(src => src.SarpanchWardsCode))
                     .ForMember(dest => dest.SarpanchWardsType, opt => opt.MapFrom(src => src.SarpanchWardsType))
                     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                     .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
 
                     .ForMember(dest => dest.SarpanchWardsCategory, opt => opt.MapFrom(src => src.SarpanchWardsCategory))
                     .ForMember(dest => dest.SarpanchWardsCreatedAt, opt => opt.MapFrom(src => src.SarpanchWardsCreatedAt))
                     .ForMember(dest => dest.SarpanchWardsUpdatedAt, opt => opt.MapFrom(src => src.SarpanchWardsUpdatedAt))
                     .ForMember(dest => dest.SarpanchWardsDeletedAt, opt => opt.MapFrom(src => src.SarpanchWardsDeletedAt))
                     .ForMember(dest => dest.SarpanchWardsStatus, opt => opt.MapFrom(src => src.IsStatus))

            .ReverseMap();
            CreateMap<UpdateSarpanchWardsViewModel, SarpanchWards>()
                     .ForMember(dest => dest.SarpanchWardsMasterId, opt => opt.MapFrom(src => src.SarpanchWardsMasterId))
                     .ForMember(dest => dest.SarpanchWardsName, opt => opt.MapFrom(src => src.SarpanchWardsName))
                     .ForMember(dest => dest.SarpanchWardsCode, opt => opt.MapFrom(src => src.SarpanchWardsCode))
                     .ForMember(dest => dest.SarpanchWardsType, opt => opt.MapFrom(src => src.SarpanchWardsType))
                     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                     .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
 
                     .ForMember(dest => dest.SarpanchWardsCategory, opt => opt.MapFrom(src => src.SarpanchWardsCategory))
                     .ForMember(dest => dest.SarpanchWardsCreatedAt, opt => opt.MapFrom(src => src.SarpanchWardsCreatedAt))
                     .ForMember(dest => dest.SarpanchWardsUpdatedAt, opt => opt.MapFrom(src => src.SarpanchWardsUpdatedAt))
                     .ForMember(dest => dest.SarpanchWardsDeletedAt, opt => opt.MapFrom(src => src.SarpanchWardsDeletedAt))
                     .ForMember(dest => dest.SarpanchWardsStatus, opt => opt.MapFrom(src => src.IsStatus))

            .ReverseMap();
            #endregion


        }
        #region Convert DateTime UTC
        private DateTime? ParseAndConvertToUtc(string dateTimeString)
        {
            if (string.IsNullOrEmpty(dateTimeString))
            {
                return null;
            }

            DateTime parsedDateTime = DateTime.Parse(dateTimeString);
            return parsedDateTime.ToUniversalTime();
        }

        #endregion
    }
}
