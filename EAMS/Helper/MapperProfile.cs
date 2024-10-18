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
using EAMS_ACore.Models.EventActivityModels;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.Models.QueueModel;
using EAMS_ACore.NotificationModels;
using EAMS_ACore.ReportModels;
using LBPAMS.ViewModels;
using LBPAMS.ViewModels.EventActivityViewModels;
using LBPAMS.ViewModels.PublicModels;
using LBPAMS.ViewModels.ReportViewModel;

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
            .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
            .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
            .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
            .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));


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
                .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                .ForMember(dest => dest.PSZonePanchayatMasterId, opt => opt.MapFrom(src => src.PSZonePanchayatMasterId))
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
                  .ForMember(dest => dest.IsPrimaryBooth, opt => opt.MapFrom(src => src.IsPrimaryBooth))
             //.ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))

             .ReverseMap();
            CreateMap<UpdateBoothMasterViewModel, BoothMaster>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                .ForMember(dest => dest.PSZonePanchayatMasterId, opt => opt.MapFrom(src => src.PSZonePanchayatMasterId))
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
                  .ForMember(dest => dest.IsPrimaryBooth, opt => opt.MapFrom(src => src.IsPrimaryBooth))
             .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))

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
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
                .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
                .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsStatus))
                .ReverseMap();
            CreateMap<EventMasterListViewModel, EventMaster>()
               .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
               .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
               .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
               .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
               .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
               .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsStatus))
               .ReverseMap();
            CreateMap<EventMasterByIdViewModel, EventMaster>()
     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
     .ForPath(dest => dest.StateMaster.StateName, opt => opt.MapFrom(src => src.StateName)) // Use ForPath for nested property
     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
     .ForPath(dest => dest.ElectionTypeMaster.ElectionType, opt => opt.MapFrom(src => src.ElectionTypeName)) // Use ForPath for nested property
     .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
     .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
     .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
     .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsStatus))
     .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => src.StartDateTime))
     .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => src.EndDateTime))
     .ReverseMap();

            CreateMap<UpdateEventMasterViewModel, EventMaster>()

                .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
                .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
                .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsStatus))
                .ReverseMap();
            #endregion

            #region UpdateEventStatusViewModel and EventMaster

            CreateMap<UpdateEventStatusViewModel, EventMaster>()
                .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsStatus))
                .ReverseMap();

            #endregion

            #region FieldOfficerViewModel FieldOfficerMaster
            CreateMap<UpdateFieldOfficerViewModel, FieldOfficerMaster>()
              .ForMember(dest => dest.FieldOfficerMasterId, opt => opt.MapFrom(src => src.FieldOfficerMasterId))
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
              .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
              .ForMember(dest => dest.FieldOfficerName, opt => opt.MapFrom(src => src.FieldOfficerName))
              .ForMember(dest => dest.FieldOfficerOfficeName, opt => opt.MapFrom(src => src.FieldOfficerOfficeName))
              .ForMember(dest => dest.FieldOfficerMobile, opt => opt.MapFrom(src => src.FieldOfficerMobile))
              .ForMember(dest => dest.FieldOfficerDesignation, opt => opt.MapFrom(src => src.FieldOfficerDesignation))
              .ForMember(dest => dest.FieldOfficerStatus, opt => opt.MapFrom(src => src.IsStatus))
              .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
              .ReverseMap();
            #endregion

            #region FieldOfficerViewModel FieldOfficerMaster
            CreateMap<FieldOfficerViewModel, FieldOfficerMaster>()
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
              .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
              .ForMember(dest => dest.FieldOfficerName, opt => opt.MapFrom(src => src.FieldOfficerName))
              .ForMember(dest => dest.FieldOfficerOfficeName, opt => opt.MapFrom(src => src.FieldOfficerOfficeName))
              .ForMember(dest => dest.FieldOfficerMobile, opt => opt.MapFrom(src => src.FieldOfficerMobile))
              .ForMember(dest => dest.FieldOfficerDesignation, opt => opt.MapFrom(src => src.FieldOfficerDesignation))
              .ForMember(dest => dest.FieldOfficerStatus, opt => opt.MapFrom(src => src.IsStatus))
              .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
              .ReverseMap();
            #endregion
            #region AROResultMasterViewModel AROResultMaster
            CreateMap<AROResultMasterViewModel, AROResultMaster>()
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
              .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
              .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
              .ForMember(dest => dest.AROName, opt => opt.MapFrom(src => src.AROName))
              .ForMember(dest => dest.AROOfficeName, opt => opt.MapFrom(src => src.AROOfficeName))
              .ForMember(dest => dest.AROMobile, opt => opt.MapFrom(src => src.AROMobile))
              .ForMember(dest => dest.ARODesignation, opt => opt.MapFrom(src => src.ARODesignation))
              .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.IsStatus))
              .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
              .ReverseMap();
            #endregion

            #region UpdateAROResultViewModel AROResultMaster
            CreateMap<UpdateAROResultViewModel, AROResultMaster>()
              .ForMember(dest => dest.AROMasterId, opt => opt.MapFrom(src => src.AROMasterId))
              .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
              .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
              .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
              .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
              .ForMember(dest => dest.AROName, opt => opt.MapFrom(src => src.AROName))
              .ForMember(dest => dest.AROOfficeName, opt => opt.MapFrom(src => src.AROOfficeName))
              .ForMember(dest => dest.AROMobile, opt => opt.MapFrom(src => src.AROMobile))
              .ForMember(dest => dest.ARODesignation, opt => opt.MapFrom(src => src.ARODesignation))
              .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.IsStatus))
              .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
              .ReverseMap();
            #endregion
            #region SectorOfficerBoothViewModel CombinedMaster
            CreateMap<FieldOfficerBoothViewModel, CombinedMaster>()
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                  .ForMember(dest => dest.FieldOfficerMasterId, opt => opt.MapFrom(src => src.FieldOfficerMasterId))
                 .ForMember(dest => dest.BoothName, opt => opt.MapFrom(src => src.BoothName))
                 .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                 .ForMember(dest => dest.FourthLevelHTotalVoters, opt => opt.MapFrom(src => src.FourthLevelHTotalVoters))
                 .ForMember(dest => dest.FourthLevelHName, opt => opt.MapFrom(src => src.FourthLevelHName))
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
                .ForMember(dest => dest.IsAssigned, opt => opt.MapFrom(src => src.IsAssigned))
                .ForMember(dest => dest.BoothAuxy, opt => opt.MapFrom(src => src.BoothAuxy))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
                .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
                .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
                .ForMember(dest => dest.EventStatus, opt => opt.MapFrom(src => src.EventStatus))

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
                .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                .ForMember(dest => dest.FourthLevelHName, opt => opt.MapFrom(src => src.FourthLevelHName))
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

            CreateMap<UpdateEventActivityViewModel, UpdateEventActivity>()
         .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
         .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
         .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
         .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
         .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
         .ForMember(dest => dest.EventStatus, opt => opt.MapFrom(src => src.EventStatus))
         .ForMember(dest => dest.NoOfPollingAgents, opt => opt.MapFrom(src => src.NoOfPollingAgents))
         .ForMember(dest => dest.VoterInQueue, opt => opt.MapFrom(src => src.VoterInQueue))
          .ReverseMap();

            CreateMap<UpdateVTEventActivityViewModel, UpdateEventActivity>()
            .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
            .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
            .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
            .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
            .ForMember(dest => dest.EventStatus, opt => opt.MapFrom(src => src.EventStatus))
            .ForMember(dest => dest.VotesPolled, opt => opt.MapFrom(src => src.VotesPolled))


            .ReverseMap();
            CreateMap<UpdateFinalVoteEventActivityViewModel, UpdateEventActivity>()
        .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
        .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
        .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
        .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
        .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
        .ForMember(dest => dest.EventStatus, opt => opt.MapFrom(src => src.EventStatus))
        .ForMember(dest => dest.FinalMaleVotes, opt => opt.MapFrom(src => src.FinalMaleVotes))
        .ForMember(dest => dest.FinalFeMaleVotes, opt => opt.MapFrom(src => src.FinalFeMaleVotes))
        .ForMember(dest => dest.FinalTransgenderVotes, opt => opt.MapFrom(src => src.FinalTransgenderVotes))

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
                                slotManagement.ElectionTypeMasterId = src.ElectionTypeMasterId;
                                slotManagement.EventMasterId = src.EventMasterId;
                            }

                            return slotManagements;
                        });
            #endregion


            #region InterruptionViewModel PollInterruption
            CreateMap<InterruptionViewModel, PollInterruption>()
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.boothMasterId))
    .ForMember(dest => dest.InterruptionType, opt => opt.MapFrom(src => src.InterruptionType))
    .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
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
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                .ForMember(dest => dest.PSZonePanchayatMasterId, opt => opt.MapFrom(src => src.PSZonePanchayatMasterId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));


            #endregion

            #region GPWardReportViewModel BoothReportModel
            CreateMap<GPWardReportViewModel, BoothReportModel>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId));


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


            CreateMap<UpdateKycViewModel, Kyc>()
                 .ForMember(dest => dest.NominationPdfPath, opt => opt.MapFrom(src => src.NominationPdf))

                .ReverseMap();
            #endregion

            #region UnOpposedViewModel UnOpposed
            CreateMap<UnOpposedViewModel, UnOpposed>()
             .ForMember(dest => dest.NominationPdfPath, opt => opt.Ignore())

            .ReverseMap();
            CreateMap<UpdateUnOpposedViewModel, UnOpposed>()
                .ForMember(dest => dest.NominationPdfPath, opt => opt.MapFrom(src => src.NominationPdf))

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
                 .ForMember(dest => dest.IsCC, opt => opt.MapFrom(src => src.IsCC))
                 .ForMember(dest => dest.IsNN, opt => opt.MapFrom(src => src.IsNN))
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
                 .ForMember(dest => dest.IsCC, opt => opt.MapFrom(src => src.IsCC))
                 .ForMember(dest => dest.IsNN, opt => opt.MapFrom(src => src.IsNN))
                .ReverseMap();
            CreateMap<FourthLevelH, ListFourthLevelHViewModel>()
                   .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                   .ForMember(dest => dest.HierarchyName, opt => opt.MapFrom(src => src.HierarchyName))
                   .ForMember(dest => dest.HierarchyCode, opt => opt.MapFrom(src => src.HierarchyCode))
                   .ForMember(dest => dest.HierarchyType, opt => opt.MapFrom(src => src.HierarchyType))
                   .ForMember(dest => dest.HierarchyCategory, opt => opt.MapFrom(src => src.HierarchyType))
                   .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                   .ForMember(dest => dest.ElectionTypeName, opt => opt.MapFrom(src => src.ElectionTypeMaster.ElectionType))
                   .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                   .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateMaster.StateName))
                   .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                   .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictMaster.DistrictName))
                   .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                   .ForMember(dest => dest.AssemblyName, opt => opt.MapFrom(src => src.AssemblyMaster.AssemblyName))
                .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.HierarchyStatus))
                .ForMember(dest => dest.IsCC, opt => opt.MapFrom(src => src.IsCC))
                 .ForMember(dest => dest.IsNN, opt => opt.MapFrom(src => src.IsNN))
               .ReverseMap();
            #endregion

            #region PSZonePanchayatViewModel PSZonePanchayat
            CreateMap<PSZonePanchayatViewModel, PSZonePanchayat>()
                     .ForMember(dest => dest.PSZonePanchayatName, opt => opt.MapFrom(src => src.PSZonePanchayatName))
                     .ForMember(dest => dest.PSZonePanchayatCode, opt => opt.MapFrom(src => src.PSZonePanchayatCode))
                     .ForMember(dest => dest.PSZonePanchayatType, opt => opt.MapFrom(src => src.PSZonePanchayatType))
                     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                     .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                     .ForMember(dest => dest.PSZonePanchayatBooths, opt => opt.MapFrom(src => src.PSZonePanchayatBooths))
                     .ForMember(dest => dest.PSZonePanchayatCategory, opt => opt.MapFrom(src => src.PSZonePanchayatCategory))
                     .ForMember(dest => dest.PSZonePanchayatCreatedAt, opt => opt.MapFrom(src => src.PSZonePanchayatCreatedAt))
                     .ForMember(dest => dest.PSZonePanchayatUpdatedAt, opt => opt.MapFrom(src => src.PSZonePanchayatUpdatedAt))
                     .ForMember(dest => dest.PSZonePanchayatDeletedAt, opt => opt.MapFrom(src => src.PSZonePanchayatDeletedAt))
                     .ForMember(dest => dest.PSZonePanchayatStatus, opt => opt.MapFrom(src => src.IsStatus))
     .ReverseMap();

            CreateMap<UpdatePSZonePanchayatViewModel, PSZonePanchayat>()
                     .ForMember(dest => dest.PSZonePanchayatMasterId, opt => opt.MapFrom(src => src.PSZonePanchayatMasterId))
                     .ForMember(dest => dest.PSZonePanchayatName, opt => opt.MapFrom(src => src.PSZonePanchayatName))
                     .ForMember(dest => dest.PSZonePanchayatCode, opt => opt.MapFrom(src => src.PSZonePanchayatCode))
                     .ForMember(dest => dest.PSZonePanchayatType, opt => opt.MapFrom(src => src.PSZonePanchayatType))
                     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                     .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                     .ForMember(dest => dest.PSZonePanchayatBooths, opt => opt.MapFrom(src => src.PSZonePanchayatBooths))
                     .ForMember(dest => dest.PSZonePanchayatCategory, opt => opt.MapFrom(src => src.PSZonePanchayatCategory))
                     .ForMember(dest => dest.PSZonePanchayatStatus, opt => opt.MapFrom(src => src.IsStatus))

                .ReverseMap();

            CreateMap<PSZonePanchayat, ListPSZonePanchayatViewModel>()
                    .ForMember(dest => dest.PSZonePanchayatMasterId, opt => opt.MapFrom(src => src.PSZonePanchayatMasterId))
                    .ForMember(dest => dest.PSZonePanchayatName, opt => opt.MapFrom(src => src.PSZonePanchayatName))
                    .ForMember(dest => dest.PSZonePanchayatCode, opt => opt.MapFrom(src => src.PSZonePanchayatCode))
                    .ForMember(dest => dest.PSZonePanchayatType, opt => opt.MapFrom(src => src.PSZonePanchayatType))
                    .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                   .ForMember(dest => dest.ElectionTypeName, opt => opt.MapFrom(src => src.ElectionTypeMaster.ElectionType))
                   .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                   .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateMaster.StateName))
                   .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                   .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictMaster.DistrictName))
                   .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                   .ForMember(dest => dest.AssemblyName, opt => opt.MapFrom(src => src.AssemblyMaster.AssemblyName))
                   .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                   .ForMember(dest => dest.FourthLevelHName, opt => opt.MapFrom(src => src.FourthLevelH.HierarchyName))
                    .ForMember(dest => dest.PSZonePanchayatBooths, opt => opt.MapFrom(src => src.PSZonePanchayatBooths))
                    .ForMember(dest => dest.PSZonePanchayatCategory, opt => opt.MapFrom(src => src.PSZonePanchayatCategory))
                    .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.PSZonePanchayatStatus))

               .ReverseMap();
            #endregion

            #region AddGPPanchayatWardsViewModel GPPanchayatWards
            CreateMap<AddGPPanchayatWardsViewModel, GPPanchayatWards>()
                     .ForMember(dest => dest.GPPanchayatWardsName, opt => opt.MapFrom(src => src.GPPanchayatWardsName))
                     .ForMember(dest => dest.GPPanchayatWardsCode, opt => opt.MapFrom(src => src.GPPanchayatWardsCode))
                     .ForMember(dest => dest.GPPanchayatWardsType, opt => opt.MapFrom(src => src.GPPanchayatWardsType))
                     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                     .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                     .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                     .ForMember(dest => dest.GPPanchayatWardsCategory, opt => opt.MapFrom(src => src.GPPanchayatWardsCategory))
                     .ForMember(dest => dest.GPPanchayatWardsStatus, opt => opt.MapFrom(src => src.IsStatus))
                     .ForMember(dest => dest.IsCC, opt => opt.MapFrom(src => src.IsCC))
                     .ForMember(dest => dest.IsNN, opt => opt.MapFrom(src => src.IsNN))
            .ReverseMap();

            CreateMap<UpdateGPPanchayatWardsViewModel, GPPanchayatWards>()
                     .ForMember(dest => dest.GPPanchayatWardsMasterId, opt => opt.MapFrom(src => src.GPPanchayatWardsMasterId))
                     .ForMember(dest => dest.GPPanchayatWardsName, opt => opt.MapFrom(src => src.GPPanchayatWardsName))
                     .ForMember(dest => dest.GPPanchayatWardsCode, opt => opt.MapFrom(src => src.GPPanchayatWardsCode))
                     .ForMember(dest => dest.GPPanchayatWardsType, opt => opt.MapFrom(src => src.GPPanchayatWardsType))
                     .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                     .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                     .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                     .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                     .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                     .ForMember(dest => dest.GPPanchayatWardsCategory, opt => opt.MapFrom(src => src.GPPanchayatWardsCategory))
                     .ForMember(dest => dest.GPPanchayatWardsStatus, opt => opt.MapFrom(src => src.IsStatus))
                     .ForMember(dest => dest.IsCC, opt => opt.MapFrom(src => src.IsCC))
                     .ForMember(dest => dest.IsNN, opt => opt.MapFrom(src => src.IsNN))
            .ReverseMap();
            CreateMap<GPPanchayatWards, ListGPPanchayatWardsViewModel>()
                   .ForMember(dest => dest.GPPanchayatWardsMasterId, opt => opt.MapFrom(src => src.GPPanchayatWardsMasterId))
                   .ForMember(dest => dest.GPPanchayatWardsName, opt => opt.MapFrom(src => src.GPPanchayatWardsName))
                   .ForMember(dest => dest.GPPanchayatWardsCode, opt => opt.MapFrom(src => src.GPPanchayatWardsCode))
                   .ForMember(dest => dest.GPPanchayatWardsType, opt => opt.MapFrom(src => src.GPPanchayatWardsType))
                   .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                   .ForMember(dest => dest.ElectionTypeName, opt => opt.MapFrom(src => src.ElectionTypeMaster.ElectionType))
                   .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                   .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateMaster.StateName))
                   .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                   .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictMaster.DistrictName))
                   .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                   .ForMember(dest => dest.AssemblyName, opt => opt.MapFrom(src => src.AssemblyMaster.AssemblyName))
                   .ForMember(dest => dest.GPPanchayatWardsCategory, opt => opt.MapFrom(src => src.GPPanchayatWardsCategory))
                   .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.GPPanchayatWardsStatus))
                   .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                   .ForMember(dest => dest.FourthLevelHName, opt => opt.MapFrom(src => src.FourthLevelH.HierarchyName))
                   .ForMember(dest => dest.IsCC, opt => opt.MapFrom(src => src.IsCC))
                   .ForMember(dest => dest.IsNN, opt => opt.MapFrom(src => src.IsNN))
          .ReverseMap();

            CreateMap<GPPanchayatWards, GPPanchListViewModel>()
                  .ForMember(dest => dest.GPPanchayatWardsMasterId, opt => opt.MapFrom(src => src.GPPanchayatWardsMasterId))
                  .ForMember(dest => dest.GPPanchayatWardsName, opt => opt.MapFrom(src => src.GPPanchayatWardsName))
                  .ForMember(dest => dest.GPPanchayatWardsCode, opt => opt.MapFrom(src => src.GPPanchayatWardsCode))
         .ReverseMap();
            #endregion

            #region GPVoterViewModel GPVoter

            CreateMap<GPVoterViewModel, GPVoter>()
             .ForMember(dest => dest.GPVoterPdfPath, opt => opt.Ignore())
            .ReverseMap();

            CreateMap<UpdateGPVoterViewModel, GPVoter>()
            .ForMember(dest => dest.GPVoterPdfPath, opt => opt.MapFrom(src => src.GPVoterPdf))

           .ReverseMap();
            #endregion

            #region ResultDeclarationViewModel  ResultDeclaration 
            CreateMap<ResultDeclarationListViewModel, ResultDeclaration>()
     .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
     .ForMember(dest => dest.KycMasterId, opt => opt.MapFrom(src => src.KycMasterId))
     .ForMember(dest => dest.GPPanchayatWardsMasterId, opt => opt.MapFrom(src => src.GPPanchayatWardsMasterId))
     .ForMember(dest => dest.VoteMargin, opt => opt.MapFrom(src => src.VoteMargin))
     .ForMember(dest => dest.IsWinner, opt => opt.MapFrom(src => src.IsWinner))
     .ForMember(dest => dest.IsReCounting, opt => opt.MapFrom(src => src.IsReCounting))
     .ForMember(dest => dest.IsDraw, opt => opt.MapFrom(src => src.IsDraw))
     .ForMember(dest => dest.IsDrawLottery, opt => opt.MapFrom(src => src.IsDrawLottery))
     .ReverseMap();

            CreateMap<UpdateResultDeclarationViewModel, ResultDeclaration>()
     .ForMember(dest => dest.ResultDeclarationMasterId, opt => opt.MapFrom(src => src.ResultDeclarationMasterId))
     .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
     .ForMember(dest => dest.KycMasterId, opt => opt.MapFrom(src => src.KycMasterId))
     .ForMember(dest => dest.GPPanchayatWardsMasterId, opt => opt.MapFrom(src => src.GPPanchayatWardsMasterId))
     .ForMember(dest => dest.VoteMargin, opt => opt.MapFrom(src => src.VoteMargin))
     .ForMember(dest => dest.IsWinner, opt => opt.MapFrom(src => src.IsWinner))
     .ForMember(dest => dest.IsReCounting, opt => opt.MapFrom(src => src.IsReCounting))
     .ForMember(dest => dest.IsDraw, opt => opt.MapFrom(src => src.IsDraw))
     .ForMember(dest => dest.IsDrawLottery, opt => opt.MapFrom(src => src.IsDrawLottery))
     .ReverseMap();

            CreateMap<ResultDeclarationReportListViewModel, ResultDeclaration>()
    .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
    .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
    .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
    .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
    .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
    .ReverseMap();     
            CreateMap<ResultDeclarationReportListViewModel, ResultDeclarationReportListModel>()
    .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
    .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
    .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
    .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
    .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
    .ReverseMap();
            #endregion

            #region UpdateResultDeclarationViewModel  ResultDeclaration
            CreateMap<UpdateResultDeclarationViewModel, ResultDeclaration>()
                 .ForMember(dest => dest.ResultDeclarationMasterId, opt => opt.MapFrom(src => src.ResultDeclarationMasterId))
                 .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                 .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                 .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                 .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                 .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                 .ForMember(dest => dest.GPPanchayatWardsMasterId, opt => opt.MapFrom(src => src.GPPanchayatWardsMasterId))
                 //.ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.CandidateName))
                 //.ForMember(dest => dest.FatherName, opt => opt.MapFrom(src => src.FatherName))
                 .ForMember(dest => dest.VoteMargin, opt => opt.MapFrom(src => src.VoteMargin))
                 .ForMember(dest => dest.ResultDecStatus, opt => opt.MapFrom(src => src.ResultDecStatus))
                 .ReverseMap();
            #endregion

            #region EventByBoothViewModel  EventByBooth
            CreateMap<EventByBoothViewModel, EventByBooth>()
                .ForMember(dest => dest.BoothMasterId, opt => opt.MapFrom(src => src.BoothMasterId))
                .ForMember(dest => dest.EventMasterId, opt => opt.MapFrom(src => src.EventMasterId))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
                .ForMember(dest => dest.EventSequence, opt => opt.MapFrom(src => src.EventSequence))
                .ForMember(dest => dest.EventABBR, opt => opt.MapFrom(src => src.EventABBR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();
            #endregion

            #region PanchayatReleaseViewModel  FourthLevel
            CreateMap<PanchayatReleaseViewModel, FourthLevelH>()
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                .ForMember(dest => dest.AssginedType, opt => opt.MapFrom(src => src.AssginedType))
                .ReverseMap();
            #endregion

            #region ResultListViewModel ResultModel
            CreateMap<ResultListViewModel, AROResultMasterList>()
                .ForMember(dest => dest.AROMasterId, opt => opt.MapFrom(src => src.AROMasterId))
                .ForMember(dest => dest.StateMasterId, opt => opt.MapFrom(src => src.StateMasterId))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateName))
                .ForMember(dest => dest.DistrictMasterId, opt => opt.MapFrom(src => src.DistrictMasterId))
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName))
                .ForMember(dest => dest.AssemblyMasterId, opt => opt.MapFrom(src => src.AssemblyMasterId))
                .ForMember(dest => dest.AssemblyName, opt => opt.MapFrom(src => src.AssemblyName))
                .ForMember(dest => dest.FourthLevelHMasterId, opt => opt.MapFrom(src => src.FourthLevelHMasterId))
                .ForMember(dest => dest.HierarchyName, opt => opt.MapFrom(src => src.FourthLevelHName))
                .ForMember(dest => dest.AssemblyCode, opt => opt.MapFrom(src => src.AssemblyCode))
                .ForMember(dest => dest.AROName, opt => opt.MapFrom(src => src.AROName))
                .ForMember(dest => dest.ARODesignation, opt => opt.MapFrom(src => src.ARODesignation))
                .ForMember(dest => dest.AROOfficeName, opt => opt.MapFrom(src => src.AROOfficeName))
                .ForMember(dest => dest.AROMobile, opt => opt.MapFrom(src => src.AROMobile))
                .ForMember(dest => dest.IsStatus, opt => opt.MapFrom(src => src.IsStatus))
                .ForMember(dest => dest.OTPGeneratedTime, opt => opt.MapFrom(src => src.OTPGeneratedTime))
                .ForMember(dest => dest.OTP, opt => opt.MapFrom(src => src.OTP))
                .ForMember(dest => dest.OTPExpireTime, opt => opt.MapFrom(src => src.OTPExpireTime))
                .ForMember(dest => dest.OTPAttempts, opt => opt.MapFrom(src => src.OTPAttempts))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
                .ForMember(dest => dest.ElectionTypeMasterId, opt => opt.MapFrom(src => src.ElectionTypeMasterId))
                .ForMember(dest => dest.ElectionTypeName, opt => opt.MapFrom(src => src.ElectionTypeName))
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
