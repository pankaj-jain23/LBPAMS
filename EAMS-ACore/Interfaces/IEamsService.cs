using EAMS.Helper;
using EAMS.ViewModels.PSFormViewModel;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.ElectionType;
using EAMS_ACore.Models.EventActivityModels;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.Models.QueueModel;
using EAMS_ACore.ReportModels;
using EAMS_ACore.SignalRModels;
using System.Security.Claims;

namespace EAMS_ACore.Interfaces
{
    public interface IEamsService
    {
        #region Update && Delete MasterStatus
        Task<ServiceResponse> UpdateMasterStatus(UpdateMasterStatus updateMasterStatus);
        Task<ServiceResponse> DeleteMasterStatus(DeleteMasterStatus updateMasterStatus);
        #endregion

        #region State Master
        Task<List<StateMaster>> GetState();
        Task<Response> UpdateStateById(StateMaster stateMaster);
        Task<Response> ResetAccounts(string stateMasterId);
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
        Task<List<CombinedMaster>> GetAssemblies(string stateId, string districtMasterId, string electionId);
        Task<List<CombinedMaster>> GetAssembliesByElectionType(string stateId, string districtMasterId, string electionTypeId);
        Task<Response> UpdateAssembliesById(AssemblyMaster assemblyMaster);
        Task<Response> UpdatePC(ParliamentConstituencyMaster pcMaster);
        Task<Response> AddAssemblies(AssemblyMaster assemblyMaster);
        Task<Response> AddPC(ParliamentConstituencyMaster pcMaster);
        Task<AssemblyMaster> GetAssemblyById(string assemblyId);



        #endregion

        #region SO Master
        Task<List<FieldOfficerMaster>> GetFieldOfficersListById(int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<FieldOfficerProfile> GetFieldOfficerProfile(string Id, string role);
        Task<Response> AddFieldOfficer(FieldOfficerMaster fieldOfficerViewModel);
        Task<Response> AddBLOOfficer(BLOMaster bLOMaster);
        Task<Response> UpdateFieldOfficer(FieldOfficerMaster fieldOfficerViewModel);
        Task<Response> UpdateBLOOfficer(BLOMaster bLOMaster);
        /// <summary this api for Portal>
        Task<List<CombinedMaster>> GetBoothListByFoId(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId);
        /// </summary>
        ///   /// <summary this api for Mobile App>
        Task<List<CombinedMaster>> GetBoothListForFo(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId);
        Task<List<CombinedMaster>> GetBoothListForResultDeclaration(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId);
        /// </summary>
        Task<FieldOfficerMasterList> GetFieldOfficerById(int FieldOfficerMasterId);
        #endregion

        #region AROResult
        Task<Response> AddAROResult(AROResultMaster aROResultMaster);
        Task<Response> UpdateAROResult(AROResultMaster aROResultMaster);
        Task<AROResultMasterList> GetAROResultById(int aroMasterId);
        Task<List<AROResultMaster>> GetAROListById(int stateMasterId, int districtMasterId, int assemblyMasterId);


        #endregion

        #region BoothMaster
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

        #region EventMaster

        Task<List<EventMaster>> GetEventListById(int stateMasterId, int electionTypeMasterId);
        Task<List<EventAbbr>> GetEventAbbrList( );
        Task<ServiceResponse> AddEvent(EventMaster eventMaster);
        Task<ServiceResponse> UpdateEvent(EventMaster eventMaster);
        Task<ServiceResponse> UpdateEventStaus(EventMaster eventMaster);
        Task<EventMaster> GetEventById(int eventMasterId);
        Task<List<EventMaster>> GetEventListForBooth(int stateMasterId, int electionTypeMasterId);
        Task<ServiceResponse> DeleteEventById(int eventMasterId);
        Task<List<EventWiseBooth>> GetBoothListByEventId(string eventId, string soId);
        Task<List<EventActivityWiseBooth>> GetBoothEventActivityById(string soId);
        Task<List<TurnOutBoothListStatus>> GetBoothInfoinPollDetail(string eventId, string soId);


        Task<List<EventWiseBooth>> GetBoothStatusforARO(string assemblyMasterId, string boothMasterId);

        #endregion

        #region PCMaster

        Task<List<ParliamentConstituencyMaster>> GetPCList(string stateMasterId);
        Task<ParliamentConstituencyMaster> GetPCById(string pcMasterId);
        Task<List<AssemblyMaster>> GetAssemblyByPCId(string stateMasterid, string PcMasterId);
        Task<List<AssemblyMaster>> GetAssemblyByDistrictId(string stateMasterid, string districtMasterId);
        #endregion

        #region Event Activity
        Task<ServiceResponse> EventActivity(ElectionInfoMaster electionInfoMaster);
        Task<ServiceResponse> UpdateEventActivity(UpdateEventActivity updateEventActivity);
        Task<List<BoothEvents>> GetBoothEventListById(int stateMasterId,int electionTypeMasterId,int boothMasterId);
        Task<VoterTurnOutPolledDetailViewModel> GetLastUpdatedPollDetail(int boothMasterId);
        Task<Models.Queue> GetVoterInQueue(string boothMasterId);

        Task<FinalViewModel> GetFinalVotes(int boothMasterId);

        //Task<Response> AddVoterTurnOut(string boothMasterId, int eventid, string voterValue);
        Task<Response> AddVoterTurnOut(AddVoterTurnOut addVoterTurnOut);

        Task<List<EventWiseBoothStatus>> EventWiseBoothStatus(string soId);
        //Task<List<EventActivityForDashboard>> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId);
        //Task<(List<EventActivityForDashboard> eventActivities, int totalBoothCount)> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId);
        Task<(List<EventActivityForDashboard> eventActivities, int totalBoothCount)> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId);
        Task<int> GetTotalBoothActivity(int stateMasterId, int districtMasterId, string eventName);
        Task<List<EventActivityCount>> GetEventListDistrictWiseById(string stateId);
        Task<List<EventActivityCount>> GetEventListPCWiseById(string stateId, string userId);
        Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseById(string stateId, string districtId);
        //Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseByStateId(string stateId);

