using EAMS.Helper;
using EAMS.ViewModels.PSFormViewModel;
using EAMS_ACore;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.IExternal;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
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
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace EAMS_BLL.Services
{
    public class EamsService : IEamsService
    {
        private readonly IEamsRepository _eamsRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ICacheService _cacheService;
        public EamsService(IEamsRepository eamsRepository, IAuthRepository authRepository, ICacheService cacheService)
        {
            _eamsRepository = eamsRepository;
            _authRepository = authRepository;
            _cacheService = cacheService;
        }
        private DateTime? BharatDateTime()
        {
            DateTime dateTime = DateTime.Now;
            DateTime utcDateTime = DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
            TimeSpan istOffset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
            TimeZoneInfo istTimeZone = TimeZoneInfo.CreateCustomTimeZone("IST", istOffset, "IST", "IST");
            DateTime hiINDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);
            return DateTime.SpecifyKind(hiINDateTime, DateTimeKind.Utc);
        }

        #region UpdateMaster
        public async Task<ServiceResponse> UpdateMasterStatus(UpdateMasterStatus updateMasterStatus)
        {
            var isSucced = await _eamsRepository.UpdateMasterStatus(updateMasterStatus);
            if (isSucced.IsSucceed)
            {
                return new ServiceResponse
                {
                    IsSucceed = true,
                    Message = isSucced.Message,
                };

            }
            else
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = isSucced.Message
                };
            }
        }
        #endregion

        #region Clear Mappings
        public async Task<ServiceResponse> IsClearBLOMappings(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.IsClearBLOMappings(stateMasterId, electionTypeMasterId);
        }
        public async Task<ServiceResponse> IsClearSOMappings(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.IsClearSOMappings(stateMasterId, electionTypeMasterId);
        }
        public async Task<ServiceResponse> IsClearPollDetails(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.IsClearPollDetails(stateMasterId, electionTypeMasterId);
        }
        public async Task<ServiceResponse> IsClearElectionInfo(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.IsClearElectionInfo(stateMasterId, electionTypeMasterId);
        }
        public async Task<ServiceResponse> IsClearSlotInfo(int stateMasterId, int electionTypeMasterId, int eventMasterId)
        {
            return await _eamsRepository.IsClearSlotInfo(stateMasterId, electionTypeMasterId, eventMasterId);
        }
        #endregion

        #region DeleteMaster
        public async Task<ServiceResponse> DeleteMasterStatus(DeleteMasterStatus updateMasterStatus)
        {
            var isSucced = await _eamsRepository.DeleteMasterStatus(updateMasterStatus);
            if (isSucced.IsSucceed)
            {
                return new ServiceResponse
                {
                    IsSucceed = true,
                    Message = isSucced.Message,
                };

            }
            else
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = isSucced.Message
                };
            }
        }
        #endregion


        #region State Master

        public async Task<List<StateMaster>> GetState()
        {
            return await _eamsRepository.GetState();
        }

        public async Task<Response> ResetAccounts(string stateMasterId)
        {
            return await _eamsRepository.ResetAccounts(stateMasterId);
        }

        public async Task<Response> UpdateStateById(StateMaster stateMaster)
        {
            return await _eamsRepository.UpdateStateById(stateMaster);
        }
        public async Task<Response> AddState(StateMaster stateMaster)
        {
            return await _eamsRepository.AddState(stateMaster);

        }
        public async Task<StateMaster> GetStateById(string stateId)
        {
            return await _eamsRepository.GetStateById(stateId);
        }


        #endregion

        #region District Master

        public async Task<List<CombinedMaster>> GetDistrictById(string stateMasterId)
        {
            return await _eamsRepository.GetDistrictById(stateMasterId);
        }


        public async Task<Response> UpdateDistrictById(DistrictMaster districtMaster)
        {
            return await _eamsRepository.UpdateDistrictById(districtMaster);

        }

        public async Task<Response> AddDistrict(DistrictMaster districtMaster)
        {
            return await _eamsRepository.AddDistrict(districtMaster);
        }
        public async Task<DistrictMaster> GetDistrictRecordById(string districtId)
        {
            return await _eamsRepository.GetDistrictRecordById(districtId);
        }
        #endregion   

        #region Assembly  Master
        public async Task<List<CombinedMaster>> GetAssemblies(string stateId, string districtId, string electionTypeId)
        {
            return await _eamsRepository.GetAssemblies(stateId, districtId, electionTypeId);
        }
        public async Task<List<CombinedMaster>> GetAssembliesByElectionType(string stateId, string districtId, string electionTypeId)
        {
            return await _eamsRepository.GetAssembliesByElectionType(stateId, districtId, electionTypeId);
        }

        public async Task<Response> UpdateAssembliesById(AssemblyMaster assemblyMaster)
        {
            return await _eamsRepository.UpdateAssembliesById(assemblyMaster);
        }
        public async Task<Response> UpdatePC(ParliamentConstituencyMaster pcMaster)
        {
            return await _eamsRepository.UpdatePC(pcMaster);
        }

        public async Task<Response> AddAssemblies(AssemblyMaster assemblyMaster)
        {
            return await _eamsRepository.AddAssemblies(assemblyMaster);
        }

        public async Task<Response> AddPC(ParliamentConstituencyMaster pcMaster)
        {
            return await _eamsRepository.AddPC(pcMaster);
        }

        public async Task<AssemblyMaster> GetAssemblyById(string assemblyMasterId)
        {
            return await _eamsRepository.GetAssemblyById(assemblyMasterId);
        }
        #endregion

        #region  FO Master
        public async Task<List<FieldOfficerMaster>> GetFieldOfficersListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetFieldOfficersListById(stateMasterId, districtMasterId, assemblyMasterId, electionTypeMasterId);
        }

        public async Task<List<CombinedMaster>> AppNotDownload(string stateMasterId)
        {
            return await _eamsRepository.AppNotDownload(stateMasterId);
        }

        public async Task<FieldOfficerProfile> GetFieldOfficerProfile(string Id, string role)
        {
            if (role == "FO")
            {
                return await _eamsRepository.GetFieldOfficerProfile(Id);
            }
            else if (role == "BLO")
            {
                return await _eamsRepository.GetBLOOfficerProfile(Id);
            }
            else
            {
                return await _eamsRepository.GetAROProfile(Id);
            }
        }

        public async Task<Response> AddFieldOfficer(FieldOfficerMaster fieldOfficerViewModel)
        {
            return await _eamsRepository.AddFieldOfficer(fieldOfficerViewModel);
        }
        public async Task<Response> AddBLOOfficer(BLOMaster bLOMaster)
        {
            return await _eamsRepository.AddBLOOfficer(bLOMaster);
        }
        public async Task<Response> UpdateFieldOfficer(FieldOfficerMaster fieldOfficerViewModel)
        {
            return await _eamsRepository.UpdateFieldOfficer(fieldOfficerViewModel);
        }

        public async Task<Response> UpdateBLOOfficer(BLOMaster bLOMaster)
        {
            return await _eamsRepository.UpdateBLOOfficer(bLOMaster);
        }
        /// <summary this api for Portal>
        public async Task<List<CombinedMaster>> GetBoothListByFoId(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId)
        {
            return await _eamsRepository.GetBoothListByFoId(stateMasterId, districtMasterId, assemblyMasterId, foId);
        }
        /// </summary>
        /// <summary this api for Mpbile App>
        public async Task<List<CombinedMaster>> GetBoothListForFo(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId)
        {
            return await _eamsRepository.GetBoothListForFo(stateMasterId, districtMasterId, assemblyMasterId, foId);
        }

        public async Task<List<CombinedMaster>> GetBoothListForResultDeclaration(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId)
        {
            return await _eamsRepository.GetBoothListForResultDeclaration(stateMasterId, districtMasterId, assemblyMasterId, foId);
        }
        /// </summary>
        public async Task<FieldOfficerMasterList> GetFieldOfficerById(int FieldOfficerMasterId)
        {
            return await _eamsRepository.GetFieldOfficerById(FieldOfficerMasterId);
        }

        #endregion

        #region AROResult
      public  async Task<ServiceResponse> IsMobileNumberUnique(string mobileNumber)
        {
            return await _eamsRepository.IsMobileNumberUnique( mobileNumber);
        }
        public async Task<Response> AddAROResult(AROResultMaster aROResultMaster)
        {
            return await _eamsRepository.AddAROResult(aROResultMaster);
        }
        public async Task<Response> UpdateAROResult(AROResultMaster aROResultMaster)
        {
            return await _eamsRepository.UpdateAROResult(aROResultMaster);
        }
        public async Task<AROResultMasterList> GetAROResultById(int aroMasterId)
        {
            return await _eamsRepository.GetAROResultById(aroMasterId);
        }
        public async Task<List<AROResultMaster>> GetAROListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            return await _eamsRepository.GetAROListById(stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<IsRDProfileUpdated> IsRDProfileUpdated(int aroMasterId, string userId)
        {
            return await _eamsRepository.IsRDProfileUpdated(aroMasterId, userId);
        }
        #endregion

        #region Booth Master

        public async Task<List<CombinedMaster>> GetBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetBoothListById(stateMasterId, districtMasterId, assemblyMasterId);
        }

        public async Task<List<CombinedMaster>> GetBoothListByFourthLevelId(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            return await _eamsRepository.GetBoothListByFourthLevelId(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
        }
        public async Task<List<CombinedMaster>> GetBoothListByPSZonePanchayatId(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int psZonePanchayatMasterId)
        {
            return await _eamsRepository.GetBoothListByPSZonePanchayatId(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId, psZonePanchayatMasterId);
        }
        public async Task<List<CombinedMaster>> GetBoothListByIdforPSO(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetBoothListByIdforPSO(stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<List<CombinedMaster>> GetUnassignedBoothListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            return await _eamsRepository.GetUnassignedBoothListById(stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<Response> AddBooth(BoothMaster boothMaster)
        {
            return await _eamsRepository.AddBooth(boothMaster);
        }

        public async Task<Response> UpdateBooth(BoothMaster boothMaster)
        {
            return await _eamsRepository.UpdateBooth(boothMaster);
        }
        public async Task<Response> BoothMapping(List<BoothMaster> boothMaster)
        {
            return await _eamsRepository.BoothMapping(boothMaster);
        }
        public async Task<Response> ReleaseBooth(BoothMaster boothMaster)
        {
            return await _eamsRepository.ReleaseBooth(boothMaster);
        }
        public async Task<Response> ReleaseBoothBLO(BoothMaster boothMaster)
        {
            return await _eamsRepository.ReleaseBoothBLO(boothMaster);
        }

        public async Task<BoothMaster> GetBoothById(string boothMasterId)
        {
            return await _eamsRepository.GetBoothById(boothMasterId);
        }
        public async Task<BoothDetailForVoterInQueue> GetBoothDetailForVoterInQueue(int boothMasterId)
        {
            return await _eamsRepository.GetBoothDetailForVoterInQueue(boothMasterId);
        }

        #endregion

        #region Event Master
        public async Task<ServiceResponse> IsVTEventTimeExtended(int stateMasterId, int electionTypeMasterId, bool isVTEventTimeExtended)
        {

            return await _eamsRepository.IsVTEventTimeExtended(stateMasterId, electionTypeMasterId, isVTEventTimeExtended);
        }
        public async Task<List<EventMaster>> GetEventListById(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetEventListById(stateMasterId, electionTypeMasterId);
        }
        public async Task<List<EventMaster>> GetEventListForBooth(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetEventListForBooth(stateMasterId, electionTypeMasterId);
        }
        public async Task<List<EventAbbr>> GetEventAbbrList()
        {
            return await _eamsRepository.GetEventAbbrList();
        }
        public async Task<ServiceResponse> AddEvent(EventMaster eventMaster)
        {
            return await _eamsRepository.AddEvent(eventMaster);
        }
        public async Task<ServiceResponse> UpdateEvent(EventMaster eventMaster)
        {
            return await _eamsRepository.UpdateEvent(eventMaster);
        }
        public async Task<ServiceResponse> UpdateEventStatus(EventMaster eventMaster)
        {
            var isSucced = await _eamsRepository.UpdateEventStatus(eventMaster);
            if (isSucced.IsSucceed)
            {
                return new ServiceResponse
                {
                    IsSucceed = true,
                    Message = "Status Updated Successfully"
                };

            }
            else
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "Status not Updated! "
                };
            }
        }
        public async Task<EventMaster> GetEventById(int eventMasterId)
        {
            return await _eamsRepository.GetEventById(eventMasterId);
        }
        public async Task<ServiceResponse> DeleteEventById(int eventMasterId)
        {
            return await _eamsRepository.DeleteEventById(eventMasterId);
        }
        public async Task<List<EventWiseBooth>> GetBoothListByEventId(string eventId, string soId)
        {
            return await _eamsRepository.GetBoothListByEventId(eventId, soId);
        }
        public async Task<List<EventActivityWiseBooth>> GetBoothEventActivityById(string soId)
        {
            return await _eamsRepository.GetBoothEventActivityById(soId);
        }
        public async Task<List<TurnOutBoothListStatus>> GetBoothInfoinPollDetail(string soId, string eventId)
        {
            return await _eamsRepository.GetBoothInfoinPollDetail(soId, eventId);
        }
        public async Task<List<EventWiseBooth>> GetBoothStatusforARO(string assemblyMasterId, string boothMasterId)
        {
            return await _eamsRepository.GetBoothStatusforARO(assemblyMasterId, boothMasterId);
        }

        #endregion

        #region PCMaster
        public async Task<List<ParliamentConstituencyMaster>> GetPCList(string stateMasterId)
        {
            return await _eamsRepository.GetPCList(stateMasterId);
        }
        public async Task<ParliamentConstituencyMaster> GetPCById(string pcMasterId)
        {
            return await _eamsRepository.GetPCById(pcMasterId);
        }



        public async Task<List<AssemblyMaster>> GetAssemblyByPCId(string stateMasterid, string PcMasterId)
        {
            return await _eamsRepository.GetAssemblyByPCId(stateMasterid, PcMasterId);
        }

        public async Task<List<AssemblyMaster>> GetAssemblyByDistrictId(string stateMasterid, string districtMasterId)
        {
            return await _eamsRepository.GetAssemblyByDistrictId(stateMasterid, districtMasterId);
        }

        #endregion

        #region EventActivity
        public async Task<(bool IsToday, string StartDateString, bool IsPrePolled)> IsEventActivityValid(int stateMasterId, int electionTypeMasterId, int eventMasterId)
        {
            return await _eamsRepository.IsEventActivityValid(stateMasterId, electionTypeMasterId, eventMasterId);
        }
        public async Task<ServiceResponse> UpdateEventActivity(UpdateEventActivity updateEventActivity, string userType)
        {
            // Get the previous event status
            var previousEventStatus = await _eamsRepository.GetPreviousEvent(updateEventActivity);
            var nextEvent = await _eamsRepository.GetNextEvent(updateEventActivity);
            // If no previous event exists, update the activity directly only for Party Dispatch Case
            if (previousEventStatus == null)
            {
                //For Undo case we have to check next event is false or not in Party Dispatch Case
                CheckEventActivity checkEventActivity = new CheckEventActivity()
                {
                    StateMasterId = updateEventActivity.StateMasterId,
                    DistrictMasterId = updateEventActivity.DistrictMasterId,
                    AssemblyMasterId = updateEventActivity.AssemblyMasterId,
                    BoothMasterId = updateEventActivity.BoothMasterId,
                    ElectionTypeMasterId = updateEventActivity.ElectionTypeMasterId,
                    EventMasterId = nextEvent.EventMasterId,
                    EventABBR = nextEvent.EventABBR,
                    EventSequence = nextEvent.EventSequence,
                    EventStatus = nextEvent.EventStatus,
                };

                var isPDTrue = await IsEventActivityDone(checkEventActivity);
                if (isPDTrue.IsSucceed == true)
                {
                    return new ServiceResponse
                    {
                        IsSucceed = false,
                        Message = "You have to undo Last updated Event"
                    };
                }
                return await UpdateEventsActivity(updateEventActivity, userType);
            }

            // Check if the event activity is done
            var previousEventResponse = await IsEventActivityDone(previousEventStatus);
            var NextEventResponse = await IsEventActivityDone(nextEvent);

            // If the event is not done, return a failure response
            if (!previousEventResponse.IsSucceed)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = previousEventResponse.Message
                };
            }
            if (previousEventResponse.IsSucceed == true && NextEventResponse.IsSucceed == true)
            {
                return new ServiceResponse
                {
                    IsSucceed = false,
                    Message = "You have to undo Last updated Event"
                };
            }
            // If the event UpdateEventsActivity done, proceed with updating the activity
            return await UpdateEventsActivity(updateEventActivity, userType);
        }


        private async Task<ServiceResponse> IsEventActivityDone(CheckEventActivity checkEventActivity)
        {
            ServiceResponse response = null;

            switch (checkEventActivity.EventABBR)
            {
                case "PD": // Party Dispatch
                    response = await _eamsRepository.IsPartyDispatch(checkEventActivity);
                    break;

                case "PA": // Party Arrived
                    response = await _eamsRepository.IsPartyArrived(checkEventActivity);
                    break;
                case "SP": // Setup Polling Station
                    response = await _eamsRepository.IsSetupPollingStation(checkEventActivity);
                    break;
                case "MP": // Mock Poll Done
                    response = await _eamsRepository.IsMockPollDone(checkEventActivity);
                    break;
                case "PS": // Poll Started
                    response = await _eamsRepository.IsPollStarted(checkEventActivity);
                    break;
                case "VT": // Voter Turn Out
                    response = await _eamsRepository.IsVoterTurnOut(checkEventActivity);
                    break;
                case "VQ": // Voter In Queue
                    response = await _eamsRepository.IsVoterInQueue(checkEventActivity);
                    break;
                case "FV": // Final Votes Polled
                    response = await _eamsRepository.IsFinalVotesPolled(checkEventActivity);
                    break;
                case "PE": // Poll Ended
                    response = await _eamsRepository.IsPollEnded(checkEventActivity);
                    break;
                case "EO": // EVMVVPATOff
                    response = await _eamsRepository.IsEVMVVPATOff(checkEventActivity);
                    break;

                case "PC": // PartyDeparted	
                    response = await _eamsRepository.IsPartyDeparted(checkEventActivity);
                    break;

                case "PR": // PartyReachedAtCollection
                    response = await _eamsRepository.IsPartyReachedAtCollection(checkEventActivity);
                    break;

                case "ED": // EVMDeposited
                    response = await _eamsRepository.IsEVMDeposited(checkEventActivity);
                    break;



                default:
                    // Handle any unsupported events if necessary
                    response = new ServiceResponse { IsSucceed = false };
                    break;
            }

            return response;

        }

        private async Task<ServiceResponse> UpdateEventsActivity(UpdateEventActivity updateEventActivity, string userType)
        {
            ServiceResponse response = null;

            switch (updateEventActivity.EventABBR)
            {
                case "PD": // Party Dispatch
                    response = await _eamsRepository.PartyDispatch(updateEventActivity);
                    break;

                case "PA": // Party Arrived
                    response = await _eamsRepository.PartyArrived(updateEventActivity);
                    break;
                case "SP": // Setup Polling Station
                    response = await _eamsRepository.SetupPollingStation(updateEventActivity);
                    break;
                case "MP": // Mock Poll Done
                    response = await _eamsRepository.MockPollDone(updateEventActivity);
                    break;
                case "PS": // Poll Started
                    response = await _eamsRepository.PollStarted(updateEventActivity);
                    break;
                case "VT": // Voter Turn Out
                    response = await _eamsRepository.VoterTurnOut(updateEventActivity, userType);
                    break;
                case "VQ": // Voter In Queue
                    response = await _eamsRepository.VoterInQueue(updateEventActivity);
                    break;
                case "FV": // Final Votes Polled
                    response = await _eamsRepository.FinalVotesPolled(updateEventActivity);
                    break;
                case "PE": // Poll Ended
                    response = await _eamsRepository.PollEnded(updateEventActivity);
                    break;
                case "EO": // EVMVVPATOff
                    response = await _eamsRepository.EVMVVPATOff(updateEventActivity);
                    break;

                case "PC": // PartyDeparted	
                    response = await _eamsRepository.PartyDeparted(updateEventActivity);
                    break;

                case "PR": // PartyReachedAtCollection
                    response = await _eamsRepository.PartyReachedAtCollection(updateEventActivity);
                    break;

                case "ED": // EVMDeposited
                    response = await _eamsRepository.EVMDeposited(updateEventActivity);
                    break;



                default:
                    // Handle any unsupported events if necessary
                    response = new ServiceResponse { IsSucceed = false };
                    break;
            }

            return response;
        }
        public async Task<List<BoothEvents>> GetBoothEventListById(int stateMasterId, int electionTypeMasterId, int boothMasterId)
        {
            return await _eamsRepository.GetBoothEventListById(stateMasterId, electionTypeMasterId, boothMasterId);
        }

        public async Task<ServiceResponse> EventActivity(ElectionInfoMaster electionInfoMaster)
        {
            return new ServiceResponse
            {
                IsSucceed = false
            };
        }
        public async Task<List<EventWiseBoothStatus>> EventWiseBoothStatus(string soId)
        {
            return await _eamsRepository.EventWiseBoothStatus(soId);
        }

        public async Task<VoterTurnOutPolledDetailViewModel> GetLastUpdatedPollDetail(int boothMasterId, string userType)

        {
            return await _eamsRepository.GetLastUpdatedPollDetail(boothMasterId, userType);

        }
        public async Task<EAMS_ACore.Models.Queue> GetVoterInQueue(string boothMasterId)

        {
            return await _eamsRepository.GetVoterInQueue(boothMasterId);

        }


        public async Task<FinalViewModel> GetFinalVotes(int boothMasterId)

        {
            return await _eamsRepository.GetFinalVotes(boothMasterId);

        }

        public async Task<Response> AddVoterTurnOut(AddVoterTurnOut addVoterTurnOut)
        {
            var boothRecord = await _eamsRepository.GetBoothById(addVoterTurnOut.boothMasterId.ToString());
            if (boothRecord != null)
            {

                //if (Convert.ToInt32(addVoterTurnOut.Male) <= boothRecord.Male   && Convert.ToInt32(addVoterTurnOut.Female)  <= boothRecord.Female  && Convert.ToInt32(addVoterTurnOut.Transgender) <= boothRecord.Transgender)

                //{
                if (Convert.ToInt32(addVoterTurnOut.voterValue) <= boothRecord.TotalVoters)
                {
                    return await _eamsRepository.AddVoterTurnOut(addVoterTurnOut);
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "VoterValue should not exceeded to Total Voters Available" };
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "The counts of Male, Female, and Transgender voters should be equal to or less than the corresponding available values for Male, Female, and Transgender." };


            }

            //}
            //else
            //{
            //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Booth Not Exists." };
            //}


        }
        public async Task<(List<EventActivityForDashboard> eventActivities, int totalBoothCount)> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId)
        {
            // Calling the repository method
            return await _eamsRepository.GetEventActivitiesForDashboard(stateMasterId, districtMasterId);
        }

        public async Task<int> GetTotalBoothActivity(int stateMasterId, int districtMasterId, string eventName)
        {
            // Logic to fetch the total booths based on the event
            return await _eamsRepository.GetTotalBoothActivity(stateMasterId, districtMasterId, eventName);
        }

        //public async Task<(List<EventActivityForDashboard> eventActivities, int totalBoothCount)> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId)
        //{
        //    // Calling the repository method
        //    return await _eamsRepository.GetEventActivitiesForDashboard(stateMasterId, districtMasterId);
        //}
        //public async Task<List<EventActivityForDashboard>> GetEventActivitiesForDashboard(int stateMasterId, int districtMasterId)
        //{
        //    // Calling the repository method
        //    return await _eamsRepository.GetEventActivitiesForDashboard(stateMasterId, districtMasterId);
        //}
        public async Task<List<EventActivityCount>> GetEventListDistrictWiseById(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetEventListDistrictWiseById(stateMasterId, electionTypeMasterId);
        }
        ///This API fetches the district-wise event list for Pending events.

        public async Task<List<EventActivityCount>> GetPendingEventListDistrictWiseById(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetPendingEventListDistrictWiseById(stateMasterId, electionTypeMasterId);
        }
        public async Task<List<EventActivityCount>> GetEventListPCWiseById(string stateId, string userId)
        {
            return await _eamsRepository.GetEventListPCWiseById(stateId, userId);
        }
        public async Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseById(int stateMasterId, int? districtMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetEventListAssemblyWiseById(stateMasterId, districtMasterId, electionTypeMasterId);
        }
        ///This API fetches the Assembly-wise event list for Pending events.
        public async Task<List<AssemblyEventActivityCount>> GetPendingAssemblyWiseEventListById(int stateMasterId, int? districtMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetPendingAssemblyWiseEventListById(stateMasterId, districtMasterId, electionTypeMasterId);
        }
        public async Task<List<FourthLevelEventActivityCount>> GetEventListFourthLevelHWiseById(int stateMasterId, int? districtMasterId, int? assemblyMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetEventListFourthLevelHWiseById(stateMasterId, districtMasterId, assemblyMasterId, electionTypeMasterId);
        }
        ///This API fetches the FourthLevelH-wise event list for Pending events.

        public async Task<List<FourthLevelEventActivityCount>> GetPendingEventListFourthLevelHWiseById(int stateMasterId, int? districtMasterId, int? assemblyMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetPendingEventListFourthLevelHWiseById(stateMasterId, districtMasterId, assemblyMasterId, electionTypeMasterId);
        }
        public async Task<List<AssemblyEventActivityCountPCWise>> GetEventListAssemblyWiseByPCId(string stateId, string pcId)
        {
            return await _eamsRepository.GetEventListAssemblyWiseByPCId(stateId, pcId);
        }
        public async Task<List<EventActivityBoothWise>> GetEventListBoothWiseById(int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelHMasterId, int? electionTypeMasterId)
        {
            return await _eamsRepository.GetEventListBoothWiseById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId, electionTypeMasterId);
        }
        ///This API fetches the Booth-wise event list for Pending events.
        public async Task<List<EventActivityBoothWise>> GetPendingBoothWiseEventListById(int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelHMasterId, int? electionTypeMasterId)
        {
            return await _eamsRepository.GetPendingBoothWiseEventListById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId, electionTypeMasterId);
        }
        public async Task<List<EventActivityBoothWise>> GetEventListBoothWiseByPCId(string stateId, string pcId, string assemblyId)
        {
            return await _eamsRepository.GetEventListBoothWiseByPCId(stateId, pcId, assemblyId);
        }

        #endregion

        #region SendDashBoardCount 
        public async Task<DashBoardRealTimeCount> GetDashBoardCount(ClaimsIdentity claimsIdentity)
        {
            return await _eamsRepository.GetDashBoardCount(claimsIdentity);
        }
        public async Task<DashBoardRealTimeCount> GetEventActivityDashBoardCount(string role, int electionTypeMasterId, int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelMasterId)
        {

            return await _eamsRepository.GetEventActivityDashBoardCount(role, electionTypeMasterId, stateMasterId, districtMasterId, assemblyMasterId, fourthLevelMasterId);
        }

        public async Task<List<DashboardConnectedUser>> DashboardConnectedUser(DahboardMastersId dashboardMastersId, string roleType)
        {
            return await _eamsRepository.DashboardConnectedUser(dashboardMastersId, roleType);
        }


        #endregion

        #region SlotManagement
        public async Task<Response> AddEventSlot(List<SlotManagementMaster> addEventSlot)
        {
            return await _eamsRepository.AddEventSlot(addEventSlot);
        }

        public async Task<List<SlotManagementMaster>> GetEventSlotList(int stateMasterId, int electionTypeMasterId, int EventId)
        {
            return await _eamsRepository.GetEventSlotList(stateMasterId, electionTypeMasterId, EventId);
        }
        public async Task<List<SlotManagementMaster>> GetEventSlotListByEventAbbr(int stateMasterId, int electionTypeMasterId, string eventAbbr)

        {
            return await _eamsRepository.GetEventSlotListByEventAbbr(stateMasterId, electionTypeMasterId, eventAbbr);
        }
         #endregion

        #region UserList
        public async Task<List<UserList>> GetUserList(string userName, string type)
        {
            List<UserList> list = new List<UserList>();
            if (type == "SO")
            {

                return list = await _eamsRepository.GetUserList(userName, type);


            }
            else if (type == "ARO")
            {

                var aroUsers = await _authRepository.FindUserListByName(userName);
                foreach (var user in aroUsers)
                {
                    var mappedUser = new UserList
                    {
                        Name = user.UserName,
                        MobileNumber = user.PhoneNumber
                        // Map other properties as needed
                    };

                    list.Add(mappedUser);
                }

                return list;

            }

            return list.ToList();
        }


        #endregion

        #region PollInterruption Interruption
        //public async Task<Response> AddPollInterruption(PollInterruption pollInterruption)
        //{
        //    // Fetch the booth master record based on the provided BoothMasterId
        //    var boothMasterRecord = await _eamsRepository.GetBoothRecord(Convert.ToInt32(pollInterruption.BoothMasterId));
        //    if (boothMasterRecord == null)
        //    {
        //        return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth Record Not Found" };
        //    }

        //    bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString());
        //    bool isResumeformat = IsHHmmFormat(pollInterruption.ResumeTime.ToString());

        //    // Declare time comparison variables
        //    bool stopTimeIsLessEqualToCurrentTime = true; // Default to true
        //    bool resumeTimeIsLessEqualToCurrentTime = true; // Default to true
        //    bool compareStopAndResumeTime = true; // Default to true

        //    if (isStopformat && !isResumeformat)
        //    {
        //        stopTimeIsLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
        //    }

        //    if (!isStopformat && isResumeformat)
        //    {
        //        resumeTimeIsLessEqualToCurrentTime = ResumeTimeConvertTimeOnly(pollInterruption.ResumeTime.ToString());
        //    }

        //    if (isStopformat && isResumeformat)
        //    {
        //        compareStopAndResumeTime = CompareStopandResumeTime(pollInterruption.StopTime.ToString(), pollInterruption.ResumeTime.ToString());
        //    }

        //    if (!stopTimeIsLessEqualToCurrentTime)
        //    {
        //        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };
        //    }
        //    else if (!resumeTimeIsLessEqualToCurrentTime)
        //    {
        //        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be greater than Current Time" };
        //    }
        //    else if (!compareStopAndResumeTime)
        //    {
        //        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be less than Stop Time" };
        //    }
        //    var pollInterruptionRecord = await _eamsRepository.GetPollInterruptionData(pollInterruption.BoothMasterId.ToString());
        //    if (pollInterruptionRecord == null) // if no poll added in table
        //    {
        //        ElectionInfoMaster electionInfoRecord = await _eamsRepository.GetElectionInfoRecord(boothMasterRecord.BoothMasterId);
        //        if (electionInfoRecord != null)
        //        {
        //            PollInterruption pollInterruptionData = new PollInterruption
        //            {
        //                StateMasterId = boothMasterRecord.StateMasterId,
        //                DistrictMasterId = boothMasterRecord.DistrictMasterId,
        //                AssemblyMasterId = boothMasterRecord.AssemblyMasterId,
        //                BoothMasterId = boothMasterRecord.BoothMasterId,
        //                StopTime = isStopformat ? TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture) : (TimeOnly?)null,
        //                ResumeTime = isResumeformat ? TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture) : (TimeOnly?)null,
        //                InterruptionType = pollInterruption.InterruptionType,
        //                Flag = isStopformat && isResumeformat ? InterruptionCategory.Both.ToString() :
        //               isStopformat ? InterruptionCategory.Stop.ToString() :
        //               isResumeformat ? InterruptionCategory.Resume.ToString() : null,
        //                CreatedAt = BharatDateTime(),
        //                UpdatedAt = BharatDateTime(),
        //                IsPollInterrupted = false,
        //                Remarks = pollInterruption.Remarks
        //            };

        //            if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)
        //            {
        //                if (pollInterruption.OldCU != null && pollInterruption.OldBU != null && pollInterruption.NewBU != null && pollInterruption.NewCU != null)
        //                {
        //                    pollInterruptionData.NewCU = pollInterruption.NewCU;
        //                    pollInterruptionData.NewBU = pollInterruption.NewBU;
        //                    pollInterruptionData.OldCU = pollInterruption.OldCU;
        //                    pollInterruptionData.OldBU = pollInterruption.OldBU;

        //                    var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
        //                    return result;
        //                }
        //                else
        //                {
        //                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter Old CU, Old BU, New CU & New BU Value" };
        //                }
        //            }
        //            else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder ||
        //                     (InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.Other)
        //            {
        //                var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
        //                return result;
        //            }
        //            else
        //            {
        //                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
        //            }
        //        }
        //    }


        //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Election info record not found" };
        //}

        public async Task<Response> AddPollInterruption(PollInterruption pollInterruption)
        {
            var latestPollInterruptionRecord = await _eamsRepository.GetPollInterruptionData(pollInterruption.BoothMasterId.ToString());
            // Fetch the booth master record based on the provided BoothMasterId
            var boothMasterRecord = await _eamsRepository.GetBoothRecord(Convert.ToInt32(pollInterruption.BoothMasterId));
            if (boothMasterRecord == null)
            {
                return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth Record Not Found" };
            }

            bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString());
            bool isResumeformat = IsHHmmFormat(pollInterruption.ResumeTime.ToString());

            // Declare time comparison variables
            bool stopTimeIsLessEqualToCurrentTime = true; // Default to true
            bool resumeTimeIsLessEqualToCurrentTime = true; // Default to true
            bool compareStopAndResumeTime = true; // Default to true

            if (isStopformat && !isResumeformat)
            {
                stopTimeIsLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
            }

            if (!isStopformat && isResumeformat)
            {
                resumeTimeIsLessEqualToCurrentTime = ResumeTimeConvertTimeOnly(pollInterruption.ResumeTime.ToString());
            }

            if (isStopformat && isResumeformat)
            {
                compareStopAndResumeTime = CompareStopandResumeTime(pollInterruption.StopTime.ToString(), pollInterruption.ResumeTime.ToString());
            }

            if (!stopTimeIsLessEqualToCurrentTime)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };
            }
            else if (!resumeTimeIsLessEqualToCurrentTime)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be greater than Current Time" };
            }
            else if (!compareStopAndResumeTime)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be less than Stop Time" };
            }


            PollInterruption pollInterruptionData = new PollInterruption
            {
                StateMasterId = boothMasterRecord.StateMasterId,
                DistrictMasterId = boothMasterRecord.DistrictMasterId,
                AssemblyMasterId = boothMasterRecord.AssemblyMasterId,
                BoothMasterId = boothMasterRecord.BoothMasterId,
                InterruptionType = pollInterruption.InterruptionType,
                // Flag = isStopformat && isResumeformat ? InterruptionCategory.Both.ToString() :
                //isStopformat ? InterruptionCategory.Stop.ToString() :
                //isResumeformat ? InterruptionCategory.Resume.ToString() : null,
                CreatedAt = BharatDateTime(),
                Remarks = pollInterruption.Remarks,
                ElectionTypeMasterId = pollInterruption.ElectionTypeMasterId

            };
            if (isStopformat == true)
            {
                pollInterruptionData.StopTime = pollInterruption.StopTime;
                pollInterruptionData.IsPollInterrupted = true;
                pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                boothMasterRecord.IsBoothInterrupted = true;
                await _eamsRepository.UpdateBooth(boothMasterRecord);
            }
            if (isResumeformat == true)
            {
                pollInterruptionData.StopTime = latestPollInterruptionRecord.StopTime;
                pollInterruptionData.ResumeTime = pollInterruption.ResumeTime;
                pollInterruptionData.IsPollInterrupted = false;
                pollInterruptionData.Flag = InterruptionCategory.Resume.ToString();
                boothMasterRecord.IsBoothInterrupted = false;
                await _eamsRepository.UpdateBooth(boothMasterRecord);
            }
            if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)
            {
                if (isStopformat)
                {
                    if (pollInterruption.OldCU != null && pollInterruption.OldBU != null)
                    {
                        pollInterruptionData.NewCU = pollInterruption.NewCU;
                        pollInterruptionData.NewBU = pollInterruption.NewBU;
                        pollInterruptionData.OldCU = pollInterruption.OldCU;
                        pollInterruptionData.OldBU = pollInterruption.OldBU;

                        return await _eamsRepository.AddPollInterruption(pollInterruptionData);

                    }
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter Old CU, Old BU Value" };
                }

                if (isResumeformat)
                {
                    if (pollInterruption.NewCU != null && pollInterruption.NewBU != null)
                    {
                        pollInterruptionData.NewCU = pollInterruption.NewCU;
                        pollInterruptionData.NewBU = pollInterruption.NewBU;
                        pollInterruptionData.OldCU = pollInterruption.OldCU;
                        pollInterruptionData.OldBU = pollInterruption.OldBU;

                        return await _eamsRepository.AddPollInterruption(pollInterruptionData);

                    }
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter New CU & New BU Value" };
                }
            }
            else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder ||
                     (InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.Other)
            {
                return await _eamsRepository.AddPollInterruption(pollInterruptionData);

            }

            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not valid" };


        }


        public async Task<PollInterruption> GetPollInterruption(string boothMasterId)
        {
            var res = await _eamsRepository.GetPollInterruptionData(boothMasterId);
            var boothExists = await _eamsRepository.GetBoothRecord(Convert.ToInt32(boothMasterId));
            if (res.StopTime != null && res.ResumeTime != null)
            {
                PollInterruption pollInterruptionData = new PollInterruption()
                {
                    StateMasterId = res.StateMasterId,
                    DistrictMasterId = res.DistrictMasterId,
                    AssemblyMasterId = res.AssemblyMasterId,
                    BoothMasterId = res.BoothMasterId,
                    StopTime = res.StopTime,
                    ResumeTime = res.ResumeTime,
                    PollInterruptionId = res.PollInterruptionId,
                    InterruptionType = res.InterruptionType,
                    Flag = "New",
                    UpdatedAt = res.UpdatedAt,
                    IsPollInterrupted = false,


                };
                return pollInterruptionData;
            }
            else if (res.StopTime != null && res.ResumeTime == null)
            {
                PollInterruption pollInterruptionData = new PollInterruption()
                {
                    StateMasterId = res.StateMasterId,
                    DistrictMasterId = res.DistrictMasterId,
                    AssemblyMasterId = res.AssemblyMasterId,
                    BoothMasterId = res.BoothMasterId,
                    StopTime = res.StopTime,
                    ResumeTime = res.ResumeTime,
                    PollInterruptionId = res.PollInterruptionId,
                    InterruptionType = res.InterruptionType,
                    Flag = "Resume",
                    UpdatedAt = res.UpdatedAt,
                    IsPollInterrupted = true,


                };
                return pollInterruptionData;
            }
            else if (res == null)
            {
                PollInterruption pollInterruptionData = new PollInterruption()
                {
                    StateMasterId = res.StateMasterId,
                    DistrictMasterId = res.DistrictMasterId,
                    AssemblyMasterId = res.AssemblyMasterId,
                    BoothMasterId = res.BoothMasterId,
                    StopTime = res.StopTime,
                    ResumeTime = res.ResumeTime,
                    PollInterruptionId = res.PollInterruptionId,
                    InterruptionType = res.InterruptionType,
                    Flag = "Initial",
                    IsPollInterrupted = false,


                };
                return pollInterruptionData;
            }
            else
            {
                PollInterruption pollInterruptionData = new PollInterruption()
                {
                    StateMasterId = boothExists.StateMasterId,
                    DistrictMasterId = boothExists.DistrictMasterId,
                    AssemblyMasterId = boothExists.AssemblyMasterId,
                    BoothMasterId = boothExists.BoothMasterId,
                    StopTime = null,
                    ResumeTime = null,
                    Flag = "Initial",
                    IsPollInterrupted = false,


                };
                return pollInterruptionData;

            }
        }

        public async Task<PollInterruption> GetPollInterruptionbyId(string boothMasterId)
        {
            return await _eamsRepository.GetPollInterruptionData(boothMasterId);
        }

        public async Task<List<PollInterruptionHistoryModel>> GetPollInterruptionHistoryById(string boothMasterId)
        {
            return await _eamsRepository.GetPollInterruptionHistoryById(boothMasterId);
        }
        public string GetInterruptionReason(string reason)
        {
            int interruptionreason = Convert.ToInt16(reason);
            string reasonStatus = "";
            if ((InterruptionReason)interruptionreason == InterruptionReason.EVMFault)
            {
                reasonStatus = InterruptionReason.EVMFault.ToString();



            }
            else if ((InterruptionReason)interruptionreason == InterruptionReason.LawAndOrder)
            {
                reasonStatus = InterruptionReason.LawAndOrder.ToString();
            }
            else
            {
                reasonStatus = "";
            }
            return reasonStatus;
        }

        public async Task<List<PollInterruptionDashboard>> GetPollInterruptionDashboard(ClaimsIdentity claimsIdentity)
        {
            return await _eamsRepository.GetPollInterruptionDashboard(claimsIdentity);
        }


        public async Task<int> GetPollInterruptionDashboardCount(string role, int electionTypeMasterId, int stateMasterId, int? districtMasterId, int? assemblyMasterId, int? fourthLevelMasterId)
        {
            return await _eamsRepository.GetPollInterruptionDashboardCount(role, electionTypeMasterId, stateMasterId, districtMasterId, assemblyMasterId, fourthLevelMasterId);
        }

        public async Task<List<PollInterruptionDashboard>> GetBoothListBySoIdfoInterruption(ClaimsIdentity claimsIdentity)
        {
            return await _eamsRepository.GetBoothListBySoIdfoInterruption(claimsIdentity);
        }

        #endregion

        #region common Methods Event Activity
        static bool IsHHmmFormat(string timeString)
        {
            DateTime dummyDate; // A dummy date to use for parsing
            return DateTime.TryParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dummyDate);
        }
        /// <summary>
        /// Checking stop time is less then current Time
        /// </summary>
        /// <param name="stopTime"></param>
        /// <returns></returns>
        static bool StopTimeConvertTimeOnly(string stopTime)
        {

            DateTime currentTime = DateTime.Now;
            //DateTime currentTime = DateTime.SpecifyKind(currentTimes.ToUniversalTime(), DateTimeKind.Utc);
            DateTime stopTimeConvert = DateTime.ParseExact(stopTime, "HH:mm", CultureInfo.InvariantCulture);
            TimeOnly stopTimeConverttime = TimeOnly.ParseExact(stopTime, "HH:mm", CultureInfo.InvariantCulture);
            if (stopTimeConvert <= currentTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checking Resume time is less then current Time
        /// </summary>
        /// <param name="resumeTime"></param>
        /// <returns></returns>
        static bool ResumeTimeConvertTimeOnly(string resumeTime)
        {

            DateTime currentTime = DateTime.Now;
            //DateTime currentTime = DateTime.SpecifyKind(currentTimes.ToUniversalTime(), DateTimeKind.Utc);
            DateTime resumeTimeConvert = DateTime.ParseExact(resumeTime, "HH:mm", CultureInfo.InvariantCulture);
            TimeOnly resumeTimeConvertTime = TimeOnly.ParseExact(resumeTime, "HH:mm", CultureInfo.InvariantCulture);
            if (resumeTimeConvert <= currentTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Compare Stop and ResumeTime
        /// </summary>
        /// <param name="stopTime"></param>
        /// <param name="resumeTime"></param>
        /// <returns></returns>
        static bool CompareStopandResumeTime(string stopTime, string resumeTime)
        {

            DateTime currentTime = DateTime.Now;
            //DateTime currentTime = DateTime.SpecifyKind(currentTimes.ToUniversalTime(), DateTimeKind.Utc);
            DateTime stopTimeConvert = DateTime.ParseExact(stopTime, "HH:mm", CultureInfo.InvariantCulture);
            //TimeOnly stopTimeConvertTime = TimeOnly.ParseExact(stopTime, "HH:mm", CultureInfo.InvariantCulture);

            DateTime resumeTimeConvert = DateTime.ParseExact(resumeTime, "HH:mm", CultureInfo.InvariantCulture);
            // TimeOnly resumeTimeConvertTime = TimeOnly.ParseExact(resumeTime, "HH:mm", CultureInfo.InvariantCulture);
            if (resumeTimeConvert >= stopTimeConvert)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static bool CheckLastResumeTime(TimeOnly? InterruptionRecordResumeTime, string newStopTime)
        {


            TimeOnly newStopTimeConverttime = TimeOnly.ParseExact(newStopTime, "HH:mm", CultureInfo.InvariantCulture);
            if (InterruptionRecordResumeTime >= newStopTimeConverttime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool CheckLastResumeTime2(TimeOnly? InterruptionRecordResumeTime, string newStopTime)
        {


            TimeOnly newStopTimeConverttime = TimeOnly.ParseExact(newStopTime, "HH:mm", CultureInfo.InvariantCulture);
            if (newStopTimeConverttime >= InterruptionRecordResumeTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool CheckLastStopTime(TimeOnly? InterruptionRecordStopTime, string newResumeTime)
        {


            TimeOnly newResumeTimeConverttime = TimeOnly.ParseExact(newResumeTime, "HH:mm", CultureInfo.InvariantCulture);
            if (newResumeTimeConverttime >= InterruptionRecordStopTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region PSOFORM
        public async Task<Response> AddPSOForm(PollingStationMaster pollingStationMaster, ClaimsIdentity claimsIdentity)
        {
            return await _eamsRepository.AddPSOForm(pollingStationMaster, claimsIdentity);
        }

        public async Task<bool> PollingStationRecord(Int32 boothMasterId)
        {
            return await _eamsRepository.PollingStationRecord(boothMasterId);
        }

        public async Task<bool> GetPollingStationRecordById(int? psMasterId)
        {
            return await _eamsRepository.GetPollingStationRecordById(psMasterId);
        }

        public async Task<List<PSFormViewModel>> GetPSFormRecordbyPSId(string pollingStationMasterId)
        {
            return await _eamsRepository.GetPSFormRecordbyPSId(pollingStationMasterId);
        }

        public async Task<List<PSoFomListView>> GetPSOlistbyARO(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetPSOlistbyARO(stateMasterId, districtMasterId, assemblyMasterId);
        }


        public async Task<List<PSoFomCombined>> GetPsoFormDetail(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetPsoFormDetail(stateMasterId, districtMasterId, assemblyMasterId);
        }


        public async Task<Response> UpdatePSoForm(PollingStationMaster psMaster, ClaimsIdentity claimsIdentity)
        {
            return await _eamsRepository.UpdatePSoForm(psMaster, claimsIdentity);

        }
        public async Task<List<LocationModel>> GetLocationMasterById(string locationMasterId)
        {
            return await _eamsRepository.GetLocationMasterById(locationMasterId);
        }
        public async Task<Response> AddLocation(LocationModel locationModel)
        {
            return await _eamsRepository.AddLocation(locationModel);
        }

        public async Task<Response> UpdateLocation(LocationModel locationModel)
        {
            return await _eamsRepository.UpdateLocation(locationModel);
        }
        #endregion

        #region Reports
        public async Task<List<ConsolidatePanchResultDeclarationReportList>> GetConsolidatedPanchResultDeclarationReport(ResultDeclaration resultDeclaration)
        {

            return await _eamsRepository.GetConsolidatedPanchResultDeclarationReport(resultDeclaration);
        }
        public async Task<List<ConsolidatedUnOpposedPanchSarPanchAndNoKycCandidateReportList>> GetConsolidatedUnOppossedPanchResultDeclarationReport(ResultDeclaration resultDeclaration)
        {

            return await _eamsRepository.GetConsolidatedUnOppossedPanchResultDeclarationReport(resultDeclaration);
        }
        public async Task<List<ConsolidatedUnOpposedPanchSarPanchAndNoKycCandidateReportList>> GetConsolidatedNoKycPanchResultDeclarationReport(ResultDeclaration resultDeclaration)
        {

            return await _eamsRepository.GetConsolidatedNoKycPanchResultDeclarationReport(resultDeclaration);
        }
        public async Task<List<ConsolidateSarPanchResultDeclarationReportList>> GetConsolidatedSarPanchResultDeclarationReport(ResultDeclarationReportListModel resultDeclaration)
        {

            return await _eamsRepository.GetConsolidatedSarPanchResultDeclarationReport(resultDeclaration);
        }
        public async Task<List<ConsolidatedUnOpposedPanchSarPanchAndNoKycCandidateReportList>> GetConsolidatedUnOppossedSarPanchResultDeclarationReport(ResultDeclarationReportListModel resultDeclaration)
        {

            return await _eamsRepository.GetConsolidatedUnOppossedSarPanchResultDeclarationReport(resultDeclaration);
        }

        public async Task<List<ConsolidatedUnOpposedPanchSarPanchAndNoKycCandidateReportList>> GetConsolidatedNoKycSarPanchResultDeclarationReport(ResultDeclarationReportListModel resultDeclaration)
        {

            return await _eamsRepository.GetConsolidatedNoKycSarPanchResultDeclarationReport(resultDeclaration);
        }
        public async Task<List<ConsolidatePanchResultDeclarationReportList>> GetConsolidatedElectedPanchResultDeclarationReport(ResultDeclaration resultDeclaration)
        {

            return await _eamsRepository.GetConsolidatedElectedPanchResultDeclarationReport(resultDeclaration);
        }
        public async Task<List<ConsolidateSarPanchResultDeclarationReportList>> GetConsolidatedElectedSarPanchResultDeclarationReport(ResultDeclarationReportListModel resultDeclaration)
        {

            return await _eamsRepository.GetConsolidatedElectedSarPanchResultDeclarationReport(resultDeclaration);
        }
        public async Task<List<ConsolidateBoothReport>> GetConsolidateBoothReports(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetConsolidateBoothReports(boothReportModel);
        }
        public async Task<List<ConsolidateBoothReport>> GetConsolidateGPWardReports(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetConsolidateGPWardReports(boothReportModel);
        }
        public async Task<List<SoReport>> GetSOReport(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetSOReport(boothReportModel);
        }
        public async Task<List<SoReport>> GetPendingSOReport(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetPendingSOReport(boothReportModel);
        }
        public async Task<List<AssemblyWisePendingBooth>> GetAssemblyWisePendingReports(string stateMasterId)
        {

            return await _eamsRepository.GetAssemblyWisePendingReports(stateMasterId);
        }
        public async Task<List<VTPSReportReportModel>> GetVoterTurnOutPollingStationReports(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetVoterTurnOutPollingStationReports(boothReportModel);
        }
        public async Task<List<VTReportModel>> GetVoterTurnOutConsolidatedReports(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetVoterTurnOutConsolidatedReports(boothReportModel);
        }
        public async Task<List<VTReportModel>> GetSlotBasedVoterTurnOutReport(SlotVTReportModel boothReportModel)
        {

            return await _eamsRepository.GetSlotBasedVoterTurnOutReport(boothReportModel);
        }
        public async Task<List<VoterTurnOutSlotWise>> GetVoterTurnOutSlotBasedReport(string stateMasterId, string electionTypeMasterId)
        {

            return await _eamsRepository.GetVoterTurnOutSlotBasedReport(stateMasterId, electionTypeMasterId);
        }
        public async Task<List<AssemblyVoterTurnOutSlotWise>> GetSlotVTReporttAssemblyWise(string stateMasterId, string districtMasterId, string electionTypeMasterId)
        {

            return await _eamsRepository.GetSlotVTReporttAssemblyWise(stateMasterId, districtMasterId, electionTypeMasterId);
        }
        public async Task<List<BoothWiseVoterTurnOutSlotWise>> GetSlotVTReportBoothWise(string stateMasterId, string districtMasterId, string assemblyId, string electionTypeMasterId)
        {

            return await _eamsRepository.GetSlotVTReportBoothWise(stateMasterId, districtMasterId, assemblyId, electionTypeMasterId);
        }



        #endregion

        #region Location Master
        public async Task<List<LocationModelList>> GetLocationMasterforARO(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetLocationMasterforARO(stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<List<LocationModelList>> GetLocationMasterforALL(BoothReportModel boothReportModel)
        {
            return await _eamsRepository.GetLocationMasterforALL(boothReportModel);
        }

        #endregion




        #region HelpDesk
        public async Task<Response> AddHelpDeskInfo(HelpDeskDetail helpDeskDetail)
        {
            return await _eamsRepository.AddHelpDeskInfo(helpDeskDetail);
        }
        public async Task<List<HelpDeskDetail>> GetHelpDeskDetail(string asid)
        {
            return await _eamsRepository.GetHelpDeskDetail(asid);
        }
        #endregion

        #region HelpDesk
        public async Task<List<SectorOfficerPendency>> GetDistrictWiseSOCountEventWiseCount(string stateMasterId)
        {

            return await _eamsRepository.GetDistrictWiseSOCountEventWiseCount(stateMasterId);
        }
        public async Task<List<SectorOfficerPendencyAssembly>> GetAssemblyWiseSOCountEventWiseCount(string stateMasterId, string districtmasterid, string electionTypeMasterId)
        {

            return await _eamsRepository.GetAssemblyWiseSOCountEventWiseCount(stateMasterId, districtmasterid, electionTypeMasterId);
        }
        //public async Task<List<SectorOfficerPendencyBooth>> GetBoothWiseSOEventWiseCount(string stateMasterId, string districtmasterid, string assemblyMasterid)
        //{

        //    return await _eamsRepository.GetBoothWiseSOEventWiseCount(stateMasterId, districtmasterid, assemblyMasterid);
        //}

        public async Task<List<SectorOfficerPendencyBooth>> GetBoothWiseSOEventWiseCount(string soMasterId)
        {

            return await _eamsRepository.GetBoothWiseSOEventWiseCount(soMasterId);
        }
        public async Task<List<SectorOfficerPendencybySoNames>> GetSONamesEventWiseCount(string stateMasterId, string districtmasterid, string assemblyMasterid)
        {

            return await _eamsRepository.GetSONamesEventWiseCount(stateMasterId, districtmasterid, assemblyMasterid);
        }



        #endregion

        #region QueueManagement
        public async Task<ServiceResponse> AddQueue(QIS addQIS)
        {
            addQIS.PCMasterId = await _eamsRepository.GetPCMasterIdByAssemblyIdCode(addQIS.AssemblyMasterId.ToString());

            return await _eamsRepository.AddQueue(addQIS);
        }
        public async Task<QIS> GetQISList(string stateMasterId, string districtMasterId, string assemblyMasterId, string boothMasterId)
        {
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null)
            {
                return await _eamsRepository.GetQISList(Convert.ToInt32(stateMasterId), Convert.ToInt32(districtMasterId), Convert.ToInt32(assemblyMasterId), Convert.ToInt32(boothMasterId));
            }
            return null;

        }
        #endregion

        #region BLOBoothMaster
        public async Task<List<BoothMaster>> GetBLOBoothById(string bloBoothMasterId)
        {
            return await _eamsRepository.GetBLOBoothById(bloBoothMasterId);
        }
        public async Task<List<CombinedMaster>> GetBlosListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetBlosListById(stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<Response> BLOBoothMapping(List<BoothMaster> boothMaster)
        {
            return await _eamsRepository.BLOBoothMapping(boothMaster);
        }
        public async Task<List<CombinedMaster>> GetAssignedBoothListByBLOId(string stateMasterId, string districtMasterId, string assemblyMasterId, string bloId)
        {
            return await _eamsRepository.GetAssignedBoothListByBLOId(stateMasterId, districtMasterId, assemblyMasterId, bloId);
        }
        public async Task<List<CombinedMaster>> GetUnassignedBLOBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetUnassignedBLOBoothListById(stateMasterId, districtMasterId, assemblyMasterId);
        }

        public async Task<BLOOfficerCustom> GetBLObyId(string bloMasterId)
        {
            return await _eamsRepository.GetBLObyId(bloMasterId);
        }
        #endregion

        #region Randomization

        public async Task<ServiceResponse> AddRandomization(PPR pPR)
        {
            return await _eamsRepository.AddRandomization(pPR);
        }
        public async Task<int> GetRoundCountByRandomizationTaskId(int? randomizationTaskId, int? stateMasterId)
        {
            return await _eamsRepository.GetRoundCountByRandomizationTaskId(randomizationTaskId, stateMasterId);
        }
        public async Task<int> GetCurrentRoundByRandomizationById(int? stateMasterId, int? districtmasterId, int? randomizationTaskDetailMasterId)
        {
            return await _eamsRepository.GetCurrentRoundByRandomizationById(stateMasterId, districtmasterId, randomizationTaskDetailMasterId);
        }
        public async Task<ServiceResponse> AddRandomizationTaskDetail(RandomizationTaskDetail randomizationTaskDetail)
        {
            return await _eamsRepository.AddRandomizationTaskDetail(randomizationTaskDetail);
        }

        public async Task<List<RandomizationList>> GetRandomizationListByStateId(int stateMasterId)
        {
            return await _eamsRepository.GetRandomizationListByStateId(stateMasterId);
        }
        public async Task<RandomizationList> GetRandomizationById(int pprMasterId)
        {
            return await _eamsRepository.GetRandomizationById(pprMasterId);
        }
        public async Task<List<RandomizationTableList>> GetRandomizationTableListByStateId(int stateMasterId)
        {
            return await _eamsRepository.GetRandomizationTableListByStateId(stateMasterId);
        }
        public async Task<RandomizationTableList> GetRandomizationListByDistrictId(int stateMasterId, int districtMasterId)
        {
            return await _eamsRepository.GetRandomizationListByDistrictId(stateMasterId, districtMasterId);
        }
        public async Task<ServiceResponse> UpdateRandomizationById(PPR pPR)
        {
            return await _eamsRepository.UpdateRandomizationById(pPR);
        }
        public async Task<List<RandomizationTaskDetail>> GetRandomizationTaskListByStateId(int stateMasterId)
        {
            return await _eamsRepository.GetRandomizationTaskListByStateId(stateMasterId);
        }


        #endregion

        #region GetBoothByLocation
        public async Task<List<CombinedMaster>> GetBoothByLocation(string latitude, string longitude)
        {
            return await _eamsRepository.GetBoothByLocation(latitude, longitude);
        }
        #endregion

        #region BLO
        public async Task<List<BLOBoothAssignedQueueCount>> GetBLOQueueCount(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetBLOQueueCount(boothReportModel);
        }
        public async Task<List<BLOBoothAssignedQueueCount>> GetUnassignedBLOs(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetUnassignedBLOs(boothReportModel);
        }
        public async Task<List<BLOBoothAssignedQueueCount>> GetAssignedBLOs(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetAssignedBLOs(boothReportModel);
        }

        public async Task<List<BLOBoothAssignedQueueCount>> GetBLOQueueCountOpen(string statemasterid, string districtmasterid)
        {

            return await _eamsRepository.GetBLOQueueCountOpen(statemasterid, districtmasterid);
        }
        #endregion

        #region Mobile Version
        public async Task<MobileVersion> GetMobileVersionById(string stateMasterId)
        {
            return await _eamsRepository.GetMobileVersionById(stateMasterId);
        }

        public async Task<ServiceResponse> AddMobileVersion(MobileVersion mobileVersion)
        {
            return await _eamsRepository.AddMobileVersion(mobileVersion);
        }


        #endregion

        #region KYC Public Details
        public async Task<ServiceResponse> AddKYCDetails(Kyc kyc)
        {
            if (!int.TryParse(kyc.Age, out int age) || age < 21)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Age must be 21 or above." };
            }
            if (kyc.ElectionTypeMasterId == 1)
            {
                return await _eamsRepository.AddKYCDetailsForGP(kyc);
            }
            // ElectionTypeMasterId == 4 For "Municipal Corporation","Municipal Council" and "Nagar Panchayat"
            else if (kyc.ElectionTypeMasterId == 4 || kyc.ElectionTypeMasterId == 5 || kyc.ElectionTypeMasterId == 6)
            {
                return await _eamsRepository.AddKYCDetailsForMCorpMCounAndNP(kyc);
            }
            return null;
        }
        public async Task<ServiceResponse> UpdateKycDetails(Kyc kyc)
        {
            if (!int.TryParse(kyc.Age, out int age) || age < 21)
            {
                return new ServiceResponse { IsSucceed = false, Message = "Age must be 21 or above." };
            }
            // ElectionTypeMasterId == 1 For "Gram Panchayats"
            if (kyc.ElectionTypeMasterId == 1)
            {
                return await _eamsRepository.UpdateKycDetailsForGP(kyc);
            }
            // ElectionTypeMasterId == 4 For "Municipal Corporation","Municipal Council" and "Nagar Panchayat"
            else if (kyc.ElectionTypeMasterId == 4 || kyc.ElectionTypeMasterId == 5 || kyc.ElectionTypeMasterId == 6)
            {
                return await _eamsRepository.UpdateKycDetailsForMCorpMCounAndNP(kyc);
            }
            return null;
        }

        public async Task<List<Kyc>> GetKYCDetails()
        {
            return await _eamsRepository.GetKYCDetails();

        }
        public async Task<KycList> GetKycById(int kycMasterId)
        {
            return await _eamsRepository.GetKycById(kycMasterId);

        }
        public async Task<List<KycList>> GetKYCDetailByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            return await _eamsRepository.GetKYCDetailByAssemblyId(electionType, stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<List<KycList>> GetKYCDetailByFourthAndWardId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelMasterId, int? wardMasterId)
        {
            return await _eamsRepository.GetKYCDetailByFourthAndWardId(electionType, stateMasterId, districtMasterId, assemblyMasterId, fourthLevelMasterId, wardMasterId);
        }
        public async Task<List<KycList>> GetKYCDetailByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId, string userId)
        {
            return await _eamsRepository.GetKYCDetailByAssemblyId(electionType, stateMasterId, districtMasterId, assemblyMasterId, userId);
        }
        public async Task<ServiceResponse> DeleteKycById(int kycMasterId)
        {
            return await _eamsRepository.DeleteKycById(kycMasterId);
        }
        #endregion

        #region UnOpposed Public Details
        public async Task<ServiceResponse> AddUnOpposedDetails(UnOpposed unOpposed)
        {
            return await _eamsRepository.AddUnOpposedDetails(unOpposed);
        }
        public async Task<List<UnOpposed>> GetUnOpposedDetails()
        {
            return await _eamsRepository.GetUnOpposedDetails();

        }
        public async Task<List<UnOpposedList>> GetUnOpposedDetailsByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            return await _eamsRepository.GetUnOpposedDetailsByAssemblyId(electionType, stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<ServiceResponse> UpdateUnOpposedDetails(UnOpposed unOpposed)
        {
            return await _eamsRepository.UpdateUnOpposedDetails(unOpposed);
        }

        public async Task<UnOpposedList> GetUnOpposedById(int unOpposedMasterId)
        {
            return await _eamsRepository.GetUnOpposedById(unOpposedMasterId);
        }
        public async Task<ServiceResponse> DeleteUnOpposedById(int unOpposedMasterId)
        {
            return await _eamsRepository.DeleteUnOpposedById(unOpposedMasterId);
        }
        #endregion

        #region Election Type Master
        public async Task<List<ElectionTypeMaster>> GetAllElectionTypes()
        {
            return await _eamsRepository.GetAllElectionTypes();
        }
        #endregion

        #region FourthLevelH
        public async Task<Response> AddFourthLevelH(FourthLevelH fourthLevelH)
        {

            return await _eamsRepository.AddFourthLevelH(fourthLevelH);
        }
        public async Task<List<FourthLevelH>> GetFourthLevelHListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            return await _eamsRepository.GetFourthLevelHListById(stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<FourthLevelH> GetFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            return await _eamsRepository.GetFourthLevelHById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
        }
        public async Task<Response> UpdateFourthLevelH(FourthLevelH fourthLevelH)
        {
            return await _eamsRepository.UpdateFourthLevelH(fourthLevelH);
        }
        public async Task<ServiceResponse> DeleteFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            return await _eamsRepository.DeleteFourthLevelHById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
        }
        #endregion

        #region PSZonePanchayat
        public async Task<Response> AddPSZonePanchayat(PSZonePanchayat psZonePanchayat)
        {

            return await _eamsRepository.AddPSZonePanchayat(psZonePanchayat);
        }
        public async Task<List<PSZonePanchayat>> GetPSZonePanchayatListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            return await _eamsRepository.GetPSZonePanchayatListById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
        }
        public async Task<PSZonePanchayat> GetPSZonePanchayatById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int psZonePanchayatMasterId)
        {
            return await _eamsRepository.GetPSZonePanchayatById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId, psZonePanchayatMasterId);
        }
        public async Task<Response> UpdatePSZonePanchayat(PSZonePanchayat psZonePanchayat)
        {
            return await _eamsRepository.UpdatePSZonePanchayat(psZonePanchayat);
        }
        public async Task<ServiceResponse> DeletePSZonePanchayatById(int psZonePanchayatMasterId)
        {
            return await _eamsRepository.DeletePSZonePanchayatById(psZonePanchayatMasterId);
        }
        #endregion

        #region GPPanchayatWards
        public async Task<Response> AddGPPanchayatWards(GPPanchayatWards gpPanchayatWards)
        {

            return await _eamsRepository.AddGPPanchayatWards(gpPanchayatWards);
        }
        public async Task<List<GPPanchayatWards>> GetPanchayatWardforResultDeclaration(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            return await _eamsRepository.GetPanchayatWardforResultDeclaration(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
        }
        public async Task<Response> UpdateGPPanchayatWards(GPPanchayatWards gpPanchayatWards)
        {
            return await _eamsRepository.UpdateGPPanchayatWards(gpPanchayatWards);
        }

        public async Task<GPPanchayatWards> GetGPPanchayatWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int gpPanchayatWardsMasterId)
        {
            return await _eamsRepository.GetGPPanchayatWardsById(stateMasterId, districtMasterId, assemblyMasterId, FourthLevelHMasterId, gpPanchayatWardsMasterId);
        }

        public async Task<Response> DeleteGPPanchayatWardsById(int gpPanchayatWardsMasterId)
        {
            return await _eamsRepository.DeleteGPPanchayatWardsById(gpPanchayatWardsMasterId);
        }
        public async Task<List<GPPanchayatWards>> GetGPPanchayatWardsListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId)
        {
            return await _eamsRepository.GetGPPanchayatWardsListById(stateMasterId, districtMasterId, assemblyMasterId, FourthLevelHMasterId);
        }
        #endregion

        #region GPVoter
        public async Task<ServiceResponse> IsVoterAndKycExist(int fourthLevelMasterId)
        {
            return await _eamsRepository.IsVoterAndKycExist(fourthLevelMasterId);
        }
        public async Task<ServiceResponse> AddGPVoterDetails(GPVoter gpVoterPdf)
        {
            return await _eamsRepository.AddGPVoterDetails(gpVoterPdf);
        }

        public async Task<ServiceResponse> UpdateGPVoterDetails(GPVoter gpVoterPdf)
        {
            return await _eamsRepository.UpdateGPVoterDetails(gpVoterPdf);
        }
        public async Task<GPVoterList> GetGPVoterById(int gpVoterMasterId)
        {
            return await _eamsRepository.GetGPVoterById(gpVoterMasterId);
        }
        public async Task<List<GPVoterList>> GetGPVoterListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetGPVoterListById(stateMasterId, districtMasterId, assemblyMasterId, electionTypeMasterId);
        }
        public async Task<List<GPVoterList>> GetGPVoterListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId, string userId)
        {
            return await _eamsRepository.GetGPVoterListById(stateMasterId, districtMasterId, assemblyMasterId, electionTypeMasterId, userId);
        }
        public async Task<ServiceResponse> DeleteGPVoterById(int gpVoterMasterId)
        {
            return await _eamsRepository.DeleteGPVoterById(gpVoterMasterId);
        }

        public async Task<List<VoterType>> GetVoterTypeListById()
        {
            return await _eamsRepository.GetVoterTypeListById();
        }
        #endregion

        #region ResultDeclaration

        public async Task<ServiceResponseForRD> AddResultDeclarationDetails(List<ResultDeclaration> resultDeclaration)
        {
            if (resultDeclaration == null || !resultDeclaration.Any())
            {
                return new ServiceResponseForRD
                {
                    IsWinner = false,
                    IsDraw = false,
                    IsSucceed = false,
                    Message = "No data provided."
                };
            }
            if (resultDeclaration.Any(r => string.IsNullOrEmpty(r.VoteMargin.ToString()) && !r.IsDrawLottery))
            {
                return new ServiceResponseForRD
                {
                    IsSucceed = false,
                    Message = "All fields are required."
                };
            }
            // Fetch all candidate names for the given KycMasterIds
            var kycMasterIds = resultDeclaration.Select(r => r.KycMasterId).Distinct().ToList();
            var candidateNames = await _eamsRepository.GetCandidateNameByKycMasterId(kycMasterIds);

            // Filter valid results and sort by VoteMargin
            var validResults = resultDeclaration
                .Where(r => !string.IsNullOrEmpty(r.VoteMargin.ToString()))
                .OrderByDescending(r =>
                {
                    int voteMargin;
                    return int.TryParse(r.VoteMargin.ToString(), out voteMargin) ? voteMargin : 0;
                })
                .ToList();

            if (!validResults.Any())
            {
                return new ServiceResponseForRD
                {
                    IsWinner = false,
                    IsDraw = false,
                    IsSucceed = false,
                    Message = "No valid vote margins found."
                };
            }
            // Assuming you have a method to fetch the TotalVoters for the ward based on the provided parameters
            var totalVotersInWard = await _eamsRepository.GetTotalVotersForUrbanRDAsync(validResults.First().StateMasterId, validResults.First().DistrictMasterId, validResults.First().AssemblyMasterId, validResults.First().FourthLevelHMasterId);
            //if (  totalVotersInWard is null || totalVotersInWard is 0)
            //{
            //    return new ServiceResponseForRD
            //    {
            //        IsSucceed = false,
            //        Message = "There is no Voters Available"
            //    };
            //}
            // Calculate the sum of VoteMargin
            var totalVoteMargin = validResults.Sum(r => r.VoteMargin);

            // Check if sum of VoteMargin is less than or equal to total voters in the ward
            if (totalVoteMargin > totalVotersInWard)
            {
                return new ServiceResponseForRD
                {
                    IsSucceed = false,
                    Message = "Total Vote Margin is greater than total votes of the ward."
                };
            }
            // Determine highest vote margin
            var highestVoteMarginStr = validResults[0].VoteMargin;
            if (int.TryParse(highestVoteMarginStr.ToString(), out var highestVoteMargin))
            {
                var highestMarginCandidates = validResults
                    .Where(r => int.TryParse(r.VoteMargin.ToString(), out var margin) && margin == highestVoteMargin)
                    .ToList();

                // Case 4: Lottery Winner
                var lotteryCandidate = validResults.FirstOrDefault(c => c.IsDrawLottery && c.IsWinner == true);
                if (lotteryCandidate != null)
                {
                    lotteryCandidate.IsWinner = true;
                    var candidateName = candidateNames.GetValueOrDefault(lotteryCandidate.KycMasterId, "Unknown Candidate");
                    await _eamsRepository.AddResultDeclarationDetails(resultDeclaration);

                    return new ServiceResponseForRD
                    {
                        IsWinner = true,
                        IsDraw = false,
                        IsSucceed = true,
                        Message = $"Candidate {candidateName} won by lottery."
                    };
                }

                // Case 3: Recounting
                if (validResults.Any(c => c.IsReCounting))
                {
                    if (highestMarginCandidates.Count > 1)
                    {
                        foreach (var candidate in highestMarginCandidates)
                        {
                            candidate.IsDraw = true;
                        }
                        var candidateNamesList = highestMarginCandidates
                            .Select(c => candidateNames.GetValueOrDefault(c.KycMasterId, "Unknown Candidate"))
                            .ToList();
                        await _eamsRepository.AddResultDeclarationDetails(resultDeclaration);

                        return new ServiceResponseForRD
                        {
                            IsWinner = false,
                            IsDraw = true,
                            IsSucceed = false,
                            Message = $"Recount detected a draw situation between candidates: {string.Join(", ", candidateNamesList)}."
                        };
                    }
                    else
                    {
                        var candidateName = candidateNames.GetValueOrDefault(highestMarginCandidates[0].KycMasterId, "Unknown Candidate");
                        highestMarginCandidates[0].IsWinner = true;
                        await _eamsRepository.AddResultDeclarationDetails(resultDeclaration);

                        return new ServiceResponseForRD
                        {
                            IsWinner = true,
                            IsDraw = false,
                            IsSucceed = true,
                            Message = $"Candidate {candidateName} is the winner after recounting."
                        };
                    }
                }

                // Case 2: Draw Situation
                if (highestMarginCandidates.Count > 1)
                {
                    foreach (var candidate in highestMarginCandidates)
                    {
                        candidate.IsDraw = true;
                    }
                    var candidateNamesList = highestMarginCandidates
                        .Select(c => candidateNames.GetValueOrDefault(c.KycMasterId, "Unknown Candidate"))
                        .ToList();
                    await _eamsRepository.AddResultDeclarationDetails(resultDeclaration);

                    return new ServiceResponseForRD
                    {
                        IsWinner = false,
                        IsDraw = true,
                        IsSucceed = false,
                        Message = $"Draw situation detected between candidates: {string.Join(", ", candidateNamesList)}."
                    };
                }

                // Case 1: Single Winner
                if (highestMarginCandidates.Count == 1)
                {
                    var candidateName = candidateNames.GetValueOrDefault(highestMarginCandidates[0].KycMasterId, "Unknown Candidate");
                    highestMarginCandidates[0].IsWinner = true;
                    await _eamsRepository.AddResultDeclarationDetails(resultDeclaration);

                    return new ServiceResponseForRD
                    {
                        IsWinner = true,
                        IsDraw = false,
                        IsSucceed = true,
                        Message = $"Candidate {candidateName} is the winner with the highest vote margin."
                    };
                }
            }

            return await _eamsRepository.AddResultDeclarationDetails(resultDeclaration);
        }

        public async Task<ServiceResponseForRD> UpdateResultDeclarationForPortal(List<ResultDeclaration> resultDeclaration)
        {
            if (resultDeclaration == null || !resultDeclaration.Any())
            {
                return new ServiceResponseForRD { IsSucceed = false, Message = "No data provided." };
            }
            if (resultDeclaration.Any(r => string.IsNullOrEmpty(r.VoteMargin.ToString()) && !r.IsDrawLottery))
            {
                return new ServiceResponseForRD
                {
                    IsSucceed = false,
                    Message = "All fields are required."
                };
            }
            // Fetch all candidate names for the given KycMasterIds
            var kycMasterIds = resultDeclaration.Select(r => r.KycMasterId).Distinct().ToList();
            var candidateNames = await _eamsRepository.GetCandidateNameByKycMasterId(kycMasterIds);

            // Filter the records that have non-null and non-empty VoteMargin
            var validResults = resultDeclaration
                .Where(r => !string.IsNullOrEmpty(r.VoteMargin.ToString()))
                .OrderByDescending(r =>
                {
                    int voteMargin;
                    return int.TryParse(r.VoteMargin.ToString(), out voteMargin) ? voteMargin : 0;
                })
                .ToList();

            // Calculate the total vote margin
            var totalVoteMargin = validResults.Sum(r => r.VoteMargin);

            // Fetch total votes in the ward (this may require a method to fetch this data)
            var totalVotersInWard = await _eamsRepository.GetTotalVotersForUrbanRDAsync(validResults.First().StateMasterId, validResults.First().DistrictMasterId, validResults.First().AssemblyMasterId, validResults.First().FourthLevelHMasterId);

            // Check if the total vote margin exceeds the total votes in the ward
            if (totalVoteMargin > totalVotersInWard)
            {
                return new ServiceResponseForRD
                {
                    IsSucceed = false,
                    Message = "Total vote margin is greater than the total votes of the ward."
                };
            }

            if (validResults.Count > 0)
            {
                var highestVoteMarginStr = validResults[0].VoteMargin;
                if (int.TryParse(highestVoteMarginStr.ToString(), out var highestVoteMargin))
                {
                    var highestMarginCandidates = validResults
                        .Where(r => int.TryParse(r.VoteMargin.ToString(), out var margin) && margin == highestVoteMargin)
                        .ToList();

                    // Case 4: Lottery Winner
                    var lotteryCandidate = validResults.FirstOrDefault(c => c.IsDrawLottery && c.IsWinner == true);
                    if (lotteryCandidate != null)
                    {
                        lotteryCandidate.IsWinner = true;
                        var candidateName = candidateNames.GetValueOrDefault(lotteryCandidate.KycMasterId, "Unknown Candidate");
                        await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration);

                        return new ServiceResponseForRD
                        {
                            IsWinner = true,
                            IsDraw = false,
                            IsSucceed = true,
                            Message = $"Candidate {candidateName} won by lottery."
                        };
                    }

                    // Case 3: Recounting
                    if (validResults.Any(c => c.IsReCounting))
                    {
                        if (highestMarginCandidates.Count > 1)
                        {
                            foreach (var candidate in highestMarginCandidates)
                            {
                                candidate.IsDraw = true;
                            }
                            var candidateNamesList = highestMarginCandidates
                                .Select(c => candidateNames.GetValueOrDefault(c.KycMasterId, "Unknown Candidate"))
                                .ToList();

                            await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration);

                            return new ServiceResponseForRD
                            {
                                IsWinner = false,
                                IsDraw = true,
                                IsSucceed = false,
                                Message = $"Recount detected a draw situation between candidates: {string.Join(", ", candidateNamesList)}."
                            };
                        }
                        else
                        {
                            var candidateName = candidateNames.GetValueOrDefault(highestMarginCandidates[0].KycMasterId, "Unknown Candidate");
                            highestMarginCandidates[0].IsWinner = true;
                            await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration);

                            return new ServiceResponseForRD
                            {
                                IsWinner = true,
                                IsDraw = false,
                                IsSucceed = true,
                                Message = $"Candidate {candidateName} is the winner after recounting."
                            };
                        }
                    }

                    // Case 2: Draw Situation
                    if (highestMarginCandidates.Count > 1)
                    {
                        foreach (var candidate in highestMarginCandidates)
                        {
                            candidate.IsDraw = true;
                        }
                        var candidateNamesList = highestMarginCandidates
                            .Select(c => candidateNames.GetValueOrDefault(c.KycMasterId, "Unknown Candidate"))
                            .ToList();

                        await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration);

                        return new ServiceResponseForRD
                        {
                            IsWinner = false,
                            IsDraw = true,
                            IsSucceed = false,
                            Message = $"Draw situation detected between candidates: {string.Join(", ", candidateNamesList)}."
                        };
                    }

                    // Case 1: Single Winner
                    if (highestMarginCandidates.Count == 1)
                    {
                        var candidateName = candidateNames.GetValueOrDefault(highestMarginCandidates[0].KycMasterId, "Unknown Candidate");
                        highestMarginCandidates[0].IsWinner = true;
                        await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration);

                        return new ServiceResponseForRD
                        {
                            IsWinner = true,
                            IsDraw = false,
                            IsSucceed = true,
                            Message = $"Candidate {candidateName} is the winner with the highest vote margin."
                        };
                    }
                }
            }

            return await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration);
        }


        //public async Task<ServiceResponse> UpdateResultDeclarationForPortal(List<ResultDeclaration> resultDeclaration)
        //{
        //    if (resultDeclaration == null || !resultDeclaration.Any())
        //    {
        //        return new ServiceResponse { IsSucceed = false, Message = "No data provided." };
        //    }

        //    // Filter the records that have non-null and non-empty VoteMargin
        //    var validResults = resultDeclaration
        //        .Where(r => !string.IsNullOrEmpty(r.VoteMargin))
        //        .OrderByDescending(r =>
        //        {
        //            // Use TryParse to safely convert VoteMargin to int
        //            int voteMargin;
        //            return int.TryParse(r.VoteMargin, out voteMargin) ? voteMargin : 0;
        //        })
        //        .ToList();

        //    if (validResults.Count > 0)
        //    {
        //        // Get the highest VoteMargin
        //        var highestVoteMarginStr = validResults[0].VoteMargin;
        //        if (int.TryParse(highestVoteMarginStr, out var highestVoteMargin))
        //        {
        //            // Get all candidates with the highest vote margin
        //            var highestMarginCandidates = validResults
        //                .Where(r => int.TryParse(r.VoteMargin, out var margin) && margin == highestVoteMargin)
        //                .ToList();

        //            // Case 4: If any candidate has IsLottery = true, mark that candidate as the winner
        //            var lotteryCandidate = validResults.FirstOrDefault(c => c.IsDrawLottery && c.IsWinner == true);
        //            if (lotteryCandidate != null)
        //            {
        //                lotteryCandidate.IsWinner = true;
        //                await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration); // Persist after determining winner
        //                return new ServiceResponse
        //                {
        //                    IsSucceed = true,
        //                    Message = $"Candidate won by lottery."
        //                };
        //            }

        //            // Case 3: If ReCounting is true, recheck candidates and handle the draw situation again
        //            if (validResults.Any(c => c.IsReCounting))
        //            {
        //                // Handle recounting logic (essentially similar to regular draw check)
        //                if (highestMarginCandidates.Count > 1)
        //                {
        //                    foreach (var candidate in highestMarginCandidates)
        //                    {
        //                        candidate.IsDraw = true;
        //                    }
        //                    await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration); // Persist draw situation
        //                    return new ServiceResponse
        //                    {
        //                        IsSucceed = false,
        //                        Message = "Recount detected a draw situation between the highest vote margins."
        //                    };
        //                }
        //                else
        //                {
        //                    highestMarginCandidates[0].IsWinner = true;
        //                    await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration); // Persist after declaring winner
        //                    return new ServiceResponse
        //                    {
        //                        IsSucceed = true,
        //                        Message = $"Candidate is the winner after recounting."
        //                    };
        //                }
        //            }

        //            // Case 2: If multiple candidates have the same highest vote margin, mark as draw
        //            if (highestMarginCandidates.Count > 1)
        //            {
        //                foreach (var candidate in highestMarginCandidates)
        //                {
        //                    candidate.IsDraw = true;
        //                }
        //                await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration); // Persist draw situation
        //                return new ServiceResponse
        //                {
        //                    IsSucceed = false,
        //                    Message = $"Draw situation detected between candidates."
        //                };
        //            }

        //            // Case 1: If only one candidate has the highest vote margin, mark as winner
        //            if (highestMarginCandidates.Count == 1)
        //            {
        //                highestMarginCandidates[0].IsWinner = true;
        //                await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration); // Persist after declaring winner
        //                return new ServiceResponse
        //                {
        //                    IsSucceed = true,
        //                    Message = $"Candidate is the winner with the highest vote margin."
        //                };
        //            }
        //        }
        //    }

        //    // If no specific condition was met, proceed with default update persistence
        //    return await _eamsRepository.UpdateResultDeclarationForPortal(resultDeclaration);
        //}


        public async Task<ServiceResponse> CheckIfAllBoothsPollEnded(int fieldOfficerMasterId)
        {
            return await _eamsRepository.CheckIfAllBoothsPollEnded(fieldOfficerMasterId);
        }
        public async Task<Response> UpdateResultDeclarationDetails(ResultDeclaration resultDeclaration)
        {
            return await _eamsRepository.UpdateResultDeclarationDetails(resultDeclaration);
        }
        public async Task<ResultDeclaration> GetResultDeclarationById(int resultDeclarationMasterId)
        {
            return await _eamsRepository.GetResultDeclarationById(resultDeclarationMasterId);

        }
        public async Task<ServiceResponse> DeleteResultDeclarationById(int resultDeclarationMasterId)
        {
            return await _eamsRepository.DeleteResultDeclarationById(resultDeclarationMasterId);
        }
        public async Task<List<CandidateListForResultDeclaration>> GetSarpanchListById(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            return await _eamsRepository.GetSarpanchListById(stateMasterId, districtMasterId, electionTypeMasterId, assemblyMasterId, fourthLevelHMasterId);
        }

        public async Task<List<CandidateListForResultDeclaration>> GetPanchListById(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gPPanchayatWardsMasterId)
        {
            return await _eamsRepository.GetPanchListById(stateMasterId, districtMasterId, electionTypeMasterId, assemblyMasterId, fourthLevelHMasterId, gPPanchayatWardsMasterId);
        }
        public async Task<List<ResultDeclarationList>> GetResultDeclarationsByElectionType(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gpPanchayatWardsMasterId)
        {
            return await _eamsRepository.GetResultDeclarationsByElectionType(stateMasterId, districtMasterId, electionTypeMasterId, assemblyMasterId, fourthLevelHMasterId, gpPanchayatWardsMasterId);
        }

        public async Task<ResultDeclarationBoothWardList> GetResultByBoothId(int boothMasterId)
        {
            return await _eamsRepository.GetResultByBoothId(boothMasterId);
        }
        public async Task<ResultDeclarationBoothWardList> GetResultByFourthLevelHMasterId(int fourthLevelHMasterId)
        {
            return await _eamsRepository.GetResultByFourthLevelHMasterId(fourthLevelHMasterId);
        }
        public async Task<List<BoothResultList>> GetBoothResultListByFourthLevelId(int fourthlevelMasterId)
        {
            return await _eamsRepository.GetBoothResultListByFourthLevelId(fourthlevelMasterId);
        }

        public async Task<ResultDeclarationBoothWardList> GetResultByWardId(int boothMasterId)
        {
            return await _eamsRepository.GetResultByWardId(boothMasterId);
        }

        public async Task<List<BoothResultList>> GetWardResultListByFourthLevelId(int fourthlevelMasterId)
        {
            return await _eamsRepository.GetWardResultListByFourthLevelId(fourthlevelMasterId);
        }
        #endregion

        #region PancahyatMapping
        public async Task<Response> PanchayatMapping(List<FourthLevelH> fourthLevels)
        {
            return await _eamsRepository.PanchayatMapping(fourthLevels);
        }
        public async Task<Response> ReleasePanchayat(FourthLevelH fourthLevels)
        {
            return await _eamsRepository.ReleasePanchayat(fourthLevels);
        }

        public async Task<List<CombinedPanchayatMaster>> GetPanchayatListByROId(int stateMasterId, int districtMasterId, int assemblyMasterId, string roId, string assginedType)
        {
            return await _eamsRepository.GetPanchayatListByROId(stateMasterId, districtMasterId, assemblyMasterId, roId, assginedType);
        }
        public async Task<List<CombinedPanchayatMaster>> GetPanchayatListByROId(int stateMasterId, int districtMasterId, int assemblyMasterId, string roId)
        {
            return await _eamsRepository.GetPanchayatListByROId(stateMasterId, districtMasterId, assemblyMasterId, roId);
        }
        public async Task<List<CombinedPanchayatMaster>> GetFourthLevelHListExistInRDForRO(int stateMasterId, int districtMasterId, int assemblyMasterId, string roId)
        {
            return await _eamsRepository.GetFourthLevelHListExistInRDForRO(stateMasterId, districtMasterId, assemblyMasterId, roId);
        }
        public async Task<List<CombinedPanchayatMaster>> GetFourthLevelHExistInRDListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            return await _eamsRepository.GetFourthLevelHExistInRDListById(stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<List<CombinedPanchayatMaster>> GetFourthLevelListByAROId(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId, string roId, string assginedType)
        {
            return await _eamsRepository.GetFourthLevelListByAROId(stateMasterId, districtMasterId, assemblyMasterId, electionTypeMasterId, roId, assginedType);
        }
        public async Task<List<CombinedPanchayatMaster>> GetUnassignedPanchayatListById(int stateMasterId, int districtMasterId, int assemblyMasterId, string assginedType)
        {
            return await _eamsRepository.GetUnassignedPanchayatListById(stateMasterId, districtMasterId, assemblyMasterId, assginedType);

        }

        #endregion

        #region CompletedVoterList
        public async Task<List<CompletedVTList>> GetCompletedVTList(CommonReportModel commonReportModel)
        {

            return await _eamsRepository.GetCompletedVTList(commonReportModel);
        }
        #endregion


        /// <summary>
        /// This API checks for dependencies in descending order before performing the operation.
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<IsMasterEditable> IsMasterEditable(int masterId, string type, int electionTypeMasterId)
        {
            return await _eamsRepository.IsMasterEditable(masterId, type, electionTypeMasterId);
        }

        ///


        //public async Task<List<Disaster>> GetFieldAllOfficerMaster()
        //{
        //    return await _eamsRepository.GetFieldAllOfficerMaster();
        //}
        //public async Task<List<int>> GetFOAsginedBooth(int foId)
        //{
        //    return await _eamsRepository.GetFOAsginedBooth(foId);
        //}
        //public async Task<ServiceResponse> PushDisasterEvent(List<ElectionInfoMaster> electionInfoMaster)
        //{
        //    return await _eamsRepository.PushDisasterEvent(electionInfoMaster);
        //}

        #region Result Declartion DashBoard
        public async Task<List<ResultList>> GetResultByStateId(int stateMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetResultByStateId(stateMasterId, electionTypeMasterId);
        }
        public async Task<List<ResultList>> GetResultByDistrictId(int stateMasterId, int districtMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetResultByDistrictId(stateMasterId, districtMasterId, electionTypeMasterId);
        }
        public async Task<List<ResultList>> GetResultByAssemblyId(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetResultByAssemblyId(stateMasterId, districtMasterId, assemblyMasterId, electionTypeMasterId);
        }
        public async Task<List<ResultList>> GetResultByFourthLevelId(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelMasterId, int electionTypeMasterId)
        {
            return await _eamsRepository.GetResultByFourthLevelId(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelMasterId, electionTypeMasterId);
        }
        #endregion
    }
}
