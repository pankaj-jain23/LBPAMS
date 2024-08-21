using EAMS.Helper;
using EAMS.ViewModels.PSFormViewModel;
using EAMS_ACore;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.ElectionType;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.Models.QueueModel;
using EAMS_ACore.ReportModels;
using EAMS_ACore.SignalRModels;
using System.Globalization;
using System.Security.Claims;

namespace EAMS_BLL.Services
{
    public class EamsService : IEamsService
    {
        private readonly IEamsRepository _eamsRepository;
        private readonly IAuthRepository _authRepository;
        public EamsService(IEamsRepository eamsRepository, IAuthRepository authRepository)
        {
            _eamsRepository = eamsRepository;
            _authRepository = authRepository;
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

        #region  SO Master
        public async Task<List<CombinedMaster>> GetSectorOfficersListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetSectorOfficersListById(stateMasterId, districtMasterId, assemblyMasterId);
        }

        public async Task<List<CombinedMaster>> AppNotDownload(string stateMasterId)
        {
            return await _eamsRepository.AppNotDownload(stateMasterId);
        }

        public async Task<SectorOfficerProfile> GetSectorOfficerProfile(string Id, string role)
        {
            if (role == "SO")
            {
                return await _eamsRepository.GetSectorOfficerProfile(Id);
            }
            else
            {
                return await _eamsRepository.GetBLOOfficerProfile(Id);
            }
        }
        public async Task<Response> AddSectorOfficer(SectorOfficerMaster sectorOfficerMaster)
        {
            return await _eamsRepository.AddSectorOfficer(sectorOfficerMaster);
        }
        public async Task<Response> AddBLOOfficer(BLOMaster bLOMaster)
        {
            return await _eamsRepository.AddBLOOfficer(bLOMaster);
        }
        public async Task<Response> UpdateSectorOfficer(SectorOfficerMaster sectorOfficerMaster)
        {
            return await _eamsRepository.UpdateSectorOfficer(sectorOfficerMaster);
        }
        public async Task<Response> UpdateBLOOfficer(BLOMaster bLOMaster)
        {
            return await _eamsRepository.UpdateBLOOfficer(bLOMaster);
        }
        public async Task<List<CombinedMaster>> GetBoothListBySoId(string stateMasterId, string districtMasterId, string assemblyMasterId, string soId)
        {
            return await _eamsRepository.GetBoothListBySoId(stateMasterId, districtMasterId, assemblyMasterId, soId);
        }

        public async Task<SectorOfficerMasterCustom> GetSOById(string soMasterId)
        {
            var soRecord = await _eamsRepository.GetSOById(soMasterId);
            //var stateRecord = await _eamsRepository.GetStateById(soRecord.StateMasterId.ToString());
            //var districtRecord = await _eamsRepository.GetDistrictRecordById(stateRecord);
            var soCustomRecord = await _eamsRepository.GetAssemblyByCodeandState(soRecord.SoAssemblyCode.ToString(), soRecord.StateMasterId.ToString());
            var getElectionTypeRecord = await _eamsRepository.GetElectionTypeById(soRecord.ElectionTypeMasterId.ToString());
            SectorOfficerMasterCustom sectorOfficerMasterCustom = new SectorOfficerMasterCustom()
            {
                StateMasterId = soCustomRecord.StateMasterId,
                StateName = soCustomRecord.StateMaster.StateName,
                DistrictMasterId = soCustomRecord.DistrictMasterId,
                DistrictName = soCustomRecord.DistrictMaster.DistrictName,
                DistrictStatus = soCustomRecord.DistrictMaster.DistrictStatus,
                DistrictCode = soCustomRecord.DistrictMaster.DistrictCode,
                AssemblyMasterId = soCustomRecord.AssemblyMasterId,
                AssemblyName = soCustomRecord.AssemblyName,
                AssemblyCode = soCustomRecord.AssemblyCode,
                SoMasterId = soRecord.SOMasterId,
                SoName = soRecord.SoName,
                SoMobile = soRecord.SoMobile,
                SoOfficeName = soRecord.SoOfficeName,
                SoDesignation = soRecord.SoDesignation,
                IsStatus = soRecord.SoStatus,
                ElectionTypeMasterId = soRecord.ElectionTypeMasterId,
                ElectionTypeName = getElectionTypeRecord.ElectionType,


            };
            return sectorOfficerMasterCustom;
        }
        #endregion

        #region Booth Master

        public async Task<List<CombinedMaster>> GetBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetBoothListById(stateMasterId, districtMasterId, assemblyMasterId);
        }

        public async Task<List<CombinedMaster>> GetBoothListByIdwithPsZone(string stateMasterId, string districtMasterId, string assemblyMasterId, string fourthLevelHMasterId,string BlockZonePanchayatMasterId)
        {
            return await _eamsRepository.GetBoothListByIdwithPsZone(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId, BlockZonePanchayatMasterId);
        }

        public async Task<List<CombinedMaster>> GetBoothListByIdforPSO(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            return await _eamsRepository.GetBoothListByIdforPSO(stateMasterId, districtMasterId, assemblyMasterId);
        }
        public async Task<List<CombinedMaster>> GetUnassignedBoothListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
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

        #endregion

        #region Event Master
        public async Task<List<EventMaster>> GetEventList()
        {
            return await _eamsRepository.GetEventList();
        }

        public async Task<ServiceResponse> UpdateEventStaus(EventMaster eventMaster)
        {
            var isSucced = await _eamsRepository.UpdateEventStaus(eventMaster);
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
        public async Task<Response> UpdateEventById(EventMaster eventMaster)
        {
            return await _eamsRepository.UpdateEventById(eventMaster);
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



        public Task<List<AssemblyMaster>> GetAssemblyByPCId(string stateMasterid, string PcMasterId)
        {
            return _eamsRepository.GetAssemblyByPCId(stateMasterid, PcMasterId);
        }

        public Task<List<AssemblyMaster>> GetAssemblyByDistrictId(string stateMasterid, string districtMasterId)
        {
            return _eamsRepository.GetAssemblyByDistrictId(stateMasterid, districtMasterId);
        }

        #endregion

        #region EventActivity
        public async Task<Response> EventActivity(ElectionInfoMaster electionInfoMaster)
        {
            var electionInfoRecord = await _eamsRepository.EventUpdationStatus(electionInfoMaster);

            if (electionInfoRecord != null)
            {
                string pollInterruptedMsg = "You have not entered 'Resume Time' in Poll interruption against this booth.";
                bool isPollInterruptedOfBooth = await _eamsRepository.IsPollInterrupted(electionInfoMaster.BoothMasterId);
                var boothRecord = await _eamsRepository.GetBoothById(electionInfoMaster.BoothMasterId.ToString());
                var assemblyMasterRecord = await _eamsRepository.GetAssemblyById(boothRecord.AssemblyMasterId.ToString());

                switch (electionInfoMaster.EventMasterId)
                {
                    case 1: // Party Dispatch
                        return await HandlePartyDispatchEvent(electionInfoMaster, electionInfoRecord, isPollInterruptedOfBooth, pollInterruptedMsg, assemblyMasterRecord);

                    case 2: // Party Reach
                        return await HandlePartyReachEvent(electionInfoMaster, electionInfoRecord, isPollInterruptedOfBooth, pollInterruptedMsg, assemblyMasterRecord);

                    case 3: // setup of polling
                        return await HandleSetupPollingEvent(electionInfoMaster, electionInfoRecord, isPollInterruptedOfBooth, pollInterruptedMsg, assemblyMasterRecord);

                    case 4: // mockpoll
                        return await HandleMockPollEvent(electionInfoMaster, electionInfoRecord, isPollInterruptedOfBooth, pollInterruptedMsg, assemblyMasterRecord);

                    case 5: // mockpoll
                        return await HandlePollStartedEvent(electionInfoMaster, electionInfoRecord, isPollInterruptedOfBooth, pollInterruptedMsg, assemblyMasterRecord);

                    case 7: // queue
                        return await HandleQueueEvent(electionInfoMaster, electionInfoRecord, isPollInterruptedOfBooth);
                    case 8: // final
                        return await HandleFinalVoteEvent(electionInfoMaster, electionInfoRecord, isPollInterruptedOfBooth);
                    case 9: // poll ended
                        return await HandlePollEndedEvent(electionInfoMaster, electionInfoRecord);
                    case 10: // mcewitchoff
                        return await HandleMCESwitchOffEvent(electionInfoMaster, electionInfoRecord);
                    case 11: // partydeparted
                        return await HandlePartyDepartedEvent(electionInfoMaster, electionInfoRecord);
                    case 12: // partreachcollection
                        return await HandlePartyReachCollectionEvent(electionInfoMaster, electionInfoRecord);
                    case 13: // evmdepoist
                        return await HandleEVMDepositedEvent(electionInfoMaster, electionInfoRecord);

                    // Add cases for other events...
                    default:
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Invalid EventMasterId." };
                }
            }
            else
            {
                if (electionInfoMaster.EventMasterId == 1)
                {
                    string pollInterruptedMsg = "You have not entered 'Resume Time' in Poll interruption against this booth.";
                    bool isPollInterruptedOfBooth = await _eamsRepository.IsPollInterrupted(electionInfoMaster.BoothMasterId);
                    var boothRecord = await _eamsRepository.GetBoothById(electionInfoMaster.BoothMasterId.ToString());
                    var assemblyMasterRecord = await _eamsRepository.GetAssemblyById(boothRecord.AssemblyMasterId.ToString());
                    if (isPollInterruptedOfBooth == false)
                    {
                        electionInfoMaster.PCMasterId = assemblyMasterRecord.PCMasterId;
                        return await _eamsRepository.EventActivity(electionInfoMaster);
                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };
                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check Party Dispatch Event" };
                }
            }
        }

        // Extracted Methods

        private async Task<Response> HandlePartyDispatchEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord, bool isPollInterruptedOfBooth, string pollInterruptedMsg, AssemblyMaster assemblyMasterRecord)
        {
            if (isPollInterruptedOfBooth)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };
            }

            if (electionInfoRecord.IsPartyReached == true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Has Already Arrived." };
            }

            if (electionInfoRecord.IsPartyDispatched != null)
            {
                // Update electionInfoRecord and return response
                electionInfoRecord.IsPartyDispatched = electionInfoMaster.IsPartyDispatched;
                electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                electionInfoRecord.PartyDispatchedLastUpdate = BharatDateTime();
                electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;

                return await _eamsRepository.EventActivity(electionInfoRecord);

            }
            else
            {// Party Dispatch status is already set
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Already Dispatched." };

            }