        Task<List<AssemblyEventActivityCountPCWise>> GetEventListAssemblyWiseByPCId(string stateId, string pcId);
        Task<List<EventActivityBoothWise>> GetEventListBoothWiseById(string stateId, string districtId, string assemblyId);
        Task<List<EventActivityBoothWise>> GetEventListBoothWiseByPCId(string stateId, string pcId, string assemblyId);


        #endregion

        #region SendDashBoardCount
        Task<DashBoardRealTimeCount> GetDashBoardCount(ClaimsIdentity claimsIdentity);
        Task<DashBoardRealTimeCount> GetEventActivityDashBoardCount(string role, int electionTypeMasterId, int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelMasterId);
        Task<List<DashboardConnectedUser>> DashboardConnectedUser(DahboardMastersId dashboardMastersId, string roleType);
        #endregion

        #region SlotManagement
        Task<Response> AddEventSlot(List<SlotManagementMaster> addEventSlot);
        Task<List<SlotManagementMaster>> GetEventSlotList(int stateMasterId, int electionTypeMasterId, int eventId);
        #endregion

        Task<List<UserList>> GetUserList(string soName, string type);

        #region PollInterruption Interruption
        Task<Response> AddPollInterruption(PollInterruption Pollinterruptionl);

        Task<PollInterruption> GetPollInterruptionbyId(string boothMasterId);
        Task<List<PollInterruptionHistoryModel>> GetPollInterruptionHistoryById(string boothMasterId);


        Task<List<PollInterruptionDashboard>> GetPollInterruptionDashboard(ClaimsIdentity claimsIdentity);

        Task<int> GetPollInterruptionDashboardCount(ClaimsIdentity claimsIdentity);
        //Task<List<PollInterruptionDashboard>> AddPSOForm(ClaimsIdentity claimsIdentity);

        Task<List<PollInterruptionDashboard>> GetBoothListBySoIdfoInterruption(ClaimsIdentity claimsIdentity);


        #endregion

        #region PSOFORM
        Task<Response> AddPSOForm(PollingStationMaster pollingStationMaster, ClaimsIdentity claimsIdentity);
        Task<bool> PollingStationRecord(Int32 boothMasterId);
        Task<bool> GetPollingStationRecordById(int? psMasterId);

