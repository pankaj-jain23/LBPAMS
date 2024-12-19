using EAMS.Helper;
using EAMS.ViewModels.PSFormViewModel;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.CommonModels;
using EAMS_ACore.Models.ElectionType;
using EAMS_ACore.Models.EventActivityModels;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.Models.QueueModel;
using EAMS_ACore.Models.ResultModels;
using EAMS_ACore.ReportModels;
using EAMS_ACore.SignalRModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EAMS_ACore.IRepository
{
    public interface IEamsRepository
    {
        #region UpdateMasterStatus
        Task<ServiceResponse> UpdateMasterStatus(UpdateMasterStatus updateMasterStatus);
        Task<ServiceResponse> DeleteMasterStatus(DeleteMasterStatus updateMasterStatus);
        #endregion

        Task<Response> ResetAccounts(string stateMasterId);

        #region Clear Mappings
        Task<ServiceResponse> IsClearBLOMappings(int stateMasterId, int electionTypeMasterId);
        Task<ServiceResponse> IsClearSOMappings(int stateMasterId, int electionTypeMasterId);
        Task<ServiceResponse> IsClearPollDetails(int stateMasterId, int electionTypeMasterId);
        Task<ServiceResponse> IsClearElectionInfo(int stateMasterId, int electionTypeMasterId);
        Task<ServiceResponse> IsClearSlotInfo(int stateMasterId, int electionTypeMasterId,int eventMasterId);
        #endregion

        #region State Master
        Task<List<StateMaster>> GetState();
        Task<Response> UpdateStateById(StateMaster stateMaster);
        Task<Response> AddState(StateMaster stateMaster);
        Task<StateMaster> GetStateById(string Id);

        #endregion

        #region District Master
        Task<List<CombinedMaster>> GetDistrictById(string stateMasterId);
        Task<Response> UpdateDistrictById(DistrictMaster districtMaster);
        Task<Response> AddDistrict(DistrictMaster districtMaster);
        Task<DistrictMaster> GetDistrictRecordById(string districtId);
        #endregion

        #region Assembly Master
        Task<List<CombinedMaster>> GetAssemblies(string stateId, string districtId, string electionTypeId);
        Task<List<CombinedMaster>> GetAssembliesByElectionType(string stateId, string districtId, string electionTypeId);
        Task<Response> UpdateAssembliesById(AssemblyMaster assemblyMaster);
        Task<Response> UpdatePC(ParliamentConstituencyMaster pcMaster);
        Task<Response> AddAssemblies(AssemblyMaster assemblyMaster);
        Task<Response> AddPC(ParliamentConstituencyMaster PcMaster);
        Task<AssemblyMaster> GetAssemblyById(string assemblyId);
        Task<AssemblyMaster> GetAssemblyByDistrictIdForElectionType(int districtMasterId);
        Task<ElectionInfoMaster> GetElectionInfoRecord(int boothMasterId);
        Task<AssemblyMaster> GetAssemblyByCode(string assemblyCode, string stateMasterId);
        Task<AssemblyMaster> GetAssemblyByCodeandState(string assemblyCode, string stateMasterId);

        #endregion

        #region SO Master
        Task<List<FieldOfficerMaster>> GetFieldOfficersListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId);


        Task<FieldOfficerProfile> GetFieldOfficerProfile(string foId);
        Task<FieldOfficerProfile> GetBLOOfficerProfile(string soId);
        Task<FieldOfficerProfile> GetAROProfile(string aroId);
        Task<Response> AddFieldOfficer(FieldOfficerMaster fieldOfficerViewModel);
        Task<Response> AddBLOOfficer(BLOMaster bLOMaster);
        Task<Response> UpdateFieldOfficer(FieldOfficerMaster fieldOfficerViewModel);
        Task<Response> UpdateFieldOfficerValidate(FieldOfficerMaster fieldOfficerViewModel);
        Task<Response> UpdateBLOOfficer(BLOMaster bLOMaster);
        /// <summary this api for Portal>
        Task<List<CombinedMaster>> GetBoothListByFoId(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId);
        /// </summary>
        /// <summary this api for Portal>
        Task<List<CombinedMaster>> GetBoothListForFo(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId);
        Task<List<CombinedMaster>> GetBoothListForResultDeclaration(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId);
        /// </summary>
        Task<FieldOfficerMasterList> GetFieldOfficerById(int FieldOfficerMasterId);
        #endregion

        #region AROResult
        Task<ServiceResponse> IsMobileNumberUnique(string mobileNumber);
        Task<Response> AddAROResult(AROResultMaster aROResultMaster);
        Task<Response> UpdateAROResult(AROResultMaster aROResultMaster);
        Task<Response> UpdateAROValidate(AROResultMaster aROResultMaster);
        Task<AROResultMasterList> GetAROResultById(int aroMasterId);
        Task<List<AROResultMaster>> GetAROListById(int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<IsRDProfileUpdated> IsRDProfileUpdated(int aroMasterId, string userId);
        #endregion

        #region Booth Master
        Task<List<CombinedMaster>> GetBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<List<CombinedMaster>> GetBoothListByFourthLevelId(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<List<CombinedMaster>> GetBoothListByPSZonePanchayatId(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int psZonePanchayatMasterId);
        Task<List<CombinedMaster>> GetBoothListByIdforPSO(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<List<CombinedMaster>> GetUnassignedBoothListById(int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<Response> AddBooth(BoothMaster boothMaster);
        Task<Response> UpdateBooth(BoothMaster boothMaster);
        Task<Response> BoothMapping(List<BoothMaster> boothMaster);
        Task<Response> ReleaseBooth(BoothMaster boothMaster);
        Task<Response> ReleaseBoothBLO(BoothMaster boothMaster);
        Task<BoothMaster> GetBoothById(string boothMasterId);
        Task<BoothDetailForVoterInQueue> GetBoothDetailForVoterInQueue(int boothMasterId);
        #endregion

        #region Event Master

        Task<List<EventMaster>> GetEventListById(int stateMasterId, int electionTypeMasterId);
        Task<List<EventMaster>> GetEventListForBooth(int stateMasterId, int electionTypeMasterId);
        Task<List<EventAbbr>> GetEventAbbrList();
        Task<ServiceResponse> AddEvent(EventMaster eventMaster);
        Task<ServiceResponse> UpdateEvent(EventMaster eventMaster);
        Task<ServiceResponse> UpdateEventStatus(EventMaster eventMaster);
        Task<EventMaster> GetEventById(int eventMasterId);
        Task<ServiceResponse> DeleteEventById(int eventMasterId);
        Task<List<EventWiseBooth>> GetBoothListByEventId(string eventId, string soId);
        Task<List<EventActivityWiseBooth>> GetBoothEventActivityById(string soId);
        Task<List<TurnOutBoothListStatus>> GetBoothInfoinPollDetail(string soiId, string eventId);

        Task<List<EventWiseBooth>> GetBoothStatusforARO(string assemblyMasterId, string boothMasterId);

        #endregion

        #region PCMaster
        Task<List<ParliamentConstituencyMaster>> GetPCList(string stateMasterId);
        Task<List<AssemblyMaster>> GetAssemblyByPCId(string stateMasterid, string PcMasterId);
        Task<List<AssemblyMaster>> GetAssemblyByDistrictId(string stateMasterid, string districtMasterId);
        Task<ParliamentConstituencyMaster> GetPCById(string pcMasterId);

        #endregion

        #region Event Activity
        Task<ServiceResponse> IsVTEventTimeExtended(int stateMasterId, int electionTypeMasterId, bool isVTEventTimeExtended);

        #region  IsCheckEvent 
        Task<ServiceResponse> IsPartyDispatch(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsPartyArrived(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsSetupPollingStation(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsMockPollDone(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsPollStarted(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsVoterTurnOut(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsVoterInQueue(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsFinalVotesPolled(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsPollEnded(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsEVMVVPATOff(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsPartyDeparted(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsPartyReachedAtCollection(CheckEventActivity checkEventActivity);
        Task<ServiceResponse> IsEVMDeposited(CheckEventActivity checkEventActivity);
        #endregion

        #region Activity Events

        Task<ServiceResponse> PartyDispatch(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> PartyArrived(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> SetupPollingStation(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> MockPollDone(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> PollStarted(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> VoterTurnOut(UpdateEventActivity updateEventActivity,string userType);
        Task<ServiceResponse> VoterInQueue(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> FinalVotesPolled(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> PollEnded(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> EVMVVPATOff(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> PartyDeparted(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> PartyReachedAtCollection(UpdateEventActivity updateEventActivity);
        Task<ServiceResponse> EVMDeposited(UpdateEventActivity updateEventActivity);
        #endregion
        Task<(bool IsToday, string StartDateString, bool IsPrePolled)> IsEventActivityValid(int stateMasterId, int electionTypeMasterId, int eventMasterId);
        Task<bool> IsVTEventValidSlotDate(int stateMasterId, int electionTypeMasterId );
        Task<CheckEventActivity> GetNextEvent(UpdateEventActivity updateEventActivity);
        Task<CheckEventActivity> GetPreviousEvent(UpdateEventActivity updateEventActivity);
        Task<List<BoothEvents>> GetBoothEventListById(int stateMasterId, int electionTypeMasterId, int boothMasterId);

        Task<bool> CanPollStart(int boothMasterId, int eventid);
        Task<bool> CanQueueStart(int boothMasterId);
        Task<bool> QueueTime(int boothMasterId);
        Task<bool> CanFinalValueStart(int boothMasterId);


        Task<VoterTurnOutPolledDetailViewModel> GetLastUpdatedPollDetail(int boothMasterId, string userType);
        Task<Models.Queue> GetVoterInQueue(string boothMasterId);
        Task<VotesPolledPercentage> GetVotesPolledPercentage(ClaimsIdentity claimsIdentity);
        Task<FinalViewModel> GetFinalVotes(int boothMasterId);
        Task<Models.Queue> GetTotalRemainingVoters(string boothMasterId);
        Task<Response> AddVoterTurnOut(AddVoterTurnOut addVoterTurnOut);

        Task<ElectionInfoMaster> EventUpdationStatus(ElectionInfoMaster electionInfoMaster);

        Task<List<EventWiseBoothStatus>> EventWiseBoothStatus(string soId);

        Task<bool> IsPollInterrupted(int boothMasterId);
        //Task<List<EventActivityForDashboard>> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId);
        //Task<(List<EventActivityForDashboard> eventActivities, int totalBoothCount)> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId);
        Task<(List<EventActivityForDashboard> eventActivities, int totalBoothCount)> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId);
        Task<int> GetTotalBoothActivity(int stateMasterId, int districtMasterId, string eventName);
        Task<List<EventActivityCount>> GetEventListDistrictWiseById(int stateMasterId, int electionTypeMasterId);
        ///This API fetches the district-wise event list for Pending events.
        Task<List<EventActivityCount>> GetPendingEventListDistrictWiseById(int stateMasterId,int electionTypeMasterId);
        Task<List<EventActivityCount>> GetEventListPCWiseById(string stateId, string userId);
        Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseById(int stateMasterId, int? districtMasterId, int electionTypeMasterId);
        ///This API fetches the Assembly-wise event list for Pending events.
        Task<List<AssemblyEventActivityCount>> GetPendingAssemblyWiseEventListById(int stateMasterId, int? districtMasterId, int electionTypeMasterId);
        Task<List<FourthLevelEventActivityCount>> GetEventListFourthLevelHWiseById(int stateMasterId, int? districtMasterId, int? assemblyMasterId, int electionTypeMasterId);
        ///This API fetches the FourthLevelH-wise event list for Pending events.
        Task<List<FourthLevelEventActivityCount>> GetPendingEventListFourthLevelHWiseById(int stateMasterId, int? districtMasterId, int? assemblyMasterId, int electionTypeMasterId);
        //Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseByStateId(string stateId);
        Task<List<AssemblyEventActivityCountPCWise>> GetEventListAssemblyWiseByPCId(string stateId, string pcId);
        Task<List<EventActivityBoothWise>> GetEventListBoothWiseById(int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelHMasterId, int? electionTypeMasterId);
        ///This API fetches the Booth-wise event list for Pending events.

        Task<List<EventActivityBoothWise>> GetPendingBoothWiseEventListById(int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelHMasterId, int? electionTypeMasterId);
        Task<List<EventActivityBoothWise>> GetEventListBoothWiseByPCId(string stateId, string pcId, string assemblyId);


        #endregion

        #region SendDashBoardCount
        Task<DashBoardRealTimeCount> GetDashBoardCount(ClaimsIdentity claimsIdentity);
        Task<DashBoardRealTimeCount> GetEventActivityDashBoardCount(string role, int electionTypeMasterId, int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelMasterId);
        Task<List<DashboardConnectedUser>> DashboardConnectedUser(DahboardMastersId dashboardMastersId, string roleType);

        #endregion

        #region SlotManagement
        Task<Response> AddEventSlot(List<SlotManagementMaster> addEventSlot);
        Task<List<SlotManagementMaster>> GetEventSlotList(int stateMasterId, int electionTypeMasterId, int EventId);
        Task<List<SlotManagementMaster>> GetEventSlotListByEventAbbr(int stateMasterId, int electionTypeMasterId, string eventAbbr);
        #endregion

        Task<List<UserList>> GetUserList(string soName, string type);

        #region PollInterruption Interruption
        Task<Response> AddPollInterruption(PollInterruption pollinterruptionData);
        Task<PollInterruption> GetPollInterruptionData(string boothMasterId);
        Task<List<PollInterruptionHistoryModel>> GetPollInterruptionHistoryById(string boothMasterId);

        Task<List<PollInterruptionDashboard>> GetPollInterruptionDashboard(ClaimsIdentity claimsIdentity);

        Task<int> GetPollInterruptionDashboardCount(string role, int electionTypeMasterId, int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelMasterId);

        Task<List<PollInterruptionDashboard>> GetBoothListBySoIdfoInterruption(ClaimsIdentity claimsIdentity);


        Task<BoothMaster> GetBoothRecord(int boothMasterId);


        #endregion


        #region PSOFORM
        Task<List<PSoFomCombined>> GetPsoFormDetail(string stateId, string districtId, string assemblyMasterId);
        Task<Response> AddPSOForm(PollingStationMaster pollingStationMaster, ClaimsIdentity claimsIdentity);
        Task<bool> PollingStationRecord(int boothMasterId);
        Task<bool> GetPollingStationRecordById(int? psMasterId);
        Task<List<PSoFomListView>> GetPSOlistbyARO(string stateMasterId, string districtMasterId, string assemblyMasterId);

        Task<List<PSFormViewModel>> GetPSFormRecordbyPSId(string pollingStationMasterId);

        Task<Response> UpdatePSoForm(PollingStationMaster psMaster, ClaimsIdentity claimsIdentity);

        Task<List<LocationModel>> GetLocationMasterById(string locMasterId);
        Task<List<LocationModelList>> GetLocationMasterforARO(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<List<LocationModelList>> GetLocationMasterforALL(BoothReportModel boothReportModel);
        Task<Response> AddLocation(LocationModel locationModel);

        Task<Response> UpdateLocation(LocationModel locationModel);
        #endregion

        #region Reports
        Task<List<ConsolidatePanchResultDeclarationReportList>> GetConsolidatedPanchResultDeclarationReport(ResultDeclaration resultDeclaration);
        Task<List<ConsolidatedUnOpposedPanchSarPanchAndNoKycCandidateReportList>> GetConsolidatedUnOppossedPanchResultDeclarationReport(ResultDeclaration resultDeclaration);
        Task<List<ConsolidatedUnOpposedPanchSarPanchAndNoKycCandidateReportList>> GetConsolidatedNoKycPanchResultDeclarationReport(ResultDeclaration resultDeclaration);
        Task<List<ConsolidateSarPanchResultDeclarationReportList>> GetConsolidatedSarPanchResultDeclarationReport(ResultDeclarationReportListModel resultDeclaration);
        Task<List<ConsolidatedUnOpposedPanchSarPanchAndNoKycCandidateReportList>> GetConsolidatedUnOppossedSarPanchResultDeclarationReport(ResultDeclarationReportListModel resultDeclaration);
        Task<List<ConsolidatedUnOpposedPanchSarPanchAndNoKycCandidateReportList>> GetConsolidatedNoKycSarPanchResultDeclarationReport(ResultDeclarationReportListModel resultDeclaration);
        Task<List<ConsolidatePanchResultDeclarationReportList>> GetConsolidatedElectedPanchResultDeclarationReport(ResultDeclaration resultDeclaration);
        Task<List<ConsolidateSarPanchResultDeclarationReportList>> GetConsolidatedElectedSarPanchResultDeclarationReport(ResultDeclarationReportListModel resultDeclaration);
        Task<List<ConsolidateBoothReport>> GetConsolidateBoothReports(BoothReportModel boothReportModel);
        Task<List<ConsolidateBoothReport>> GetConsolidateGPWardReports(BoothReportModel boothReportModel);
        Task<List<SoReport>> GetSOReport(BoothReportModel boothReportModel);
        Task<List<SoReport>> GetPendingSOReport(BoothReportModel boothReportModel);
        Task<List<AssemblyWisePendingBooth>> GetAssemblyWisePendingReports(string stateMasterId);
        #endregion

        Task<List<VTPSReportReportModel>> GetVoterTurnOutPollingStationReports(BoothReportModel boothReportModel);

        Task<List<VTReportModel>> GetVoterTurnOutConsolidatedReports(BoothReportModel boothReportModel);
        Task<List<VTReportModel>> GetSlotBasedVoterTurnOutReport(SlotVTReportModel boothReportModel);





        #region
        Task<Response> AddHelpDeskInfo(HelpDeskDetail helpDeskDetail);
        Task<List<HelpDeskDetail>> GetHelpDeskDetail(string assemblyId);

        #endregion

        #region District , Assembly wise SOCount and Event Count

        Task<List<SectorOfficerPendency>> GetDistrictWiseSOCountEventWiseCount(string sid);
        Task<List<SectorOfficerPendencyAssembly>> GetAssemblyWiseSOCountEventWiseCount(string sid, string district, string electionTypeMasterId);
        //Task<List<SectorOfficerPendencyBooth>> GetBoothWiseSOEventWiseCount(string stateMasterId, string districtMasterId, string assemblyMasterid);
        Task<List<SectorOfficerPendencyBooth>> GetBoothWiseSOEventWiseCount(string soMasterId);
        Task<List<SectorOfficerPendencybySoNames>> GetSONamesEventWiseCount(string stateMasterId, string districtMasterId, string assemblyMasterid);

        #endregion

        Task<List<CombinedMaster>> AppNotDownload(string stateMasterId);
        Task<List<VoterTurnOutSlotWise>> GetVoterTurnOutSlotBasedReport(string stateMasterId, string electionTypeMasterId);
        Task<List<AssemblyVoterTurnOutSlotWise>> GetSlotVTReporttAssemblyWise(string stateMasterId, string districtMasterId, string electionTypeMasterId);
        Task<List<BoothWiseVoterTurnOutSlotWise>> GetSlotVTReportBoothWise(string stateId, string districtId, string assemblyId, string electionTypeMasterId);

        Task<int?> GetPCMasterIdByAssemblyIdCode(string assemblyMasterId);

        #region QueueManagement
        Task<ServiceResponse> AddQueue(QIS addQIS);
        Task<QIS> GetQISList(int stateMasterId, int districtMasterId, int assemblyMasterId, int boothMaserid);
        #endregion

        #region BLOBoothMaster
        Task<List<BoothMaster>> GetBLOBoothById(string bloBoothMasterId);
        Task<List<CombinedMaster>> GetBlosListById(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<Response> BLOBoothMapping(List<BoothMaster> boothMaster);
        Task<List<CombinedMaster>> GetAssignedBoothListByBLOId(string stateMasterId, string districtMasterId, string assemblyMasterId, string bloId);
        Task<List<CombinedMaster>> GetUnassignedBLOBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<BLOOfficerCustom> GetBLObyId(string soId);
        #endregion

        #region Randomization

        Task<ServiceResponse> AddRandomization(PPR pPR);
        Task<int> GetRoundCountByRandomizationTaskId(int? randomizationId, int? stateMasterId);
        Task<int> GetCurrentRoundByRandomizationById(int? stateMasterId, int? districtmasterId, int? randomizationTaskDetailMasterId);
        Task<List<RandomizationList>> GetRandomizationListByStateId(int stateMasterId);
        Task<RandomizationList> GetRandomizationById(int pprMasterId);
        Task<List<RandomizationTableList>> GetRandomizationTableListByStateId(int stateMasterId);
        Task<RandomizationTableList> GetRandomizationListByDistrictId(int stateMasterId, int districtMasterId);
        Task<ServiceResponse> UpdateRandomizationById(PPR pPR);
        Task<List<RandomizationTaskDetail>> GetRandomizationTaskListByStateId(int stateMasterId);
        Task<ServiceResponse> AddRandomizationTaskDetail(RandomizationTaskDetail randomizationTaskDetail);


        #endregion

        #region GetBoothByLocation
        Task<List<CombinedMaster>> GetBoothByLocation(string latitude, string longitude);
        #endregion

        #region BLO

        Task<List<BLOBoothAssignedQueueCount>> GetBLOQueueCount(BoothReportModel boothReportModel);
        Task<List<BLOBoothAssignedQueueCount>> GetUnassignedBLOs(BoothReportModel boothReportModel);
        Task<List<BLOBoothAssignedQueueCount>> GetAssignedBLOs(BoothReportModel boothReportModel);
        Task<List<BLOBoothAssignedQueueCount>> GetBLOQueueCountOpen(string statemasterdi, string districtmasterid);
        #endregion

        #region Mobile Version
        Task<MobileVersion> GetMobileVersionById(string StateMasterId);

        Task<ServiceResponse> AddMobileVersion(MobileVersion mobileVersion);
        #endregion

        #region KYC For "Gram Panchayats" Public Details
        Task<ServiceResponse> AddKYCDetailsForGP(Kyc kyc);
        
        Task<ServiceResponse> UpdateKycDetailsForGP(Kyc kyc);
        Task<List<Kyc>> GetKYCDetails();
        Task<KycList> GetKycById(int kycMasterId);
        Task<List<KycList>> GetKYCDetailByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<List<KycList>> GetKYCDetailByFourthAndWardId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelMasterId, int? wardMasterId);

        Task<List<KycList>> GetKYCDetailByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId, string userId);
        Task<ServiceResponse> DeleteKycById(int kycMasterId);

        #endregion

        #region KYC For "Municipal Corporation","Municipal Council" and "Nagar Panchayat" Public Details
        Task<ServiceResponse> AddKYCDetailsForMCorpMCounAndNP(Kyc kyc);
        Task<ServiceResponse> UpdateKycDetailsForMCorpMCounAndNP(Kyc kyc);
        #endregion

        #region UnOpposed Public Details
        Task<ServiceResponse> AddUnOpposedDetails(UnOpposed unOpposed);
        Task<List<UnOpposed>> GetUnOpposedDetails();
        Task<List<UnOpposedList>> GetUnOpposedDetailsByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<ServiceResponse> UpdateUnOpposedDetails(UnOpposed unOpposed);
        Task<UnOpposedList> GetUnOpposedById(int unOpposedMasterId);
        Task<ServiceResponse> DeleteUnOpposedById(int unOpposedMasterId);

        #endregion

        #region ElectionType Master

        Task<List<ElectionTypeMaster>> GetAllElectionTypes();
        Task<ElectionTypeMaster> GetElectionTypeById(string elecTypeId);
        #endregion

        #region  FourthLevelH
        Task<Response> AddFourthLevelH(FourthLevelH fourthLevelH);
        Task<List<FourthLevelH>> GetFourthLevelHListById(int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<FourthLevelH> GetFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<Response> UpdateFourthLevelH(FourthLevelH fourthLevelH);
        Task<ServiceResponse> DeleteFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        #endregion

        #region SarpanchWards
        Task<Response> AddGPPanchayatWards(GPPanchayatWards gpPanchayatWards);
        Task<List<GPPanchayatWards>> GetPanchayatWardforResultDeclaration(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<List<GPPanchayatWards>> GetGPPanchayatWardsListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId);
        Task<Response> UpdateGPPanchayatWards(GPPanchayatWards gpPanchayatWards);
        Task<GPPanchayatWards> GetGPPanchayatWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int gpPanchayatWardsMasterId);

        Task<Response> DeleteGPPanchayatWardsById(int gpPanchayatWardsMasterId);

        #endregion

        #region  PSZonePanchayat
        Task<Response> AddPSZonePanchayat(PSZonePanchayat psZonePanchayat);
        Task<List<PSZonePanchayat>> GetPSZonePanchayatListById(int stateMasterId, int districtMasterId, int fourthLevelHMasterId, int assemblyMasterId);
        Task<PSZonePanchayat> GetPSZonePanchayatById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int psZonePanchayatMasterId);
        Task<Response> UpdatePSZonePanchayat(PSZonePanchayat psZonePanchayat);
        Task<ServiceResponse> DeletePSZonePanchayatById(int psZonePanchayatMasterId);

        #endregion

        #region GPVoter
        Task<ServiceResponse> IsVoterAndKycExist(int fourthLevelMasterId);
        Task<ServiceResponse> AddGPVoterDetails(GPVoter gpVoterPdf);
        Task<ServiceResponse> UpdateGPVoterDetails(GPVoter gpVoterPdf);
        Task<GPVoterList> GetGPVoterById(int gpVoterMasterId);
        Task<List<GPVoterList>> GetGPVoterListById(int stateMasterId, int districtMasterId, int assemblyMasterId,int electionTypeMasterId);
        Task<List<GPVoterList>> GetGPVoterListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId, string userId);
        Task<ServiceResponse> DeleteGPVoterById(int gpVoterMasterId);
        Task<List<VoterType>> GetVoterTypeListById();
        #endregion

        #region ResultDeclaration
        Task<ServiceResponseForRD> AddResultDeclarationDetails(List<ResultDeclaration> resultDeclaration);
        Task<int?> GetTotalVotersForUrbanRDAsync(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<ServiceResponseForRD> UpdateResultDeclarationForPortal(List<ResultDeclaration> resultDeclaration);
        Task<Dictionary<int, string>> GetCandidateNameByKycMasterId(List<int> kycMasterIds);
        Task<ResultDeclarationBoothWardList> GetResultByBoothId(int boothMasterId);
        Task<ResultDeclarationBoothWardList> GetResultByFourthLevelHMasterId(int fourthLevelHMasterId);
        Task<List<BoothResultList>> GetBoothResultListByFourthLevelId(int fourthlevelMasterId);
        Task<ResultDeclarationBoothWardList> GetResultByWardId(int wardMasterId);
        Task<List<BoothResultList>> GetWardResultListByFourthLevelId(int fourthlevelMasterId);

        Task<ServiceResponse> CheckIfAllBoothsPollEnded(int fieldOfficerMasterId);
        Task<Response> UpdateResultDeclarationDetails(ResultDeclaration resultDeclaration);
        Task<ResultDeclaration> GetResultDeclarationById(int resultDeclarationMasterId);
        Task<ServiceResponse> DeleteResultDeclarationById(int resultDeclarationMasterId);
        Task<List<CandidateListForResultDeclaration>> GetSarpanchListById(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<List<CandidateListForResultDeclaration>> GetPanchListById(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gPPanchayatWardsMasterId);

        Task<List<ResultDeclarationList>> GetResultDeclarationsByElectionType(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gpPanchayatWardsMasterId);

        #endregion

        #region PanchaytaMapping
        Task<Response> PanchayatMapping(List<FourthLevelH> fourthLevels);
        Task<Response> ReleasePanchayat(FourthLevelH fourthLevels);
        Task<List<CombinedPanchayatMaster>> GetPanchayatListByROId(int stateMasterId, int districtMasterId, int assemblyMasterId, string roId, string assginedType);
        Task<List<CombinedPanchayatMaster>> GetPanchayatListByROId(int stateMasterId, int districtMasterId, int assemblyMasterId, string roId);
        Task<List<CombinedPanchayatMaster>> GetFourthLevelHListExistInRDForRO(int stateMasterId, int districtMasterId, int assemblyMasterId, string roId);
        Task<List<CombinedPanchayatMaster>> GetFourthLevelHExistInRDListById(int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<List<CombinedPanchayatMaster>> GetFourthLevelListByAROId(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId, string roId, string assginedType);
        Task<List<CombinedPanchayatMaster>> GetUnassignedPanchayatListById(int stateMasterId, int districtMasterId, int assemblyMasterId, string assginedType);
        #endregion

        #region CompletedVoterList
        Task<List<CompletedVTList>> GetCompletedVTList(CommonReportModel commonReportModel);
        #endregion

        /// <summary>
        /// This API checks for dependencies in descending order before performing the operation.
        /// </summary>
        /// <returns></returns>
        /// 
        Task<IsMasterEditable> IsMasterEditable(int masterId, string type, int electionTypeMasterId);

        ///

        //Task<List<Disaster>> GetFieldAllOfficerMaster();
        //Task<List<int>> GetFOAsginedBooth(int foId);
        //Task<ServiceResponse> PushDisasterEvent(List<ElectionInfoMaster> electionInfoMaster);

        #region Result Declartion DashBoard
        Task<List<ResultList>> GetResultByStateId(int stateMasterId,int electionTypeMasterId);
        Task<List<ResultList>> GetResultByDistrictId(int stateMasterId, int districtMasterId, int electionTypeMasterId);
        Task<List<ResultList>> GetResultByAssemblyId(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId);
        Task<List<ResultList>> GetResultByFourthLevelId(int stateMasterId, int districtMasterId,  int assemblyMasterId, int fourthLevelMasterId, int electionTypeMasterId);
        #endregion
    }
}