            //            if (isPollInterruptedOfBooth == false)
            //            {
            //                if (electionInfoRecord.IsPartyReached == false || electionInfoRecord.IsPartyReached == null)
            //                {
            //                    if (electionInfoRecord.IsPartyDispatched is not null)
            //                    {
            //                        electionInfoRecord.IsPartyDispatched = electionInfoMaster.IsPartyDispatched;
            //                        electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
            //                        electionInfoRecord.PartyDispatchedLastUpdate = BharatDateTime();
            //        electionInfoRecord.PartyDispatchedLastUpdate = BharatDateTime();
            //        electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
            //                        return await _eamsRepository.EventActivity(electionInfoRecord);
            //    }
            //                    else
            //                    {
            //                        //Already Yes
            //                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Already Dispatched." };

            //                    }
            //                }
            //                else
            //{
            //    // party alteady arrived, cnt change status!
            //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Has Already Arrived." };
            //}
            //            }
            //            else
            //{
            //    return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };

            //}
        }

        private async Task<Response> HandlePartyReachEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord, bool isPollInterruptedOfBooth, string pollInterruptedMsg, AssemblyMaster assemblyMasterRecord)
        {



            if (electionInfoRecord.IsSetupOfPolling == true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, SetUpPolling Already yes." };
            }
            if (electionInfoRecord.IsPartyDispatched != true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Not Dispatched Yet." };
            }

            if (electionInfoRecord.IsSetupOfPolling != true)
            {
                // Update electionInfoRecord and return response
                electionInfoRecord.IsPartyReached = electionInfoMaster.IsPartyReached;
                electionInfoRecord.PartyReachedLastUpdate = BharatDateTime();
                electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;

                return await _eamsRepository.EventActivity(electionInfoRecord);

            }
            else
            {// Party aarived status is already set
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, SetUpPolling Already yes" };

            }

            //if (isPollInterruptedOfBooth == false)
            //{
            //    if (electionInfoRecord.IsPartyDispatched == true)
            //    {

            //        if (electionInfoRecord.IsSetupOfPolling == false || electionInfoRecord.IsSetupOfPolling == null)
            //        {
            //            electionInfoRecord.IsPartyReached = electionInfoMaster.IsPartyReached;
            //            electionInfoRecord.PartyReachedLastUpdate = BharatDateTime();
            //            electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
            //            electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
            //            return await _eamsRepository.EventActivity(electionInfoRecord);
            //        }
            //        else
            //        {

            //            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, SetUpPolling Already yes." };

            //        }


            //    }
            //    else
            //    {

            //        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Not Dispatched Yet." };
            //    }
            //}
            //else
            //{
            //    return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };
            //}
        }

        private async Task<Response> HandleSetupPollingEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord, bool isPollInterruptedOfBooth, string pollInterruptedMsg, AssemblyMaster assemblyMasterRecord)
        {
            if (isPollInterruptedOfBooth)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };
            }

            if (electionInfoRecord.IsPartyReached != true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Not Arrived Yet." };
            }

            if (electionInfoRecord.IsMockPollDone == true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, MockPoll Already yes." };
            }

            // Update electionInfoRecord and return response
            electionInfoRecord.IsSetupOfPolling = electionInfoMaster.IsSetupOfPolling;
            electionInfoRecord.SetupOfPollingLastUpdate = BharatDateTime();
            electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
            electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;

            return await _eamsRepository.EventActivity(electionInfoRecord);
        }
        private async Task<Response> HandleMockPollEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord, bool isPollInterruptedOfBooth, string pollInterruptedMsg, AssemblyMaster assemblyMasterRecord)
        {
            if (isPollInterruptedOfBooth)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };
            }

            if (electionInfoRecord.IsSetupOfPolling != true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Set Up Of Polling Not Done Yet." };
            }

            if (electionInfoRecord.IsPollStarted == true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Poll Started Already yes." };
            }

            // Update electionInfoRecord and return response
            electionInfoRecord.IsMockPollDone = electionInfoMaster.IsMockPollDone;
            electionInfoRecord.NoOfPollingAgents = electionInfoMaster.NoOfPollingAgents;
            electionInfoRecord.MockPollDoneLastUpdate = BharatDateTime();
            electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
            //            electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;

            return await _eamsRepository.EventActivity(electionInfoRecord);
        }
        private async Task<Response> HandlePollStartedEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord, bool isPollInterruptedOfBooth, string pollInterruptedMsg, AssemblyMaster assemblyMasterRecord)
        {
            if (isPollInterruptedOfBooth)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };
            }

            if (electionInfoRecord.IsMockPollDone != true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Mock Poll Done is not Done Yet." };
            }

            // Check if voter turn out status is entered
            var pollCanStart = await _eamsRepository.CanPollStart(electionInfoRecord.BoothMasterId, 6);
            if (!pollCanStart)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Voter Turn Out Status is Entered." };
            }

            // Update electionInfoRecord and return response
            electionInfoRecord.PollStartedLastUpdate = BharatDateTime();
            electionInfoRecord.IsPollStarted = electionInfoMaster.IsPollStarted;
            electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
            electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;

            return await _eamsRepository.EventActivity(electionInfoRecord);
        }

        public async Task<Response> HandleQueueEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord, bool isPollInterruptedOfBooth)
        {
            var queueCanStart = await _eamsRepository.CanQueueStart(electionInfoRecord.BoothMasterId);
            if (!queueCanStart)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Turn Out Not Entered any Values." };
            }

            var queueTime = await _eamsRepository.QueueTime(electionInfoRecord.BoothMasterId);
            if (!queueTime)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue will be Opened at specified Time." };
            }

            if (electionInfoRecord.FinalTVoteStatus != null && electionInfoRecord.FinalTVoteStatus == true)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue has been Freezed as Final Vote has been entered." };
            }

            if (electionInfoMaster.VoterInQueue == null)
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue value cannot be null" };
            }

            if (electionInfoMaster.VoterInQueue != null)
            {
                if (electionInfoRecord.VoterInQueue == null && electionInfoMaster.ElectionInfoStatus == true)
                {
                    EAMS_ACore.Models.Queue fetchResult = await _eamsRepository.GetTotalRemainingVoters(electionInfoMaster.BoothMasterId.ToString());
                    if (electionInfoMaster.VoterInQueue <= fetchResult.TotalVoters)
                    {
                        if (electionInfoMaster.VoterInQueue <= fetchResult.RemainingVotes)
                        {
                            electionInfoRecord.VoterInQueue = electionInfoMaster.VoterInQueue;
                            electionInfoRecord.VoterInQueueLastUpdate = BharatDateTime();
                            electionInfoRecord.IsVoterTurnOut = true;
                            electionInfoRecord.VotingTurnOutLastUpdate = BharatDateTime();
                            electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                            //electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
                            return await _eamsRepository.EventActivity(electionInfoRecord);
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voters in queue cannot exceed voter remaining!" };
                        }
                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Polling should not be more than Total Voters!" };
                    }
                }
                else
                {
                    if (electionInfoMaster.ElectionInfoStatus == false && electionInfoMaster.EventMasterId == 7 && electionInfoRecord.VoterInQueue != null && electionInfoMaster.VoterInQueue == 0)
                    {
                        //undo action
                        electionInfoRecord.VoterInQueue = null;
                        electionInfoRecord.IsQueueUndo = electionInfoMaster.IsQueueUndo;
                        electionInfoRecord.VoterInQueueLastUpdate = BharatDateTime();
                        electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                        //electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
                        return await _eamsRepository.EventActivity(electionInfoRecord);
                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue value Already Entered. Please proceed for the Final Voting value" };
                    }
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue value cannot be null" };
            }
        }
        public async Task<Response> HandleFinalVoteEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord, bool isPollInterruptedOfBooth)
        {
            //  Final Votes
            if (electionInfoRecord.VoterInQueue != null)
            {
                if (electionInfoRecord.IsPollEnded == false || electionInfoRecord.IsPollEnded == null)
                {
                    if (electionInfoMaster.FinalTVoteStatus == true)
                    {
                        if (electionInfoRecord.FinalTVoteStatus == true)
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Already Status Yes." };
                        }
                        else
                        {
                            if (electionInfoMaster.FinalTVote != null && electionInfoMaster.FinalTVote > 0)
                            {
                                // one more check that last votes polled value and final vote now should not be greater than total voters
                                EAMS_ACore.Models.Queue fetchResult = await _eamsRepository.GetTotalRemainingVoters(electionInfoMaster.BoothMasterId.ToString());
                                if (electionInfoMaster.FinalTVote <= fetchResult.TotalVoters)
                                {
                                    if (electionInfoMaster.FinalTVote > 0)
                                    {
                                        //if (electionInfoMaster.FinalTVote >= fetchResult.VotesPolled)
                                        //{
                                        electionInfoRecord.FinalTVote = electionInfoMaster.FinalTVote;
                                        electionInfoRecord.VotingLastUpdate = BharatDateTime();
                                        electionInfoRecord.FinalTVoteStatus = true;
                                        electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                        electionInfoRecord.Male = electionInfoMaster.Male;
                                        electionInfoRecord.Female = electionInfoMaster.Female;
                                        electionInfoRecord.Transgender = electionInfoMaster.Transgender;
                                        electionInfoRecord.EDC = electionInfoMaster.EDC;
                                        return await _eamsRepository.EventActivity(electionInfoRecord);
                                        //}
                                        //else
                                        //{
                                        //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Cannot be less than Last Votes Polled" };
                                        //}
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Cannot be Zero or Negative Value" };

                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Cannot be Greater than Total Voters" };
                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Cannot be Null or 0." };
                            }
                        }
                    }
                    else if (electionInfoMaster.FinalTVoteStatus == false)
                    {
                        // undo case
                        if (electionInfoRecord.FinalTVoteStatus == false)
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Vote Already Status No." };
                        }
                        else
                        {
                            //check if record null
                            if (electionInfoRecord.FinalTVoteStatus == null)
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Vote Need to be filled first." };
                            }
                            else
                            {
                                electionInfoRecord.VotingLastUpdate = BharatDateTime();
                                electionInfoRecord.FinalTVoteStatus = false;
                                electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                return await _eamsRepository.EventActivity(electionInfoRecord);
                            }
                        }
                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Vote Can't be Empty." };
                    }
                }
                else
                {
                    // already status Yes
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Poll Ended Already." };
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter in queue is not updated Yet." };
            }
        }
        public async Task<Response> HandlePollEndedEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord)
        {
            if (electionInfoRecord.FinalTVote > 0 && electionInfoRecord.FinalTVoteStatus == true) // Poll ended--
            {
                if (electionInfoRecord.IsMCESwitchOff == false || electionInfoRecord.IsMCESwitchOff == null)
                {
                    //if (electionInfoRecord.IsPollEnded == false || electionInfoRecord.IsPollEnded == null)
                    //{
                    //
                    electionInfoRecord.IsPollEnded = electionInfoMaster.IsPollEnded;
                    electionInfoRecord.IsPollEndedLastUpdate = BharatDateTime();
                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                    return await _eamsRepository.EventActivity(electionInfoRecord);
                    //}
                    //else
                    //{
                    //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Poll Already Ended." };
                    //}
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Machine Closed & EVM Switched Off Already Yes." };
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes not Done yet." };
            }
        }
        public async Task<Response> HandleMCESwitchOffEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord)
        {
            if (electionInfoRecord.IsPollEnded == true)
            {
                //if (electionInfoRecord.IsMCESwitchOff == false || electionInfoRecord.IsMCESwitchOff == null)
                //{
                if (electionInfoRecord.IsPartyDeparted == false || electionInfoRecord.IsPartyDeparted == null)
                {

                    electionInfoRecord.IsMCESwitchOff = electionInfoMaster.IsMCESwitchOff;
                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                    electionInfoRecord.MCESwitchOffLastUpdate = BharatDateTime();
                    return await _eamsRepository.EventActivity(electionInfoRecord);
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Party Already Departed." };
                }
                //}
                //else
                //{
                //    // already status Yes
                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Machine Closed and EVM Switched Off Already yes." };
                //}
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Poll Is Not Ended yet." };
            }
        }

        public async Task<Response> HandlePartyDepartedEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord)
        {
            if (electionInfoRecord.IsMCESwitchOff == true) // machine switch off and EVM cleared
            {
                //if (electionInfoRecord.IsPartyDeparted == false || electionInfoRecord.IsPartyDeparted == null)
                //{
                if (electionInfoRecord.IsPartyReachedCollectionCenter == false || electionInfoRecord.IsPartyReachedCollectionCenter == null)
                {
                    electionInfoRecord.IsPartyDeparted = electionInfoMaster.IsPartyDeparted;
                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                    electionInfoRecord.PartyDepartedLastUpdate = BharatDateTime();
                    return await _eamsRepository.EventActivity(electionInfoRecord);
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Party Already Reached at Collection Centre." };
                }
                //}
                //else
                //{
                //    // already status Yes
                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Departed Already Yes." };
                //}
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Machine Closed & EVM Not Switched Off yet." };
            }
        }
        public async Task<Response> HandlePartyReachCollectionEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord)
        {
            if (electionInfoRecord.IsPartyDeparted == true)
            {
                //if (electionInfoRecord.IsPartyReachedCollectionCenter == false || electionInfoRecord.IsPartyReachedCollectionCenter == null)
                //{
                if (electionInfoRecord.IsEVMDeposited == false || electionInfoRecord.IsEVMDeposited == null)
                {
                    electionInfoRecord.IsPartyReachedCollectionCenter = electionInfoMaster.IsPartyReachedCollectionCenter;
                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                    electionInfoRecord.PartyReachedLastUpdate = BharatDateTime();
                    return await _eamsRepository.EventActivity(electionInfoRecord);
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, EVM Deposited." };
                }
                //}
                //else
                //{
                //    // already status Yes
                //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Arrived Already Yes." };
                //}
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Is Not Departed yet." };
            }
        }

        public async Task<Response> HandleEVMDepositedEvent(ElectionInfoMaster electionInfoMaster, ElectionInfoMaster electionInfoRecord)
        {
            if (electionInfoRecord.IsPartyReachedCollectionCenter == true)
            {
                if (electionInfoRecord.IsEVMDeposited == false || electionInfoRecord.IsEVMDeposited == null)
                {
                    electionInfoRecord.IsEVMDeposited = electionInfoMaster.IsEVMDeposited;
                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                    electionInfoRecord.PartyReachedCollectionCenterLastUpdate = BharatDateTime();
                    return await _eamsRepository.EventActivity(electionInfoRecord);
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, EVM Already Deposited." };
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Is Not Reached Collection Center yet." };
            }
        }


        public async Task<Response> EventActivity2(ElectionInfoMaster electionInfoMaster)
        {
            var electionInfoRecord = await _eamsRepository.EventUpdationStatus(electionInfoMaster);
            if (electionInfoRecord != null)
            {
                string pollInterruptedMsg = "You have not entered 'Resume Time' in Poll interruption againist this booth.";
                bool isPollInterruptedOfBooth = false;
                isPollInterruptedOfBooth = await _eamsRepository.IsPollInterrupted(electionInfoMaster.BoothMasterId);
                var boothRecord = await _eamsRepository.GetBoothById(electionInfoMaster.BoothMasterId.ToString());
                var assemblyMasterRecord = await _eamsRepository.GetAssemblyById(boothRecord.AssemblyMasterId.ToString());
                switch (electionInfoMaster.EventMasterId)
                {
                    case 1: //party Dispatch
                            //check evm deposited then no interruption

                        if (isPollInterruptedOfBooth == false)
                        {
                            if (electionInfoRecord.IsPartyReached == false || electionInfoRecord.IsPartyReached == null)
                            {
                                if (electionInfoRecord.IsPartyDispatched is not null)
                                {
                                    electionInfoRecord.IsPartyDispatched = electionInfoMaster.IsPartyDispatched;
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    electionInfoRecord.PartyDispatchedLastUpdate = BharatDateTime();
                                    electionInfoRecord.PartyDispatchedLastUpdate = BharatDateTime();
                                    electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {
                                    //Already Yes
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Already Dispatched." };

                                }
                            }
                            else
                            {
                                // party alteady arrived, cnt change status!
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Has Already Arrived." };
                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };

                        }
                    case 2:

                        if (isPollInterruptedOfBooth == false)
                        {
                            if (electionInfoRecord.IsPartyDispatched == true)
                            {

                                if (electionInfoRecord.IsSetupOfPolling == false || electionInfoRecord.IsSetupOfPolling == null)
                                {
                                    electionInfoRecord.IsPartyReached = electionInfoMaster.IsPartyReached;
                                    electionInfoRecord.PartyReachedLastUpdate = BharatDateTime();
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {

                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, SetUpPolling Already yes." };

                                }


                            }
                            else
                            {

                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Not Dispatched Yet." };
                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };
                        }

                    case 3:
                        if (isPollInterruptedOfBooth == false)
                        {
                            if (electionInfoRecord.IsPartyReached == true)
                            {

                                if (electionInfoRecord.IsMockPollDone == false || electionInfoRecord.IsMockPollDone == null)
                                {
                                    electionInfoRecord.IsSetupOfPolling = electionInfoMaster.IsSetupOfPolling;
                                    electionInfoRecord.SetupOfPollingLastUpdate = BharatDateTime();
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {

                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, MockPoll Already yes." };
                                }



                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Not Arrived Yet." };

                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };

                        }
                    case 4:

                        if (isPollInterruptedOfBooth == false)
                        {
                            if (electionInfoRecord.IsSetupOfPolling == true) // mockpoll event 4th event
                            {

                                if (electionInfoRecord.IsPollStarted == false || electionInfoRecord.IsPollStarted == null)
                                {
                                    electionInfoRecord.IsMockPollDone = electionInfoMaster.IsMockPollDone;
                                    electionInfoRecord.NoOfPollingAgents = electionInfoMaster.NoOfPollingAgents;
                                    electionInfoRecord.MockPollDoneLastUpdate = BharatDateTime();
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {

                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Poll Started Already yes." };
                                }



                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Set Up Of Polling Not Done Yet." };

                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };

                        }

                    case 5:// poll started 5th event
                        if (isPollInterruptedOfBooth == false)
                        {
                            if (electionInfoRecord.IsMockPollDone == true)  // prev event
                            {
                                //check polled detail for voter turn out, if enetered then cant change poll started status

                                var pollCanStart = await _eamsRepository.CanPollStart(electionInfoRecord.BoothMasterId, 6);
                                if (pollCanStart == true)
                                {
                                    electionInfoRecord.PollStartedLastUpdate = BharatDateTime();
                                    electionInfoRecord.IsPollStarted = electionInfoMaster.IsPollStarted;
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Voter Turn Out Status is Entered." };

                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Mock Poll Done is not Done Yet." };

                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };

                        }
                    case 7:
                        //queue
                        //check only voter turn out entered or not, but not check last entered value

                        var QueueCanStart = await _eamsRepository.CanQueueStart(electionInfoRecord.BoothMasterId);
                        if (QueueCanStart == true)
                        {
                            bool queueTime = await _eamsRepository.QueueTime(electionInfoRecord.BoothMasterId);
                            if (queueTime == true)
                            {

                                //if (electionInfoRecord.FinalTVote == null)  // next event
                                if (electionInfoRecord.FinalTVoteStatus == null || electionInfoRecord.FinalTVoteStatus == false)  // next event
                                {

                                    if (electionInfoMaster.VoterInQueue != null) //model my hve o or >0
                                    {

                                        if (electionInfoRecord.VoterInQueue == null && electionInfoMaster.ElectionInfoStatus == true) // electionInfo is null for queue
                                        {

                                            EAMS_ACore.Models.Queue fetchResult = await _eamsRepository.GetTotalRemainingVoters(electionInfoMaster.BoothMasterId.ToString());
                                            if (electionInfoMaster.VoterInQueue <= fetchResult.TotalVoters)
                                            {

                                                if (electionInfoMaster.VoterInQueue <= fetchResult.RemainingVotes)
                                                {
                                                    //if (electionInfoMaster.VoterInQueue >= fetchResult.VotesPolled)
                                                    //{

                                                    electionInfoRecord.VoterInQueue = electionInfoMaster.VoterInQueue;
                                                    //electionInfoRecord.IsQueueUndo = electionInfoMaster.IsQueueUndo;
                                                    electionInfoRecord.VoterInQueueLastUpdate = BharatDateTime();
                                                    electionInfoRecord.IsVoterTurnOut = true;
                                                    electionInfoRecord.VotingTurnOutLastUpdate = BharatDateTime();
                                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                                    electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;
                                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                                    //}
                                                    //else
                                                    //{
                                                    //    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voters in queue canno t be less than Last Votes Polled!" };
                                                    //}
                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voters in queue cannot exceed voter remaining!" };

                                                }
                                            }

                                            else
                                            {

                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Polling should not be more than Total Voters!" };
                                            }


                                        }
                                        else
                                        {

                                            // info must have value already , need to check for undo
                                            if (electionInfoMaster.ElectionInfoStatus == false && electionInfoMaster.EventMasterId == 7 && electionInfoRecord.VoterInQueue != null && electionInfoMaster.VoterInQueue == 0)
                                            {
                                                //undo action
                                                electionInfoRecord.VoterInQueue = null;
                                                electionInfoRecord.IsQueueUndo = electionInfoMaster.IsQueueUndo;
                                                electionInfoRecord.VoterInQueueLastUpdate = BharatDateTime();
                                                electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                                electionInfoRecord.PCMasterId = assemblyMasterRecord.PCMasterId;

                                                return await _eamsRepository.EventActivity(electionInfoRecord);

                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue value Already Entered. Pls proceed for the Final Voting value" };
                                            }
                                        }

                                    }
                                    else
                                    {

                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue value cannot be null" };
                                    }
                                }
                                else
                                {

                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue has been Freezed as Final Vote has been entered." };
                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Queue will be Opened at specified Time." };
                            }
                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Turn Out Not Entered any Values." };

                        }

                    case 8:
                        // Final Votes
                        if (electionInfoRecord.VoterInQueue != null)
                        {
                            if (electionInfoRecord.IsPollEnded == false || electionInfoRecord.IsPollEnded == null)
                            {


                                if (electionInfoMaster.FinalTVoteStatus == true)
                                {

                                    if (electionInfoRecord.FinalTVoteStatus == true)
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Already Status Yes." };
                                    }
                                    else
                                    {
                                        if (electionInfoMaster.FinalTVote != null && electionInfoMaster.FinalTVote > 0)
                                        {
                                            // one more check that last votes polled value and final vote now should not be greater than total voters

                                            EAMS_ACore.Models.Queue fetchResult = await _eamsRepository.GetTotalRemainingVoters(electionInfoMaster.BoothMasterId.ToString());
                                            if (electionInfoMaster.FinalTVote <= fetchResult.TotalVoters) //
                                            {
                                                if (electionInfoMaster.FinalTVote >= fetchResult.VotesPolled)

                                                {
                                                    //electionInfoMaster.IsFinalVote= electionInfoMaster.IsFinalVote;
                                                    electionInfoRecord.FinalTVote = electionInfoMaster.FinalTVote;
                                                    electionInfoRecord.VotingLastUpdate = BharatDateTime();
                                                    electionInfoRecord.FinalTVoteStatus = true;
                                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                                    electionInfoRecord.Male = electionInfoMaster.Male;
                                                    electionInfoRecord.Female = electionInfoMaster.Female;
                                                    electionInfoRecord.Transgender = electionInfoMaster.Transgender;
                                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                                }
                                                else
                                                {

                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Cannot be less than Last Votes Polled" };

                                                }

                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Cannot be Greater than Total Voters" };
                                            }
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes Cannot be Null or 0." };
                                        }
                                    }
                                }
                                else if (electionInfoMaster.FinalTVoteStatus == false)
                                {
                                    // undo case
                                    if (electionInfoRecord.FinalTVoteStatus == false)
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Vote Already Status No." };
                                        //electionInfoRecord.FinalTVote = electionInfoMaster.FinalTVote;//

                                    }
                                    else
                                    {
                                        //check if record null
                                        if (electionInfoRecord.FinalTVoteStatus == null)
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Vote Need to be filled first." };
                                        }
                                        else
                                        {
                                            electionInfoRecord.VotingLastUpdate = BharatDateTime();
                                            electionInfoRecord.FinalTVoteStatus = false;
                                            electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                            return await _eamsRepository.EventActivity(electionInfoRecord);
                                        }
                                    }

                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Vote Can't be Empty." };
                                }


                            }
                            else
                            {
                                // already status Yes
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Poll Ended Already." };

                            }

                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter in queue is not updated Yet." };

                        }

                    case 9:
                        if (electionInfoRecord.FinalTVote > 0 && electionInfoRecord.FinalTVoteStatus == true) // Poll ended--
                        {
                            if (electionInfoRecord.IsMCESwitchOff == false || electionInfoRecord.IsMCESwitchOff == null)
                            {
                                if (electionInfoRecord.IsPollEnded == false || electionInfoRecord.IsPollEnded == null)
                                {
                                    //
                                    electionInfoRecord.IsPollEnded = electionInfoMaster.IsPollEnded;
                                    electionInfoRecord.IsPollEndedLastUpdate = BharatDateTime();
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Poll Already Ended." };

                                }
                            }
                            else
                            {

                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Machine Closed & EVM Switched Off Already Yes." };
                            }



                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Final Votes not Done yet." };

                        }

                    case 10:
                        if (electionInfoRecord.IsPollEnded == true) // Machine Switch Off and EVM Cleared
                        {
                            if (electionInfoRecord.IsMCESwitchOff == false || electionInfoRecord.IsMCESwitchOff == null)
                            {
                                if (electionInfoRecord.IsPartyDeparted == false || electionInfoRecord.IsMCESwitchOff == null)
                                {
                                    electionInfoRecord.IsMCESwitchOff = electionInfoMaster.IsMCESwitchOff;
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    electionInfoRecord.MCESwitchOffLastUpdate = BharatDateTime();
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {

                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Party Already Departed." };
                                }

                            }
                            else
                            {
                                // already status Yes
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Machine Closed and EVM Switched Off Already yes." };

                            }

                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Poll Is Not Ended yet." };

                        }

                    case 11:
                        if (electionInfoRecord.IsMCESwitchOff == true) // party departed
                        {
                            if (electionInfoRecord.IsPartyDeparted == false || electionInfoRecord.IsPartyDeparted == null)
                            {
                                if (electionInfoRecord.IsPartyReachedCollectionCenter == false || electionInfoRecord.IsPartyReachedCollectionCenter == null)
                                {
                                    electionInfoRecord.IsPartyDeparted = electionInfoMaster.IsPartyDeparted;
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    electionInfoRecord.PartyDepartedLastUpdate = BharatDateTime();
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {

                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, Party Already Reached at Collection Centre." };
                                }

                            }
                            else
                            {
                                // already status Yes
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Departed Already Yes." };

                            }

                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Machine Closed & EVM Not Switched Off yet." };

                        }

                    case 12:
                        if (electionInfoRecord.IsPartyDeparted == true)
                        {
                            if (electionInfoRecord.IsPartyReachedCollectionCenter == false || electionInfoRecord.IsPartyReachedCollectionCenter == null)
                            {
                                if (electionInfoRecord.IsEVMDeposited == false || electionInfoRecord.IsEVMDeposited == null)
                                {
                                    electionInfoRecord.IsPartyReachedCollectionCenter = electionInfoMaster.IsPartyReachedCollectionCenter;
                                    electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                    electionInfoRecord.PartyReachedLastUpdate = BharatDateTime();
                                    return await _eamsRepository.EventActivity(electionInfoRecord);
                                }
                                else
                                {

                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, EVM Deposited." };
                                }

                            }
                            else
                            {
                                // already status Yes
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Arrived Already Yes." };

                            }

                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Is Not Departed yet." };

                        }

                    case 13:
                        if (electionInfoRecord.IsPartyReachedCollectionCenter == true) // Machine Switch Off and EVM Cleared
                        {

                            if (electionInfoRecord.IsEVMDeposited == false || electionInfoRecord.IsEVMDeposited == null)
                            {
                                electionInfoRecord.IsEVMDeposited = electionInfoMaster.IsEVMDeposited;
                                electionInfoRecord.EventMasterId = electionInfoMaster.EventMasterId;
                                electionInfoRecord.PartyReachedCollectionCenterLastUpdate = BharatDateTime();
                                return await _eamsRepository.EventActivity(electionInfoRecord);
                            }
                            else
                            {

                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Can't Change Status, EVM Already Deposited." };
                            }


                        }
                        else
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Party Is Not Departed yet." };

                        }

                }
            }
            else if (electionInfoRecord == null)
            {
                if (electionInfoMaster.EventMasterId == 1)
                {
                    string pollInterruptedMsg = "You have not entered 'Resume Time' in Poll interruption againist this booth.";
                    bool isPollInterruptedOfBooth = false;
                    isPollInterruptedOfBooth = await _eamsRepository.IsPollInterrupted(electionInfoMaster.BoothMasterId);
                    var boothRecord = await _eamsRepository.GetBoothById(electionInfoMaster.BoothMasterId.ToString());
                    var assemblyMasterRecord = await _eamsRepository.GetAssemblyById(boothRecord.AssemblyMasterId.ToString());
                    if (isPollInterruptedOfBooth == false)
                    {
                        electionInfoMaster.PCMasterId = assemblyMasterRecord.PCMasterId;
                        return await _eamsRepository.EventActivity(electionInfoMaster);
                    }
                    else
                    {
                        return new Response { Status = RequestStatusEnum.BadRequest, Message = pollInterruptedMsg };

                    }
                }
                else
                {
                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check Party Dispatch Event" };
                }


            }

            return new Response { Status = RequestStatusEnum.BadRequest, Message = "something went wrong" };

        }

        public async Task<List<EventWiseBoothStatus>> EventWiseBoothStatus(string soId)
        {
            return await _eamsRepository.EventWiseBoothStatus(soId);
        }

        public async Task<VoterTurnOutPolledDetailViewModel> GetLastUpdatedPollDetail(string boothMasterId, int eventid)

        {
            return await _eamsRepository.GetLastUpdatedPollDetail(boothMasterId, eventid);

        }
        public async Task<EAMS_ACore.Models.Queue> GetVoterInQueue(string boothMasterId)

        {
            return await _eamsRepository.GetVoterInQueue(boothMasterId);

        }


        public async Task<FinalViewModel> GetFinalVotes(string boothMasterId)

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



        public async Task<List<EventActivityCount>> GetEventListDistrictWiseById(string stateId)
        {
            return await _eamsRepository.GetEventListDistrictWiseById(stateId);
        }
        public async Task<List<EventActivityCount>> GetEventListPCWiseById(string stateId, string userId)
        {
            return await _eamsRepository.GetEventListPCWiseById(stateId, userId);
        }
        public async Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseById(string stateId, string districtId)
        {
            return await _eamsRepository.GetEventListAssemblyWiseById(stateId, districtId);
        }
        //public async Task<List<AssemblyEventActivityCount>> GetEventListAssemblyWiseByStateId(string stateId)
        //{
        //    return await _eamsRepository.GetEventListAssemblyWiseByStateId(stateId);
        //}
        public async Task<List<AssemblyEventActivityCountPCWise>> GetEventListAssemblyWiseByPCId(string stateId, string pcId)
        {
            return await _eamsRepository.GetEventListAssemblyWiseByPCId(stateId, pcId);
        }
        public async Task<List<EventActivityBoothWise>> GetEventListBoothWiseById(string stateId, string districtId, string assemblyId)
        {
            return await _eamsRepository.GetEventListBoothWiseById(stateId, districtId, assemblyId);
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
        public async Task<List<DashboardConnectedUser>> DashboardConnectedUser(DahboardMastersId dashboardMastersId, string roleType)
        {
            return await _eamsRepository.DashboardConnectedUser(dashboardMastersId, roleType);
        }


        #endregion

        #region SlotManagement
        public Task<Response> AddEventSlot(List<SlotManagementMaster> addEventSlot)
        {
            return _eamsRepository.AddEventSlot(addEventSlot);
        }

        public Task<List<SlotManagementMaster>> GetEventSlotList(int stateMasterId, int eventId)
        {
            return _eamsRepository.GetEventSlotList(stateMasterId, eventId);
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

        public async Task<Response> AddPollInterruption(PollInterruption pollInterruption)
        {

            var boothMasterRecord = await _eamsRepository.GetBoothRecord(Convert.ToInt32(pollInterruption.BoothMasterId));
            if (boothMasterRecord != null)
            {// check is evm deposited then no interruption

                ElectionInfoMaster electioninforecord = await _eamsRepository.GetElectionInfoRecord(boothMasterRecord.BoothMasterId);

                if (electioninforecord != null)
                {

                    if (electioninforecord.IsEVMDeposited == false || electioninforecord.IsEVMDeposited == null)
                    {
                        AssemblyMaster asembrecord = await _eamsRepository.GetAssemblyById(boothMasterRecord.AssemblyMasterId.ToString());
                        var pollInterruptionRecord = await _eamsRepository.GetPollInterruptionData(pollInterruption.BoothMasterId.ToString());

                        if (pollInterruptionRecord == null) // if no poll added in table
                        {
                            // check stop time or if resume time only as it is Fresh record

                            if (pollInterruption.StopTime != null && pollInterruption.ResumeTime != null)
                            {

                                // check both time in HHM format && comaprison wd each other and from current time
                                bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString()); bool isResumeformat = IsHHmmFormat(pollInterruption.ResumeTime.ToString());
                                if (isStopformat == true && isResumeformat == true)
                                {
                                    bool StopTimeisLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
                                    bool ResumeTimeisLessEqualToCurrentTime = ResumeTimeConvertTimeOnly(pollInterruption.ResumeTime.ToString());
                                    if (StopTimeisLessEqualToCurrentTime)
                                    {
                                        if (ResumeTimeisLessEqualToCurrentTime)
                                        {
                                            bool isResumeGreaterOrEqualToStopTime = CompareStopandResumeTime(pollInterruption.StopTime.ToString(), pollInterruption.ResumeTime.ToString());
                                            if (isResumeGreaterOrEqualToStopTime)
                                            {
                                                PollInterruption pollInterruptionData = new PollInterruption()
                                                {
                                                    StateMasterId = boothMasterRecord.StateMasterId,
                                                    DistrictMasterId = boothMasterRecord.DistrictMasterId,
                                                    AssemblyMasterId = boothMasterRecord.AssemblyMasterId,
                                                    BoothMasterId = boothMasterRecord.BoothMasterId,
                                                    PCMasterId = asembrecord.PCMasterId,

                                                };
                                                if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)
                                                // if evm fault- old cu and l bu enter
                                                {
                                                    // then check old cu bu entry
                                                    if (pollInterruption.OldCU != null && pollInterruption.OldBU != null && pollInterruption.NewBU != null && pollInterruption.NewCU != null)
                                                    {
                                                        pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                        pollInterruptionData.NewCU = pollInterruption.NewCU;
                                                        pollInterruptionData.NewBU = pollInterruption.NewBU;
                                                        pollInterruptionData.OldCU = pollInterruption.OldCU;
                                                        pollInterruptionData.OldBU = pollInterruption.OldBU;
                                                        pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                        pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                        pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                        pollInterruptionData.Flag = InterruptionCategory.Both.ToString();
                                                        pollInterruptionData.CreatedAt = BharatDateTime();
                                                        pollInterruptionData.UpdatedAt = BharatDateTime();
                                                        pollInterruptionData.IsPollInterrupted = false;
                                                        pollInterruptionData.Remarks = pollInterruption.Remarks;

                                                        var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter Old CU,Old BU, New CU & New BU Value" };
                                                    }



                                                }
                                                else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)
                                                {

                                                    pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                    pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                    pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                    pollInterruptionData.Flag = InterruptionCategory.Both.ToString();
                                                    pollInterruptionData.CreatedAt = BharatDateTime();
                                                    pollInterruptionData.UpdatedAt = BharatDateTime();
                                                    pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                    pollInterruptionData.IsPollInterrupted = false;
                                                    pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                    var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                    return result;
                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                                }



                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be less than Stop Time" };

                                            }

                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be greater than Current Time" };

                                        }
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };

                                    }


                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Time formats should be in 24Hr. format" };

                                }
                            }
                            else if (pollInterruption.StopTime != null && pollInterruption.ResumeTime == null)
                            {
                                // user is entering only stopTime, so check hhm format && comprae from current time
                                bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString());

                                if (isStopformat == true)
                                {
                                    bool StopTimeisLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
                                    if (StopTimeisLessEqualToCurrentTime)
                                    {
                                        PollInterruption pollInterruptionData = new PollInterruption()
                                        {
                                            StateMasterId = boothMasterRecord.StateMasterId,
                                            DistrictMasterId = boothMasterRecord.DistrictMasterId,
                                            AssemblyMasterId = boothMasterRecord.AssemblyMasterId,
                                            BoothMasterId = boothMasterRecord.BoothMasterId,
                                            PCMasterId = asembrecord.PCMasterId,
                                        };
                                        if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)
                                        {

                                            if (pollInterruption.OldBU != string.Empty && pollInterruption.OldCU != string.Empty)
                                            {
                                                pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                pollInterruptionData.OldCU = pollInterruption.OldCU;
                                                pollInterruptionData.OldBU = pollInterruption.OldBU;
                                                pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                                                pollInterruptionData.CreatedAt = BharatDateTime();
                                                pollInterruptionData.UpdatedAt = BharatDateTime();
                                                pollInterruptionData.IsPollInterrupted = true;
                                                pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                return result;
                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter OLD CU and Old BU Values." };
                                            }


                                        }
                                        else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)
                                        {

                                            pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                            pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                            pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                                            pollInterruptionData.CreatedAt = BharatDateTime();
                                            pollInterruptionData.UpdatedAt = BharatDateTime();
                                            pollInterruptionData.IsPollInterrupted = true;
                                            pollInterruptionData.Remarks = pollInterruption.Remarks;
                                            var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                            return result;
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                        }
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };
                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time format should be in 24Hr. format" };

                                }
                            }
                            else if (pollInterruption.StopTime == null && pollInterruption.ResumeTime != null)
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time cannot be Empty!" };

                            }

                        }

                        // Poll interrupted data Already in database like resolved or pending        
                        else
                        {
                            if (pollInterruptionRecord.StopTime != null && pollInterruptionRecord.ResumeTime != null)
                            {
                                if (pollInterruption.StopTime != null && pollInterruption.ResumeTime != null)
                                { // check both time in HHM format && comaprison wd each other and from current time
                                    bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString()); bool isResumeformat = IsHHmmFormat(pollInterruption.ResumeTime.ToString());
                                    if (isStopformat == true && isResumeformat == true)
                                    {
                                        // check last Resume time with pollInterruption.StopTime, it should be greater than stop
                                        bool IsNewStopGreaterLastResumeTime = CheckLastResumeTime2(pollInterruptionRecord.ResumeTime, pollInterruption.StopTime.ToString());
                                        if (IsNewStopGreaterLastResumeTime == true)
                                        {


                                            bool StopTimeisLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
                                            bool ResumeTimeisLessEqualToCurrentTime = ResumeTimeConvertTimeOnly(pollInterruption.ResumeTime.ToString());
                                            if (StopTimeisLessEqualToCurrentTime)
                                            {
                                                if (ResumeTimeisLessEqualToCurrentTime)
                                                {
                                                    bool isResumeGreaterOrEqualToStopTime = CompareStopandResumeTime(pollInterruption.StopTime.ToString(), pollInterruption.ResumeTime.ToString());
                                                    if (isResumeGreaterOrEqualToStopTime)
                                                    {
                                                        PollInterruption pollInterruptionData = new PollInterruption()
                                                        {
                                                            StateMasterId = pollInterruptionRecord.StateMasterId,
                                                            DistrictMasterId = pollInterruptionRecord.DistrictMasterId,
                                                            AssemblyMasterId = pollInterruptionRecord.AssemblyMasterId,
                                                            BoothMasterId = pollInterruptionRecord.BoothMasterId,
                                                            PCMasterId = asembrecord.PCMasterId,
                                                        };
                                                        if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)

                                                        {

                                                            if (pollInterruption.OldBU != string.Empty && pollInterruption.OldCU != string.Empty && pollInterruption.NewBU != string.Empty && pollInterruption.NewCU != string.Empty)
                                                            {
                                                                pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                                pollInterruptionData.NewCU = pollInterruption.NewCU;
                                                                pollInterruptionData.NewBU = pollInterruption.NewBU;
                                                                pollInterruptionData.OldCU = pollInterruption.OldCU;
                                                                pollInterruptionData.OldBU = pollInterruption.OldBU;
                                                                pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                                pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                                pollInterruptionData.Flag = InterruptionCategory.Both.ToString();
                                                                pollInterruptionData.CreatedAt = BharatDateTime();
                                                                pollInterruptionData.UpdatedAt = BharatDateTime();
                                                                pollInterruptionData.IsPollInterrupted = false;
                                                                pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                                var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                                return result;
                                                            }
                                                            else
                                                            {
                                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter Old CU-BU and New CU-BU Values" };

                                                            }


                                                        }
                                                        else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)
                                                        {

                                                            pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                            pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                            pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                            pollInterruptionData.Flag = InterruptionCategory.Both.ToString();
                                                            pollInterruptionData.CreatedAt = BharatDateTime();
                                                            pollInterruptionData.UpdatedAt = BharatDateTime();
                                                            pollInterruptionData.IsPollInterrupted = false;
                                                            pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                            var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                            return result;
                                                        }
                                                        else
                                                        {
                                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                                        }



                                                    }
                                                    else
                                                    {
                                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be less than Stop Time" };

                                                    }

                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be greater than Current Time" };

                                                }
                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };

                                            }

                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time should be greater than from Last Resume Time Entered" };
                                        }
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Time formats should be in 24Hr. format" };

                                    }
                                }
                                else if (pollInterruption.StopTime != null && pollInterruption.ResumeTime == null)
                                {
                                    // user is entering only stopTime, so check hhm format && comprae from current time
                                    bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString());

                                    if (isStopformat == true)
                                    {
                                        bool StopTimeisLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
                                        if (StopTimeisLessEqualToCurrentTime)
                                        {

                                            // check last entered resume , newstoptime must be greater than equal to lastrsume
                                            bool IsStopGreaterThanLastResumeTime = CheckLastResumeTime2(pollInterruptionRecord.ResumeTime, pollInterruption.StopTime.ToString());
                                            if (IsStopGreaterThanLastResumeTime == true)
                                            {
                                                PollInterruption pollInterruptionData = new PollInterruption()
                                                {
                                                    StateMasterId = boothMasterRecord.StateMasterId,
                                                    DistrictMasterId = boothMasterRecord.DistrictMasterId,
                                                    AssemblyMasterId = boothMasterRecord.AssemblyMasterId,
                                                    BoothMasterId = boothMasterRecord.BoothMasterId,
                                                    PCMasterId = asembrecord.PCMasterId,
                                                };
                                                if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)

                                                {
                                                    if (pollInterruption.OldBU != string.Empty && pollInterruption.OldCU != string.Empty)
                                                    {
                                                        pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                        pollInterruptionData.OldCU = pollInterruption.OldCU;
                                                        pollInterruptionData.OldBU = pollInterruption.OldBU;
                                                        pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                        pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                        pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                                                        pollInterruptionData.CreatedAt = BharatDateTime();
                                                        pollInterruptionData.UpdatedAt = BharatDateTime();
                                                        pollInterruptionData.IsPollInterrupted = true;
                                                        pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                        var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter Old CU-BU Values." };

                                                    }


                                                }
                                                else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)

                                                {

                                                    pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                    pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                    pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                                                    pollInterruptionData.CreatedAt = BharatDateTime();
                                                    pollInterruptionData.UpdatedAt = BharatDateTime();
                                                    pollInterruptionData.IsPollInterrupted = true;
                                                    pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                    var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                    return result;
                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                                }
                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Should be Greater than Last Entered Resume Time" };
                                            }
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };
                                        }
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time format should be in 24Hr. format" };

                                    }
                                }
                                else if (pollInterruption.StopTime == null && pollInterruption.ResumeTime != null)
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time cannot be Empty!" };

                                }



                            }

                            //case cleared
                            else if (pollInterruptionRecord.StopTime != null && pollInterruptionRecord.ResumeTime == null)
                            {
                                //need to enter Resume Time
                                if (pollInterruption.ResumeTime.ToString() != null)
                                {
                                    bool isResumeformat = IsHHmmFormat(pollInterruption.ResumeTime.ToString());

                                    if (isResumeformat == true)
                                    {
                                        bool ResumeTimeisLessEqualToCurrentTime = ResumeTimeConvertTimeOnly(pollInterruption.ResumeTime.ToString());
                                        if (ResumeTimeisLessEqualToCurrentTime == true)
                                        {

                                            bool IsNewResumeTimeGreaterLastStopTime = CheckLastStopTime(pollInterruptionRecord.StopTime, pollInterruption.ResumeTime.ToString());
                                            if (IsNewResumeTimeGreaterLastStopTime == true)
                                            {
                                                PollInterruption pollInterruptionData = new PollInterruption()
                                                {
                                                    StateMasterId = pollInterruptionRecord.StateMasterId,
                                                    DistrictMasterId = pollInterruptionRecord.DistrictMasterId,
                                                    AssemblyMasterId = pollInterruptionRecord.AssemblyMasterId,
                                                    BoothMasterId = pollInterruptionRecord.BoothMasterId,
                                                    PCMasterId = asembrecord.PCMasterId,
                                                };
                                                if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)

                                                {
                                                    if (pollInterruption.NewCU != string.Empty && pollInterruption.NewBU != string.Empty)
                                                    {
                                                        pollInterruptionData.InterruptionType = pollInterruptionRecord.InterruptionType;
                                                        pollInterruptionData.OldCU = pollInterruptionRecord.OldCU;
                                                        pollInterruptionData.OldBU = pollInterruptionRecord.OldBU;
                                                        pollInterruptionData.NewCU = pollInterruption.NewCU;
                                                        pollInterruptionData.NewBU = pollInterruption.NewBU;
                                                        pollInterruptionData.StopTime = pollInterruptionRecord.StopTime;
                                                        pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                        pollInterruptionData.Flag = InterruptionCategory.Resume.ToString();
                                                        pollInterruptionData.CreatedAt = BharatDateTime();
                                                        pollInterruptionData.UpdatedAt = BharatDateTime();
                                                        pollInterruptionData.IsPollInterrupted = false;
                                                        pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                        var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                        return result;

                                                    }
                                                    else
                                                    {
                                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter New CU-BU Value." };

                                                    }


                                                }
                                                else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)

                                                {

                                                    pollInterruptionData.StopTime = pollInterruptionRecord.StopTime;
                                                    pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                    pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                    pollInterruptionData.Flag = InterruptionCategory.Resume.ToString();
                                                    pollInterruptionData.CreatedAt = BharatDateTime();
                                                    pollInterruptionData.UpdatedAt = BharatDateTime();
                                                    pollInterruptionData.IsPollInterrupted = false;
                                                    pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                    var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                    return result;
                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                                }

                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.NotFound, Message = "Resume Time must be greater than Last Entered Stop Time." };

                                            }
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.NotFound, Message = "Resume Time Cannot be greater than Current Time." };

                                        }
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.NotFound, Message = "Resume Time must be in 24Hr Format." };
                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.NotFound, Message = "Please Enter Resume Time in 24Hr Format." };
                                }

                            }


                        }
                    }

                    else
                    {
                        return new Response { Status = RequestStatusEnum.NotFound, Message = "Can't Raise Interruption as EVM has been deposited." };

                    }

                }
                else
                {
                    // when no electioninfo record then also elgible
                    AssemblyMaster asembrecord = await _eamsRepository.GetAssemblyById(boothMasterRecord.AssemblyMasterId.ToString());
                    var pollInterruptionRecord = await _eamsRepository.GetPollInterruptionData(pollInterruption.BoothMasterId.ToString());

                    if (pollInterruptionRecord == null) // if no poll added in table
                    {
                        // check stop time or if resume time only as it is Fresh record

                        if (pollInterruption.StopTime != null && pollInterruption.ResumeTime != null)
                        {

                            // check both time in HHM format && comaprison wd each other and from current time
                            bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString()); bool isResumeformat = IsHHmmFormat(pollInterruption.ResumeTime.ToString());
                            if (isStopformat == true && isResumeformat == true)
                            {
                                bool StopTimeisLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
                                bool ResumeTimeisLessEqualToCurrentTime = ResumeTimeConvertTimeOnly(pollInterruption.ResumeTime.ToString());
                                if (StopTimeisLessEqualToCurrentTime)
                                {
                                    if (ResumeTimeisLessEqualToCurrentTime)
                                    {
                                        bool isResumeGreaterOrEqualToStopTime = CompareStopandResumeTime(pollInterruption.StopTime.ToString(), pollInterruption.ResumeTime.ToString());
                                        if (isResumeGreaterOrEqualToStopTime)
                                        {
                                            PollInterruption pollInterruptionData = new PollInterruption()
                                            {
                                                StateMasterId = boothMasterRecord.StateMasterId,
                                                DistrictMasterId = boothMasterRecord.DistrictMasterId,
                                                AssemblyMasterId = boothMasterRecord.AssemblyMasterId,
                                                BoothMasterId = boothMasterRecord.BoothMasterId,
                                                PCMasterId = asembrecord.PCMasterId,

                                            };
                                            if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)
                                            // if evm fault- old cu and l bu enter
                                            {
                                                // then check old cu bu entry
                                                if (pollInterruption.OldCU != null && pollInterruption.OldBU != null && pollInterruption.NewBU != null && pollInterruption.NewCU != null)
                                                {
                                                    pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                    pollInterruptionData.NewCU = pollInterruption.NewCU;
                                                    pollInterruptionData.NewBU = pollInterruption.NewBU;
                                                    pollInterruptionData.OldCU = pollInterruption.OldCU;
                                                    pollInterruptionData.OldBU = pollInterruption.OldBU;
                                                    pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                    pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                    pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                    pollInterruptionData.Flag = InterruptionCategory.Both.ToString();
                                                    pollInterruptionData.CreatedAt = BharatDateTime();
                                                    pollInterruptionData.UpdatedAt = BharatDateTime();
                                                    pollInterruptionData.IsPollInterrupted = false;
                                                    pollInterruptionData.Remarks = pollInterruption.Remarks;

                                                    var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                    return result;
                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter Old CU,Old BU, New CU & New BU Value" };
                                                }



                                            }
                                            else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)
                                            {

                                                pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                pollInterruptionData.Flag = InterruptionCategory.Both.ToString();
                                                pollInterruptionData.CreatedAt = BharatDateTime();
                                                pollInterruptionData.UpdatedAt = BharatDateTime();
                                                pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                pollInterruptionData.IsPollInterrupted = false;
                                                pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                return result;
                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                            }



                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be less than Stop Time" };

                                        }

                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be greater than Current Time" };

                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };

                                }


                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Time formats should be in 24Hr. format" };

                            }
                        }
                        else if (pollInterruption.StopTime != null && pollInterruption.ResumeTime == null)
                        {
                            // user is entering only stopTime, so check hhm format && comprae from current time
                            bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString());

                            if (isStopformat == true)
                            {
                                bool StopTimeisLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
                                if (StopTimeisLessEqualToCurrentTime)
                                {
                                    PollInterruption pollInterruptionData = new PollInterruption()
                                    {
                                        StateMasterId = boothMasterRecord.StateMasterId,
                                        DistrictMasterId = boothMasterRecord.DistrictMasterId,
                                        AssemblyMasterId = boothMasterRecord.AssemblyMasterId,
                                        BoothMasterId = boothMasterRecord.BoothMasterId,
                                        PCMasterId = asembrecord.PCMasterId,
                                    };
                                    if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)
                                    {

                                        if (pollInterruption.OldBU != string.Empty && pollInterruption.OldCU != string.Empty)
                                        {
                                            pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                            pollInterruptionData.OldCU = pollInterruption.OldCU;
                                            pollInterruptionData.OldBU = pollInterruption.OldBU;
                                            pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                            pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                            pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                                            pollInterruptionData.CreatedAt = BharatDateTime();
                                            pollInterruptionData.UpdatedAt = BharatDateTime();
                                            pollInterruptionData.IsPollInterrupted = true;
                                            pollInterruptionData.Remarks = pollInterruption.Remarks;
                                            var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                            return result;
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter OLD CU and Old BU Values." };
                                        }


                                    }
                                    else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)
                                    {

                                        pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                        pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                        pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                                        pollInterruptionData.CreatedAt = BharatDateTime();
                                        pollInterruptionData.UpdatedAt = BharatDateTime();
                                        pollInterruptionData.IsPollInterrupted = true;
                                        pollInterruptionData.Remarks = pollInterruption.Remarks;
                                        var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                        return result;
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };
                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time format should be in 24Hr. format" };

                            }
                        }
                        else if (pollInterruption.StopTime == null && pollInterruption.ResumeTime != null)
                        {
                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time cannot be Empty!" };

                        }

                    }

                    // Poll interrupted data Already in database like resolved or pending        
                    else
                    {
                        if (pollInterruptionRecord.StopTime != null && pollInterruptionRecord.ResumeTime != null)
                        {
                            if (pollInterruption.StopTime != null && pollInterruption.ResumeTime != null)
                            { // check both time in HHM format && comaprison wd each other and from current time
                                bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString()); bool isResumeformat = IsHHmmFormat(pollInterruption.ResumeTime.ToString());
                                if (isStopformat == true && isResumeformat == true)
                                {
                                    // check last Resume time with pollInterruption.StopTime, it should be greater than stop
                                    bool IsNewStopGreaterLastResumeTime = CheckLastResumeTime2(pollInterruptionRecord.ResumeTime, pollInterruption.StopTime.ToString());
                                    if (IsNewStopGreaterLastResumeTime == true)
                                    {


                                        bool StopTimeisLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
                                        bool ResumeTimeisLessEqualToCurrentTime = ResumeTimeConvertTimeOnly(pollInterruption.ResumeTime.ToString());
                                        if (StopTimeisLessEqualToCurrentTime)
                                        {
                                            if (ResumeTimeisLessEqualToCurrentTime)
                                            {
                                                bool isResumeGreaterOrEqualToStopTime = CompareStopandResumeTime(pollInterruption.StopTime.ToString(), pollInterruption.ResumeTime.ToString());
                                                if (isResumeGreaterOrEqualToStopTime)
                                                {
                                                    PollInterruption pollInterruptionData = new PollInterruption()
                                                    {
                                                        StateMasterId = pollInterruptionRecord.StateMasterId,
                                                        DistrictMasterId = pollInterruptionRecord.DistrictMasterId,
                                                        AssemblyMasterId = pollInterruptionRecord.AssemblyMasterId,
                                                        BoothMasterId = pollInterruptionRecord.BoothMasterId,
                                                        PCMasterId = asembrecord.PCMasterId,
                                                    };
                                                    if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)

                                                    {

                                                        if (pollInterruption.OldBU != string.Empty && pollInterruption.OldCU != string.Empty && pollInterruption.NewBU != string.Empty && pollInterruption.NewCU != string.Empty)
                                                        {
                                                            pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                            pollInterruptionData.NewCU = pollInterruption.NewCU;
                                                            pollInterruptionData.NewBU = pollInterruption.NewBU;
                                                            pollInterruptionData.OldCU = pollInterruption.OldCU;
                                                            pollInterruptionData.OldBU = pollInterruption.OldBU;
                                                            pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                            pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                            pollInterruptionData.Flag = InterruptionCategory.Both.ToString();
                                                            pollInterruptionData.CreatedAt = BharatDateTime();
                                                            pollInterruptionData.UpdatedAt = BharatDateTime();
                                                            pollInterruptionData.IsPollInterrupted = false;
                                                            pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                            var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                            return result;
                                                        }
                                                        else
                                                        {
                                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter Old CU-BU and New CU-BU Values" };

                                                        }


                                                    }
                                                    else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)
                                                    {

                                                        pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                        pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                        pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                        pollInterruptionData.Flag = InterruptionCategory.Both.ToString();
                                                        pollInterruptionData.CreatedAt = BharatDateTime();
                                                        pollInterruptionData.UpdatedAt = BharatDateTime();
                                                        pollInterruptionData.IsPollInterrupted = false;
                                                        pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                        var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                                    }



                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be less than Stop Time" };

                                                }

                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Resume Time Cannot be greater than Current Time" };

                                            }
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };

                                        }

                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time should be greater than from Last Resume Time Entered" };
                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Time formats should be in 24Hr. format" };

                                }
                            }
                            else if (pollInterruption.StopTime != null && pollInterruption.ResumeTime == null)
                            {
                                // user is entering only stopTime, so check hhm format && comprae from current time
                                bool isStopformat = IsHHmmFormat(pollInterruption.StopTime.ToString());

                                if (isStopformat == true)
                                {
                                    bool StopTimeisLessEqualToCurrentTime = StopTimeConvertTimeOnly(pollInterruption.StopTime.ToString());
                                    if (StopTimeisLessEqualToCurrentTime)
                                    {

                                        // check last entered resume , newstoptime must be greater than equal to lastrsume
                                        bool IsStopGreaterThanLastResumeTime = CheckLastResumeTime2(pollInterruptionRecord.ResumeTime, pollInterruption.StopTime.ToString());
                                        if (IsStopGreaterThanLastResumeTime == true)
                                        {
                                            PollInterruption pollInterruptionData = new PollInterruption()
                                            {
                                                StateMasterId = boothMasterRecord.StateMasterId,
                                                DistrictMasterId = boothMasterRecord.DistrictMasterId,
                                                AssemblyMasterId = boothMasterRecord.AssemblyMasterId,
                                                BoothMasterId = boothMasterRecord.BoothMasterId,
                                                PCMasterId = asembrecord.PCMasterId,
                                            };
                                            if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)

                                            {
                                                if (pollInterruption.OldBU != string.Empty && pollInterruption.OldCU != string.Empty)
                                                {
                                                    pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                    pollInterruptionData.OldCU = pollInterruption.OldCU;
                                                    pollInterruptionData.OldBU = pollInterruption.OldBU;
                                                    pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                    pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                    pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                                                    pollInterruptionData.CreatedAt = BharatDateTime();
                                                    pollInterruptionData.UpdatedAt = BharatDateTime();
                                                    pollInterruptionData.IsPollInterrupted = true;
                                                    pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                    var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                    return result;
                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter Old CU-BU Values." };

                                                }


                                            }
                                            else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)

                                            {

                                                pollInterruptionData.StopTime = TimeOnly.ParseExact(pollInterruption.StopTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                pollInterruptionData.Flag = InterruptionCategory.Stop.ToString();
                                                pollInterruptionData.CreatedAt = BharatDateTime();
                                                pollInterruptionData.UpdatedAt = BharatDateTime();
                                                pollInterruptionData.IsPollInterrupted = true;
                                                pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                return result;
                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                            }
                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Should be Greater than Last Entered Resume Time" };
                                        }
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time Cannot be greater than Current Time" };
                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time format should be in 24Hr. format" };

                                }
                            }
                            else if (pollInterruption.StopTime == null && pollInterruption.ResumeTime != null)
                            {
                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Stop Time cannot be Empty!" };

                            }



                        }

                        //case cleared
                        else if (pollInterruptionRecord.StopTime != null && pollInterruptionRecord.ResumeTime == null)
                        {
                            //need to enter Resume Time
                            if (pollInterruption.ResumeTime.ToString() != null)
                            {
                                bool isResumeformat = IsHHmmFormat(pollInterruption.ResumeTime.ToString());

                                if (isResumeformat == true)
                                {
                                    bool ResumeTimeisLessEqualToCurrentTime = ResumeTimeConvertTimeOnly(pollInterruption.ResumeTime.ToString());
                                    if (ResumeTimeisLessEqualToCurrentTime == true)
                                    {

                                        bool IsNewResumeTimeGreaterLastStopTime = CheckLastStopTime(pollInterruptionRecord.StopTime, pollInterruption.ResumeTime.ToString());
                                        if (IsNewResumeTimeGreaterLastStopTime == true)
                                        {
                                            PollInterruption pollInterruptionData = new PollInterruption()
                                            {
                                                StateMasterId = pollInterruptionRecord.StateMasterId,
                                                DistrictMasterId = pollInterruptionRecord.DistrictMasterId,
                                                AssemblyMasterId = pollInterruptionRecord.AssemblyMasterId,
                                                BoothMasterId = pollInterruptionRecord.BoothMasterId,
                                                PCMasterId = asembrecord.PCMasterId,
                                            };
                                            if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.EVMFault)

                                            {
                                                if (pollInterruption.NewCU != string.Empty && pollInterruption.NewBU != string.Empty)
                                                {
                                                    pollInterruptionData.InterruptionType = pollInterruptionRecord.InterruptionType;
                                                    pollInterruptionData.OldCU = pollInterruptionRecord.OldCU;
                                                    pollInterruptionData.OldBU = pollInterruptionRecord.OldBU;
                                                    pollInterruptionData.NewCU = pollInterruption.NewCU;
                                                    pollInterruptionData.NewBU = pollInterruption.NewBU;
                                                    pollInterruptionData.StopTime = pollInterruptionRecord.StopTime;
                                                    pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                    pollInterruptionData.Flag = InterruptionCategory.Resume.ToString();
                                                    pollInterruptionData.CreatedAt = BharatDateTime();
                                                    pollInterruptionData.UpdatedAt = BharatDateTime();
                                                    pollInterruptionData.IsPollInterrupted = false;
                                                    pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                    var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                    return result;

                                                }
                                                else
                                                {
                                                    return new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Enter New CU-BU Value." };

                                                }


                                            }
                                            else if ((InterruptionReason)pollInterruption.InterruptionType == InterruptionReason.LawAndOrder)

                                            {

                                                pollInterruptionData.StopTime = pollInterruptionRecord.StopTime;
                                                pollInterruptionData.ResumeTime = TimeOnly.ParseExact(pollInterruption.ResumeTime.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                                                pollInterruptionData.InterruptionType = pollInterruption.InterruptionType;
                                                pollInterruptionData.Flag = InterruptionCategory.Resume.ToString();
                                                pollInterruptionData.CreatedAt = BharatDateTime();
                                                pollInterruptionData.UpdatedAt = BharatDateTime();
                                                pollInterruptionData.IsPollInterrupted = false;
                                                pollInterruptionData.Remarks = pollInterruption.Remarks;
                                                var result = await _eamsRepository.AddPollInterruption(pollInterruptionData);
                                                return result;
                                            }
                                            else
                                            {
                                                return new Response { Status = RequestStatusEnum.BadRequest, Message = "Reason is not Valid" };
                                            }

                                        }
                                        else
                                        {
                                            return new Response { Status = RequestStatusEnum.NotFound, Message = "Resume Time must be greater than Last Entered Stop Time." };

                                        }
                                    }
                                    else
                                    {
                                        return new Response { Status = RequestStatusEnum.NotFound, Message = "Resume Time Cannot be greater than Current Time." };

                                    }
                                }
                                else
                                {
                                    return new Response { Status = RequestStatusEnum.NotFound, Message = "Resume Time must be in 24Hr Format." };
                                }
                            }
                            else
                            {
                                return new Response { Status = RequestStatusEnum.NotFound, Message = "Please Enter Resume Time in 24Hr Format." };
                            }

                        }


                    }
                }
            }
            else
            {
                return new Response { Status = RequestStatusEnum.NotFound, Message = "Booth Record Not Found" };
            }
            return null;
        }

        //public async Task<PollInterruption> GetPollInterruption(string boothMasterId)
        //{
        //    var res = await _eamsRepository.GetPollInterruptionData(boothMasterId);
        //    var boothExists = await _eamsRepository.GetBoothRecord(Convert.ToInt32(boothMasterId));
        //    if (res.StopTime != null && res.ResumeTime != null)
        //    {
        //        PollInterruption pollInterruptionData = new PollInterruption()
        //        {
        //            StateMasterId = res.StateMasterId,
        //            DistrictMasterId = res.DistrictMasterId,
        //            AssemblyMasterId = res.AssemblyMasterId,
        //            BoothMasterId = res.BoothMasterId,
        //            StopTime = res.StopTime,
        //            ResumeTime = res.ResumeTime,
        //            PollInterruptionId = res.PollInterruptionId,
        //            InterruptionType = res.InterruptionType,
        //            Flag = "New",
        //            UpdatedAt = res.UpdatedAt,
        //            IsPollInterrupted = false,


        //        };
        //        return pollInterruptionData;
        //    }
        //    else if (res.StopTime != null && res.ResumeTime == null)
        //    {
        //        PollInterruption pollInterruptionData = new PollInterruption()
        //        {
        //            StateMasterId = res.StateMasterId,
        //            DistrictMasterId = res.DistrictMasterId,
        //            AssemblyMasterId = res.AssemblyMasterId,
        //            BoothMasterId = res.BoothMasterId,
        //            StopTime = res.StopTime,
        //            ResumeTime = res.ResumeTime,
        //            PollInterruptionId = res.PollInterruptionId,
        //            InterruptionType = res.InterruptionType,
        //            Flag = "Resume",
        //            UpdatedAt = res.UpdatedAt,
        //            IsPollInterrupted = true,


        //        };
        //        return pollInterruptionData;
        //    }
        //    else if (res == null)
        //    {
        //        PollInterruption pollInterruptionData = new PollInterruption()
        //        {
        //            StateMasterId = res.StateMasterId,
        //            DistrictMasterId = res.DistrictMasterId,
        //            AssemblyMasterId = res.AssemblyMasterId,
        //            BoothMasterId = res.BoothMasterId,
        //            StopTime = res.StopTime,
        //            ResumeTime = res.ResumeTime,
        //            PollInterruptionId = res.PollInterruptionId,
        //            InterruptionType = res.InterruptionType,
        //            Flag = "Initial",
        //            IsPollInterrupted = false,


        //        };
        //        return pollInterruptionData;
        //    }
        //    else
        //    {
        //        PollInterruption pollInterruptionData = new PollInterruption()
        //        {
        //            StateMasterId = boothExists.StateMasterId,
        //            DistrictMasterId = boothExists.DistrictMasterId,
        //            AssemblyMasterId = boothExists.AssemblyMasterId,
        //            BoothMasterId = boothExists.BoothMasterId,
        //            StopTime = null,
        //            ResumeTime = null,
        //            Flag = "Initial",
        //            IsPollInterrupted = false,


        //        };
        //        return pollInterruptionData;

        //    }
        //}


        public Task<PollInterruption> GetPollInterruptionbyId(string boothMasterId)
        {
            return _eamsRepository.GetPollInterruptionData(boothMasterId);
        }

        public Task<List<PollInterruptionHistoryModel>> GetPollInterruptionHistoryById(string boothMasterId)
        {
            return _eamsRepository.GetPollInterruptionHistoryById(boothMasterId);
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

        public Task<List<PollInterruptionDashboard>> GetPollInterruptionDashboard(ClaimsIdentity claimsIdentity)
        {
            return _eamsRepository.GetPollInterruptionDashboard(claimsIdentity);
        }

        public Task<int> GetPollInterruptionDashboardCount(ClaimsIdentity claimsIdentity)
        {
            return _eamsRepository.GetPollInterruptionDashboardCount(claimsIdentity);
        }

        public Task<List<PollInterruptionDashboard>> GetBoothListBySoIdfoInterruption(ClaimsIdentity claimsIdentity)
        {
            return _eamsRepository.GetBoothListBySoIdfoInterruption(claimsIdentity);
        }

        #endregion

        #region common Methods Event Activity
        static bool IsHHmmFormat(string timeString)
        {
            DateTime dummyDate; // A dummy date to use for parsing
            return DateTime.TryParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dummyDate);
        }

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
        public async Task<List<ConsolidateBoothReport>> GetConsolidateBoothReports(BoothReportModel boothReportModel)
        {

            return await _eamsRepository.GetConsolidateBoothReports(boothReportModel);
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
        public async Task<List<VoterTurnOutSlotWise>> GetVoterTurnOutSlotBasedReport(string stateMasterId)
        {

            return await _eamsRepository.GetVoterTurnOutSlotBasedReport(stateMasterId);
        }
        public async Task<List<AssemblyVoterTurnOutSlotWise>> GetSlotVTReporttAssemblyWise(string stateMasterId, string districtMasterId)
        {

            return await _eamsRepository.GetSlotVTReporttAssemblyWise(stateMasterId, districtMasterId);
        }
        public async Task<List<BoothWiseVoterTurnOutSlotWise>> GetSlotVTReportBoothWise(string stateMasterId, string districtMasterId, string assemblyId)
        {

            return await _eamsRepository.GetSlotVTReportBoothWise(stateMasterId, districtMasterId, assemblyId);
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


        #region
        public async Task<List<ChartConsolidatedReport>> GetChartConsolidatedReport(ChartReportModel chartReportModel)
        {

            return await _eamsRepository.GetChartConsolidatedReport(chartReportModel);
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
        public async Task<List<SectorOfficerPendencyAssembly>> GetAssemblyWiseSOCountEventWiseCount(string stateMasterId, string districtmasterid)
        {

            return await _eamsRepository.GetAssemblyWiseSOCountEventWiseCount(stateMasterId, districtmasterid);
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
            return await _eamsRepository.AddKYCDetails(kyc);
        }
        public async Task<ServiceResponse> UpdateKycDetails(Kyc kyc)
        {
            return await _eamsRepository.UpdateKycDetails(kyc);
        }

        public async Task<List<Kyc>> GetKYCDetails()
        {
            return await _eamsRepository.GetKYCDetails();

        }
        public async Task<List<KycList>> GetKYCDetailByFourthLevelId(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelhMasterId)
        {
            return await _eamsRepository.GetKYCDetailByFourthLevelId(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelhMasterId);
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
        public async Task<Response> DeleteFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            return await _eamsRepository.DeleteFourthLevelHById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
        }
        #endregion

        #region BlockPanchayat
        public async Task<Response> AddBlockPanchayat(BlockZonePanchayat blockPanchayat)
        {

            return await _eamsRepository.AddBlockPanchayat(blockPanchayat);
        }
        public async Task<List<BlockZonePanchayat>> GetBlockPanchayatListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            return await _eamsRepository.GetBlockPanchayatListById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
        }
        public async Task<BlockZonePanchayat> GetBlockZonePanchayatById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int blockZonePanchayatMasterId)
        {
            return await _eamsRepository.GetBlockZonePanchayatById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId, blockZonePanchayatMasterId);
        }
        public async Task<Response> UpdateBlockZonePanchayat(BlockZonePanchayat blockZonePanchayat)
        {
            return await _eamsRepository.UpdateBlockZonePanchayat(blockZonePanchayat);
        }
        public async Task<Response> DeleteBlockZonePanchayatById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int blockZonePanchayatMasterId)
        {
            return await _eamsRepository.DeleteBlockZonePanchayatById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId, blockZonePanchayatMasterId);
        }
        #endregion

        #region SarpanchWards
        public async Task<Response> AddSarpanchWards(SarpanchWards sarpanchWards)
        {

            return await _eamsRepository.AddSarpanchWards(sarpanchWards);
        }
        public async Task<List<SarpanchWards>> GetSarpanchWardsListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int BlockZonePanchayatMasterId)
        {
            return await _eamsRepository.GetSarpanchWardsListById(stateMasterId, districtMasterId, assemblyMasterId, FourthLevelHMasterId, BlockZonePanchayatMasterId);
        }
        public async Task<Response> UpdateSarpanchWards(SarpanchWards sarpanchWards)
        {
            return await _eamsRepository.UpdateSarpanchWards(sarpanchWards);
        }

        public async Task<SarpanchWards> GetSarpanchWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int BlockZonePanchayatMasterId, int SarpanchWardsMasterId)
        {
            return await _eamsRepository.GetSarpanchWardsById(stateMasterId, districtMasterId, assemblyMasterId, FourthLevelHMasterId, BlockZonePanchayatMasterId, SarpanchWardsMasterId);
        }

        public async Task<Response> DeleteSarpanchWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int BlockZonePanchayatMasterId, int SarpanchWardsMasterId)
        {
            return await _eamsRepository.DeleteSarpanchWardsById(stateMasterId, districtMasterId, assemblyMasterId, FourthLevelHMasterId, BlockZonePanchayatMasterId, SarpanchWardsMasterId);
        }

        #endregion
    }
}