        Task<List<PSoFomListView>> GetPSOlistbyARO(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<List<PSoFomCombined>> GetPsoFormDetail(string stateMasterId, string districtMasterId, string assemblyMasterId);

        Task<List<PSFormViewModel>> GetPSFormRecordbyPSId(string PSMasterId);


        Task<Response> UpdatePSoForm(PollingStationMaster psMaster, ClaimsIdentity claimsIdentity);


        Task<List<LocationModelList>> GetLocationMasterforARO(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<List<LocationModelList>> GetLocationMasterforALL(BoothReportModel model);

        Task<List<LocationModel>> GetLocationMasterById(string locationMasterId);
        Task<Response> AddLocation(LocationModel locationModel);
        Task<Response> UpdateLocation(LocationModel locationModel);
        #endregion

        #region Reports
        Task<List<ConsolidateBoothReport>> GetConsolidateBoothReports(BoothReportModel boothReportModel);
        Task<List<ConsolidateBoothReport>> GetConsolidateGPWardReports(BoothReportModel boothReportModel);
        Task<List<SoReport>> GetSOReport(BoothReportModel boothReportModel);
        Task<List<SoReport>> GetPendingSOReport(BoothReportModel boothReportModel);

        Task<List<AssemblyWisePendingBooth>> GetAssemblyWisePendingReports(string stateMasterId);
        #endregion

        Task<List<VTPSReportReportModel>> GetVoterTurnOutPollingStationReports(BoothReportModel boothReportModel);
        Task<List<VTReportModel>> GetVoterTurnOutConsolidatedReports(BoothReportModel boothReportModel);
        //Task<List<VTReportModel>> GetSlotBasedVoterTurnOutReport(SlotVTReportModel boothReportModel);

       

        #region HelpDesk
        Task<Response> AddHelpDeskInfo(HelpDeskDetail helpDeskDetail);
        Task<List<HelpDeskDetail>> GetHelpDeskDetail(string assemblyMasterId);
        #endregion

        #region
        Task<List<SectorOfficerPendency>> GetDistrictWiseSOCountEventWiseCount(string stateMasterId);
        Task<List<SectorOfficerPendencyAssembly>> GetAssemblyWiseSOCountEventWiseCount(string stateMasterId, string districtMasterId);
        //Task<List<SectorOfficerPendencyBooth>> GetBoothWiseSOEventWiseCount(string stateMasterId, string districtMasterId,string assemblyMasterid);
        Task<List<SectorOfficerPendencyBooth>> GetBoothWiseSOEventWiseCount(string soMasterId);

        Task<List<SectorOfficerPendencybySoNames>> GetSONamesEventWiseCount(string stateMasterId, string districtMasterId, string assemblyMasterid);
        #endregion

        Task<List<CombinedMaster>> AppNotDownload(string stateMasterId);
        Task<List<VoterTurnOutSlotWise>> GetVoterTurnOutSlotBasedReport(string stateMasterId);
        Task<List<AssemblyVoterTurnOutSlotWise>> GetSlotVTReporttAssemblyWise(string stateMasterId, string districtMasterId);
        Task<List<BoothWiseVoterTurnOutSlotWise>> GetSlotVTReportBoothWise(string stateId, string districtId, string assemblyId);

        #region QueueManagement
        Task<ServiceResponse> AddQueue(QIS addQIS);
        Task<QIS> GetQISList(string stateMasterId, string districtMasterId, string assemblyMasterId, string boothMasterId);
        #endregion

        #region BLOBoothMaster
        Task<List<BoothMaster>> GetBLOBoothById(string bloBoothMasterId);
        Task<List<CombinedMaster>> GetBlosListById(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<List<CombinedMaster>> GetAssignedBoothListByBLOId(string stateMasterId, string districtMasterId, string assemblyMasterId, string soId);
        Task<List<CombinedMaster>> GetUnassignedBLOBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId);
        Task<Response> BLOBoothMapping(List<BoothMaster> boothMaster);
        Task<BLOOfficerCustom> GetBLObyId(string bloId);
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
        Task<List<BLOBoothAssignedQueueCount>> GetBLOQueueCountOpen(string sid, string did);

        #endregion

        #region MobileVersions
        Task<MobileVersion> GetMobileVersionById(string StateMasterId);

        Task<ServiceResponse> AddMobileVersion(MobileVersion mobileVersion);
        #endregion

        #region KYC Public Details
        Task<ServiceResponse> AddKYCDetails(Kyc kyc);
        Task<ServiceResponse> UpdateKycDetails(Kyc kyc);
        Task<List<Kyc>> GetKYCDetails();
        Task<KycList> GetKycById(int kycMasterId);
        Task<List<KycList>> GetKYCDetailByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<List<KycList>> GetKYCDetailByFourthAndWardId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelMasterId, int? wardMasterId);
        Task<List<KycList>> GetKYCDetailByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId, string userId);
        Task<ServiceResponse> DeleteKycById(int kycMasterId);
        
        #endregion

        #region UnOpposed Public Details
        Task<ServiceResponse> AddUnOpposedDetails(UnOpposed unOpposed);
        Task<List<UnOpposed>> GetUnOpposedDetails();
        Task<List<UnOpposedList>> GetUnOpposedDetailsByAssemblyId(int electionType,int stateMasterId, int districtMasterId, int assemblyMasterId );
        Task<ServiceResponse> UpdateUnOpposedDetails(UnOpposed unOpposed);
        Task<UnOpposedList> GetUnOpposedById(int unOpposedMasterId);
        Task<ServiceResponse> DeleteUnOpposedById(int unOpposedMasterId);
        #endregion

        #region Election Type Master
        Task<List<ElectionTypeMaster>> GetAllElectionTypes();
        #endregion

        #region  FourthLevelH
        Task<Response> AddFourthLevelH(FourthLevelH fourthLevelH);
        Task<List<FourthLevelH>> GetFourthLevelHListById(int stateMasterId, int districtMasterId, int assemblyMasterId); 
        Task<FourthLevelH> GetFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<Response> UpdateFourthLevelH(FourthLevelH fourthLevelH);
        Task<ServiceResponse> DeleteFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        #endregion

        #region  PSZonePanchayat
        Task<Response> AddPSZonePanchayat(PSZonePanchayat psZonePanchayat);
        Task<List<PSZonePanchayat>> GetPSZonePanchayatListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<PSZonePanchayat> GetPSZonePanchayatById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int psZonePanchayatMasterId);
        Task<Response> UpdatePSZonePanchayat(PSZonePanchayat psZonePanchayat);
        Task<ServiceResponse> DeletePSZonePanchayatById( int psZonePanchayatMasterId);
        #endregion

        #region GPPanchayatWards
        Task<Response> AddGPPanchayatWards(GPPanchayatWards gpPanchayatWardshWards);
        Task<List<GPPanchayatWards>> GetPanchayatWardforResultDeclaration(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<List<GPPanchayatWards>> GetGPPanchayatWardsListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId);
        Task<Response> UpdateGPPanchayatWards(GPPanchayatWards gpPanchayatWards);
        Task<GPPanchayatWards> GetGPPanchayatWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int gpPanchayatWardsMasterId);

        Task<Response> DeleteGPPanchayatWardsById(int gpPanchayatWardsMasterId);

        #endregion

        #region GPVoter
        Task<ServiceResponse> AddGPVoterDetails(GPVoter gpVoterPdf);
        Task<ServiceResponse> UpdateGPVoterDetails(GPVoter gpVoterPdf);
        Task<GPVoterList> GetGPVoterById(int gpVoterMasterId);
        Task<List<GPVoterList>> GetGPVoterListById(int stateMasterId, int districtMasterId, int assemblyMasterId);
        Task<List<GPVoterList>> GetGPVoterListById(int stateMasterId, int districtMasterId, int assemblyMasterId, string userId);
        Task<ServiceResponse> DeleteGPVoterById(int gpVoterMasterId);
        #endregion

        #region ResultDeclaration
        Task<ServiceResponse> AddResultDeclarationDetails(List<ResultDeclaration> resultDeclaration);
        Task<Response> UpdateResultDeclarationDetails(ResultDeclaration resultDeclaration);
        Task<ResultDeclaration> GetResultDeclarationById(int resultDeclarationMasterId);
        Task<ServiceResponse> DeleteResultDeclarationById(int resultDeclarationMasterId);
        Task<List<ResultDeclarationList>> GetResultDeclarationsByElectionType(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gpPanchayatWardsMasterId);
        Task<List<CandidateListForResultDeclaration>> GetSarpanchListById(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId);
        Task<List<CandidateListForResultDeclaration>> GetPanchListById(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gPPanchayatWardsMasterId);
        
        #endregion

        #region PanchaytaMapping
        Task<Response> PanchayatMapping(List<FourthLevelH> fourthLevels);
        Task<Response> ReleasePanchayat(FourthLevelH fourthLevels);
        Task<List<CombinedPanchayatMaster>> GetPanchayatListByROId(int stateMasterId, int districtMasterId, int assemblyMasterId, string roId, string assginedType);
        Task<List<CombinedPanchayatMaster>> GetPanchayatListByROId(int stateMasterId, int districtMasterId, int assemblyMasterId, string roId);
        Task<List<CombinedPanchayatMaster>> GetUnassignedPanchayatListById(int stateMasterId, int districtMasterId, int assemblyMasterId, string assginedType);
        #endregion
    }
}
