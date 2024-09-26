using AutoMapper;
using EAMS.Helper;
using EAMS.ViewModels;
using EAMS.ViewModels.BLOMaster;
using EAMS.ViewModels.PSFormViewModel;
using EAMS.ViewModels.QueueViewModel;
using EAMS_ACore;
using EAMS_ACore.HelperModels;
using EAMS_ACore.IExternal;
using EAMS_ACore.Interfaces;
using EAMS_ACore.Models;
using EAMS_ACore.Models.BLOModels;
using EAMS_ACore.Models.EventActivityModels;
using EAMS_ACore.Models.PollingStationFormModels;
using EAMS_ACore.Models.QueueModel;
using LBPAMS.ViewModels;
using LBPAMS.ViewModels.EventActivityViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EAMSController : ControllerBase
    {
        private readonly ILogger<EAMSController> _logger;
        private readonly IEamsService _EAMSService;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        public EAMSController(IEamsService eamsService, IMapper mapper, ILogger<EAMSController> logger, ICacheService cacheService)
        {
            _EAMSService = eamsService;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }


        #region Reset Account
        [HttpGet]
        [Route("ResetAccount")]
        [Authorize]
        public async Task<IActionResult> ResetAccounts(string stateMasterId)
        {
            try
            {
                var resetResponse = await _EAMSService.ResetAccounts(stateMasterId);

                switch (resetResponse.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(resetResponse.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(resetResponse.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(resetResponse.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"StateList: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        #endregion

        #region MasterUpdation Status
        [HttpPut]
        [Route("UpdateMasterStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateMaster(UpdateMasterStatusViewModel updateMasterStatus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedData = _mapper.Map<UpdateMasterStatus>(updateMasterStatus);
                    var isSucceed = await _EAMSService.UpdateMasterStatus(mappedData);

                    if (isSucceed.IsSucceed)
                    {
                        //_logger.LogInformation("Master status updated successfully.");
                        return Ok(isSucceed);
                    }
                    else
                    {
                        // _logger.LogError($"Failed to update master status. Error: {isSucceed.Message}");
                        return BadRequest(isSucceed);
                    }
                }
                else
                {
                    var validationErrors = ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault();
                    // _logger.LogWarning($"UpdateMaster: {validationErrors}");
                    return BadRequest(validationErrors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateMaster: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        #endregion

        #region Master Deletion Status
        [HttpDelete]
        [Route("DeleteMasterStatus")]
        [Authorize]
        public async Task<IActionResult> DeleteMaster(string Id, string type)
        {
            DeleteMasterStatusViewModel deleteMasterStatusViewModel = new DeleteMasterStatusViewModel()
            {
                Id = Id,
                Type = type,
            };
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedData = _mapper.Map<DeleteMasterStatus>(deleteMasterStatusViewModel);
                    var isSucceed = await _EAMSService.DeleteMasterStatus(mappedData);

                    if (isSucceed.IsSucceed)
                    {
                        //_logger.LogInformation("Master status updated successfully.");
                        return Ok(isSucceed);
                    }
                    else
                    {
                        // _logger.LogError($"Failed to update master status. Error: {isSucceed.Message}");
                        return BadRequest(isSucceed);
                    }
                }
                else
                {
                    var validationErrors = ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault();
                    // _logger.LogWarning($"UpdateMaster: {validationErrors}");
                    return BadRequest(validationErrors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteMaster: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        #endregion

        #region State master
        [HttpGet]
        [Route("StateList")]
        //[Authorize] 
        public async Task<IActionResult> StateList()
        {
            try
            {
                var getState = await _cacheService.GetDataAsync<List<StateMasterViewModel>>("GetState");

                if (getState == null)
                {
                    // Cache miss, fetch data from the service
                    var stateList = await _EAMSService.GetState();
                    var mappedData = _mapper.Map<List<StateMasterViewModel>>(stateList);

                    // Optionally set data in cache
                    await _cacheService.SetDataAsync("GetState", mappedData, DateTimeOffset.Now.AddMinutes(5)); // Example expiration

                    var data = new
                    {
                        count = mappedData.Count,
                        data = mappedData
                    };
                    return Ok(data);
                }

                // Cache hit
                var dataFromCache = new
                {
                    count = getState.Count,
                    data = getState
                };
                return Ok(dataFromCache);
            }
            catch (Exception ex)
            {
                _logger.LogError($"StateList: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPut]
        [Route("UpdateStateById")]
        [Authorize]
        public async Task<IActionResult> UpdateStateById(StateMasterViewModel stateViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var electionType = User.Claims.FirstOrDefault(c => c.Type == "ElectionType").Value;

                    StateMaster stateMaster = new StateMaster()
                    {
                        StateMasterId = stateViewModel.StateId,
                        StateCode = stateViewModel.StateCode,
                        StateName = stateViewModel.StateName,
                        StateStatus = stateViewModel.IsStatus,
                        IsGenderCapturedinVoterTurnOut = stateViewModel.IsGenderCapturedinVoterTurnOut,

                    };
                    var state = await _EAMSService.UpdateStateById(stateMaster);
                    switch (state.Status)
                    {
                        case RequestStatusEnum.OK:
                            return Ok(state.Message);
                        case RequestStatusEnum.BadRequest:
                            return BadRequest(state.Message);
                        case RequestStatusEnum.NotFound:
                            return NotFound(state.Message);

                        default:
                            return StatusCode(500, "Internal Server Error");
                    }



                }
                else
                {
                    return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateStateById: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("AddState")]
        [Authorize]
        public async Task<IActionResult> AddState(AddStateMasterViewModel addStateMasterViewModel)
        {
            if (ModelState.IsValid)
            {
                var insertstate = _mapper.Map<AddStateMasterViewModel, StateMaster>(addStateMasterViewModel);
                var result = await _EAMSService.AddState(insertstate);

                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }



            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [Route("GetStateById")]
        [Authorize]
        public async Task<IActionResult> GetStateById(string stateMasterId)
        {
            var stateRecord = await _EAMSService.GetStateById(stateMasterId);
            if (stateRecord != null)
            {
                var mappedData = new
                {
                    StateMasterId = stateRecord.StateMasterId,
                    StateName = stateRecord.StateName,
                    StateCode = stateRecord.StateCode,
                    IsStatus = stateRecord.StateStatus,
                    IsGenderCapturedinVoterTurnOut = stateRecord.IsGenderCapturedinVoterTurnOut

                };
                return Ok(mappedData);
            }
            else
            {
                return NotFound($"[{stateMasterId}] not exist");
            }


        }


        #endregion

        #region District Master
        [HttpGet]
        [Route("DistrictList")]
        //[Authorize]
        public async Task<IActionResult> DistrictListById(string stateMasterId)
        {
            try
            {
                if (stateMasterId != null)
                {
                    var districtList = await _EAMSService.GetDistrictById(stateMasterId);
                    var mappedData = _mapper.Map<List<DistrictMasterViewModel>>(districtList);
                    if (stateMasterId != null)
                    {

                        var data = new
                        {
                            count = mappedData.Count,
                            data = mappedData.OrderBy(p => Int32.Parse(p.DistrictCode))
                        };
                        return Ok(data);
                    }
                    else
                    {
                        return NotFound("Data Not Found");
                    }
                }
                else
                {
                    return BadRequest(stateMasterId + "is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DistrictList: {ex.Message}");
                return BadRequest("Internal server error");
            }
        }

        [HttpPut]
        [Route("UpdateDistrictById")]
        [Authorize]
        public async Task<IActionResult> UpdateDistrictById(DistrictMasterViewModel districtViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<DistrictMasterViewModel, DistrictMaster>(districtViewModel);
                var result = await _EAMSService.UpdateDistrictById(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }


            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpPost]
        [Route("AddDistrict")]
        [Authorize]
        public async Task<IActionResult> AddDistrict(AddDistrictMasterViewModel addDistrictViewModel)
        {
            if (ModelState.IsValid)
            {

                var mappedData = _mapper.Map<AddDistrictMasterViewModel, DistrictMaster>(addDistrictViewModel);

                var result = await _EAMSService.AddDistrict(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("GetDistrictById")]
        [Authorize]
        public async Task<IActionResult> GetDistrictById(string distictMasterId)
        {
            var districtRecord = await _EAMSService.GetDistrictRecordById(distictMasterId);
            if (districtRecord != null)
            {
                var dataMapping = new
                {
                    StateMasterId = districtRecord.StateMasterId,
                    StateName = districtRecord.StateMaster.StateName,
                    DistrictMasterId = districtRecord.DistrictMasterId,
                    DistrictName = districtRecord.DistrictName,
                    DistrictCode = districtRecord.DistrictCode,
                    IsStatus = districtRecord.DistrictStatus

                };


                return Ok(dataMapping);
            }
            else
            {
                return NotFound($"[{distictMasterId}] not exist");
            }
        }

        #endregion

        #region Assembliy Master 
        [HttpGet]
        [Route("GetAssembliesListById")]
        //[Authorize]
        public async Task<IActionResult> GetAssembliesListById(string stateId, string districtId, string electionTypeId)
        {
            if (stateId != null && districtId != null)
            {
                var assemblyList = await _EAMSService.GetAssemblies(stateId, districtId, electionTypeId);  // Corrected to await the asynchronous method
                if (assemblyList != null)
                {
                    var data = new
                    {
                        count = assemblyList.Count,
                        data = assemblyList
                    };
                    return Ok(data);
                }
                else
                {
                    return NotFound("Data Not Found");
                }
            }
            else
            {
                return BadRequest("State and District Master Id's cannot be null");
            }


        }

        [HttpGet]
        [Route("GetAssembliesListByElectitionTypeId")]
        //[Authorize]
        public async Task<IActionResult> AssembliesListByElectitionTypeId(string stateId, string districtId, string electionTypeId)
        {
            if (stateId != null && districtId != null && electionTypeId != null)
            {
                var assemblyList = await _EAMSService.GetAssembliesByElectionType(stateId, districtId, electionTypeId);  // Corrected to await the asynchronous method
                if (assemblyList != null)
                {
                    var data = new
                    {
                        count = assemblyList.Count,
                        data = assemblyList
                    };
                    return Ok(data);
                }
                else
                {
                    return NotFound("Data Not Found");
                }
            }
            else
            {
                return BadRequest("State and District Master Id's cannot be null");
            }


        }

        [HttpPut]
        [Route("UpdateAssembliesById")]
        [Authorize]
        public async Task<IActionResult> UpdateAssembliesById(AssemblyMasterViewModel assemblyViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<AssemblyMasterViewModel, AssemblyMaster>(assemblyViewModel);
                var result = await _EAMSService.UpdateAssembliesById(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpPost]
        [Route("AddAssemblies")]
        [Authorize]
        public async Task<IActionResult> AddAssemblies(AddAssemblyMasterViewModel addAssemblyMasterViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<AddAssemblyMasterViewModel, AssemblyMaster>(addAssemblyMasterViewModel);


                if (addAssemblyMasterViewModel.TotalBooths > 0)
                {
                    var result = await _EAMSService.AddAssemblies(mappedData);
                    switch (result.Status)
                    {
                        case RequestStatusEnum.OK:
                            return Ok(result.Message);
                        case RequestStatusEnum.BadRequest:
                            return BadRequest(result.Message);
                        case RequestStatusEnum.NotFound:
                            return NotFound(result.Message);

                        default:
                            return StatusCode(500, "Internal Server Error");
                    }
                }
                else
                {
                    return BadRequest("For a Gram Panchayat, entering the total number of booths is not allowed.");
                }


            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("GetAssemblyById")]
        [Authorize]
        public async Task<IActionResult> GetAssemblyById(string assemblyMasterId)
        {
            var assemblyRecord = await _EAMSService.GetAssemblyById(assemblyMasterId);
            if (assemblyRecord != null)
            {
                var dataMapping = new
                {
                    StateMasterId = assemblyRecord.StateMasterId,
                    StateName = assemblyRecord.StateMaster.StateName,
                    DistrictMasterId = assemblyRecord.DistrictMaster.DistrictMasterId,
                    DistrictName = assemblyRecord.DistrictMaster.DistrictName,
                    DistrictCode = assemblyRecord.DistrictMaster.DistrictCode,
                    AssemblyMasterId = assemblyRecord.AssemblyMasterId,
                    AssemblyName = assemblyRecord.AssemblyName,
                    AssemblyCode = assemblyRecord.AssemblyCode,
                    AssemblyType = assemblyRecord.AssemblyType,
                    //PcMasterId = assemblyRecord.ParliamentConstituencyMaster.PCMasterId,
                    //PcName = assemblyRecord.ParliamentConstituencyMaster.PcName,
                    IsStatus = assemblyRecord.AssemblyStatus,
                    totalBooths = assemblyRecord.TotalBooths,
                    ElectionTypeMasterId = assemblyRecord.ElectionTypeMasterId,
                    ElectionTypeName = assemblyRecord.ElectionTypeMaster.ElectionType

                };


                return Ok(dataMapping);
            }
            else
            {
                return NotFound($"[{assemblyMasterId}] not exist");
            }
        }

        #endregion

        #region  FieldOfficer Master
        [HttpGet]
        [Route("GetFieldOfficersListById")]
        [Authorize]
        public async Task<IActionResult> GetFieldOfficersListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var foList = await _EAMSService.GetFieldOfficersListById(stateMasterId, districtMasterId, assemblyMasterId);  // Corrected to await the asynchronous method
            if (foList != null)
            {
                var data = new
                {
                    count = foList.Count,
                    data = foList
                };
                return Ok(data);
            }
            else
            {
                return BadRequest("No Record Found");
            }

        }

        [HttpGet]
        [Route("GetFieldOfficerProfile")]
        [Authorize]
        public async Task<IActionResult> GetFieldOfficerProfile()
        {
            var foIdClaim = User.Claims.FirstOrDefault(c => c.Type == "FieldOfficerMasterId")?.Value;
            var bloMasterIdClaim = User.Claims.FirstOrDefault(c => c.Type == "BLOMasterId")?.Value;
            if (foIdClaim is not null)
            {
                var foList = await _EAMSService.GetFieldOfficerProfile(foIdClaim, "FO");  // Corrected to await the asynchronous method

                if (foList != null)
                {
                    var data = new
                    {
                        data = foList
                    };
                    return Ok(data);
                }
                else
                {
                    return BadRequest("No Record Found");
                }
            }
            else
            {
                var bloList = await _EAMSService.GetFieldOfficerProfile(bloMasterIdClaim, "BLO");  // Corrected to await the asynchronous method

                if (bloList != null)
                {
                    var data = new
                    {
                        data = bloList
                    };
                    return Ok(data);
                }
                else
                {
                    return BadRequest("No Record Found");
                }
            }
        }

        [HttpPost]
        [Route("AddFieldOfficer")]
        public async Task<IActionResult> AddFieldOfficer(FieldOfficerViewModel fieldOfficerViewModel)
        {
            if (ModelState.IsValid)
            {

                var mappedData = _mapper.Map<FieldOfficerMaster>(fieldOfficerViewModel);
                var result = await _EAMSService.AddFieldOfficer(mappedData);

                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");

                }
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpPut]
        [Route("UpdateFieldOfficer")]
        public async Task<IActionResult> UpdateFieldOfficer(UpdateFieldOfficerViewModel updateFieldOfficerViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<FieldOfficerMaster>(updateFieldOfficerViewModel);

                var result = await _EAMSService.UpdateFieldOfficer(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }



            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }
        /// <summary this api for portal>
        [HttpGet]
        [Route("GetBoothListByFoId")]
        public async Task<IActionResult> GetBoothListByFoId(int stateMasterId, int districtMasterId, int assemblyMasterId, int foId)
        {
            var boothList = await _EAMSService.GetBoothListByFoId(stateMasterId, districtMasterId, assemblyMasterId, foId);  // Corrected to await the asynchronous method
            var mappedData = _mapper.Map<List<FieldOfficerBoothViewModel>>(boothList);
            var getUnassignedBoothList = await _EAMSService.GetUnassignedBoothListById(stateMasterId, districtMasterId, assemblyMasterId);  // Corrected to await the asynchronous method
            var unAssignedMappedData = _mapper.Map<List<CombinedMasterViewModel>>(getUnassignedBoothList);
            var data = new
            {
                AssignedCount = mappedData.Count,
                UnAssignedCount = unAssignedMappedData.Count,
                Assigned = mappedData.OrderBy(p => Int32.Parse(p.BoothCode_No)),
                Unassigned = unAssignedMappedData
            };
            return Ok(data);
        }


        /// <summary this api for Mobile App>
        [HttpGet]
        [Route("GetBoothListForFo")]
        [Authorize]
        public async Task<IActionResult> GetBoothListForFo()
        {
            int foId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "FieldOfficerMasterId")?.Value);
            int stateMasterId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value);
            int districtMasterId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value);
            int assemblyMasterId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value);

            var boothList = await _EAMSService.GetBoothListForFo(stateMasterId, districtMasterId, assemblyMasterId, foId);

            var mappedData = _mapper.Map<List<FieldOfficerBoothViewModel>>(boothList);
            var data = new
            {
                count = mappedData.Count,
                data = mappedData.OrderBy(p => Int32.Parse(p.BoothCode_No)),
            };
            return Ok(data);
        }


        [HttpGet]
        [Route("GetFieldOfficerById")]
        public async Task<IActionResult> GetFieldOfficerById(int FieldOfficerMasterId)
        {
            var foRecord = await _EAMSService.GetFieldOfficerById(FieldOfficerMasterId);
            if (foRecord != null)
            {



                return Ok(foRecord);
            }
            else
            {
                return NotFound($"[{FieldOfficerMasterId}] not exist");
            }

        }

        #endregion

        #region Booth Master
        /// <summary>

        /// <param name="stateMasterId"></param>
        /// <param name="districtMasterId"></param>
        /// <param name="assemblyMasterId"></param>
        /// <returns></returns>
        /// 
        /// </summary>
        [HttpGet]
        [Route("GetBoothListById")]
        public async Task<IActionResult> GetBoothListById(
                                                         string stateMasterId,
                                                         string districtMasterId,
                                                         string assemblyMasterId,
                                                         string fourthLevelHMasterId,
                                                         string? psZonePanchayatMasterId)
        {
            // Convert string parameters to integers
            if (!int.TryParse(stateMasterId, out int stateId) ||
                !int.TryParse(districtMasterId, out int districtId) ||
                !int.TryParse(assemblyMasterId, out int assemblyId) ||
                !int.TryParse(fourthLevelHMasterId, out int fourthLevelId))
            {
                return BadRequest("Invalid State, District, Assembly, or Fourth Level Master Id.");
            }

            // Check if psZonePanchayatMasterId is null or empty and parse it if needed
            int? psZonePanchayatId = string.IsNullOrEmpty(psZonePanchayatMasterId) ? (int?)null : (int?)int.Parse(psZonePanchayatMasterId);

            // Fetch the booth list based on the presence of psZonePanchayatMasterId
            List<CombinedMaster> boothList;
            if (psZonePanchayatId == null || psZonePanchayatId == 0)
            {
                // Call GetBoothListByFourthLevelId when psZonePanchayatMasterId is null
                boothList = await _EAMSService.GetBoothListByFourthLevelId(
                    stateId,
                    districtId,
                    assemblyId,
                    fourthLevelId);
            }
            else
            {
                // Call GetBoothListByPSZonePanchayatId when psZonePanchayatMasterId is provided
                boothList = await _EAMSService.GetBoothListByPSZonePanchayatId(
                    stateId,
                    districtId,
                    assemblyId,
                    fourthLevelId,
                    psZonePanchayatId.Value);
            }

            // Check if boothList is null or empty
            if (boothList == null || boothList.Count == 0)
            {
                return NotFound("Booth not found.");
            }

            // Prepare the response data
            var responseData = new
            {
                count = boothList.Count,
                data = boothList
            };

            // Return the booth list in a successful response
            return Ok(responseData);
        }

        [HttpGet]
        [Route("GetBoothListForARO")]
        [Authorize]
        public async Task<IActionResult> GetBoothListForARO()
        {
            Claim stateMaster = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
            Claim districtMaster = User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
            Claim assemblyMaster = User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
            string stateMasterId = stateMaster.Value;
            string districtMasterId = districtMaster.Value;
            string assemblyMasterId = assemblyMaster.Value;

            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null)
            {
                var boothList = await _EAMSService.GetBoothListById(stateMasterId, districtMasterId, assemblyMasterId);  // Corrected to await the asynchronous method
                if (boothList != null)
                {
                    var data = new
                    {
                        count = boothList.Count,
                        data = boothList.ToList()
                    };
                    return Ok(data);

                }
                else
                {
                    return NotFound("Booth Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }
        }

        [HttpGet]
        [Route("GetBoothListByIdforPSO")]
        [Authorize]
        public async Task<IActionResult> GetBoothListByIdforPSO()
        {
            var stateMaster = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var districtMaster = User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
            var assemblyMaster = User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;

            if (stateMaster == null || districtMaster == null || assemblyMaster == null)
            {
                return BadRequest("State, District, and Assembly Master Id's cannot be null");
            }

            var boothList = await _EAMSService.GetBoothListByIdforPSO(stateMaster, districtMaster, assemblyMaster);

            if (boothList == null)
            {
                return NotFound("Booth Not Found");
            }

            var data = new
            {
                count = boothList.Count,
                data = boothList.ToList()
            };

            return Ok(data);
        }



        /// <summary>
        /// Insert Booth Under Assembly, District, State
        /// </summary>
        /// <param name="stateViewModel"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("AddBooth")]
        [Authorize]
        public async Task<IActionResult> AddBooth(BoothMasterViewModel BoothMasterViewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {// check booths filled in assembly
                    var asemRecord = await _EAMSService.GetAssemblyById(BoothMasterViewModel.AssemblyMasterId.ToString());
                    var boothsEntered = await _EAMSService.GetBoothListById(BoothMasterViewModel.StateMasterId.ToString(), BoothMasterViewModel.DistrictMasterId.ToString(), BoothMasterViewModel.AssemblyMasterId.ToString());
                    if (BoothMasterViewModel.Male + BoothMasterViewModel.Female + BoothMasterViewModel.Transgender == BoothMasterViewModel.TotalVoters)
                    {
                        if (asemRecord != null)
                        {
                            if (asemRecord.ElectionTypeMasterId != 1)
                            {
                                if (asemRecord.TotalBooths > 0)
                                {
                                    if (boothsEntered.Count < asemRecord.TotalBooths)
                                    {
                                        var mappedData = _mapper.Map<BoothMasterViewModel, BoothMaster>(BoothMasterViewModel);
                                        var electionType = User.Claims.FirstOrDefault(c => c.Type == "ElectionTypeMasterId").Value;

                                        var result = await _EAMSService.AddBooth(mappedData);
                                        switch (result.Status)
                                        {
                                            case RequestStatusEnum.OK:
                                                return Ok(result.Message);
                                            case RequestStatusEnum.BadRequest:
                                                return BadRequest(result.Message);
                                            case RequestStatusEnum.NotFound:
                                                return NotFound(result.Message);

                                            default:
                                                return StatusCode(500, "Internal Server Error");
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest("You have already Entered " + asemRecord.TotalBooths + " " + ", cannot exceed from limit");

                                    }
                                }
                                else
                                {
                                    return StatusCode(500, "Please Enter Your Total Booths in Assembly");
                                }
                            }
                            else
                            {
                                var mappedData = _mapper.Map<BoothMasterViewModel, BoothMaster>(BoothMasterViewModel);
                                //var electionType = User.Claims.FirstOrDefault(c => c.Type == "ElectionTypeMasterId").Value;

                                var result = await _EAMSService.AddBooth(mappedData);
                                switch (result.Status)
                                {
                                    case RequestStatusEnum.OK:
                                        return Ok(result.Message);
                                    case RequestStatusEnum.BadRequest:
                                        return BadRequest(result.Message);
                                    case RequestStatusEnum.NotFound:
                                        return NotFound(result.Message);

                                    default:
                                        return StatusCode(500, "Internal Server Error");
                                }
                            }
                        }
                        else
                        {
                            return StatusCode(500, "Assembly Not Found");
                        }
                    }
                    else
                    {

                        return StatusCode(500, "The total sum of voters does not match the individual counts of Male, Female, and Transgender categories.");


                    }

                }
                else
                {

                    return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
                }

            }

            catch (Exception ex)
            {
                _logger.LogError($"AddBooth: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Route("UpdateBooth")]
        [Authorize]
        public async Task<IActionResult> UpdateBooth(UpdateBoothMasterViewModel boothMasterViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<BoothMaster>(boothMasterViewModel);

                var result = await _EAMSService.UpdateBooth(mappedData);

                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpPost]
        [Route("BoothMapping")]
        [Authorize]
        public async Task<IActionResult> BoothMapping(BoothViewModel boothMappingViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (boothMappingViewModel.BoothMasterId != null && boothMappingViewModel.BoothMasterId.Any() && boothMappingViewModel.IsAssigned == true && !string.IsNullOrWhiteSpace(boothMappingViewModel.AssignedTo))
                    {

                        List<BoothMaster> boothMasters = new List<BoothMaster>();

                        foreach (var boothMasterId in boothMappingViewModel.BoothMasterId)
                        {
                            var boothMaster = new BoothMaster
                            {
                                BoothMasterId = boothMasterId,
                                StateMasterId = boothMappingViewModel.StateMasterId,
                                DistrictMasterId = boothMappingViewModel.DistrictMasterId,
                                AssemblyMasterId = boothMappingViewModel.AssemblyMasterId,
                                AssignedBy = boothMappingViewModel.AssignedBy,
                                AssignedTo = boothMappingViewModel.AssignedTo,
                                IsAssigned = boothMappingViewModel.IsAssigned,
                                ElectionTypeMasterId = boothMappingViewModel.ElectionTypeMasterId,
                            };

                            boothMasters.Add(boothMaster);
                        }

                        var result = await _EAMSService.BoothMapping(boothMasters);
                        switch (result.Status)
                        {
                            case RequestStatusEnum.OK:
                                return Ok(result.Message);
                            case RequestStatusEnum.BadRequest:
                                return BadRequest(result.Message);
                            case RequestStatusEnum.NotFound:
                                return NotFound(result.Message);

                            default:
                                return StatusCode(500, "Internal Server Error");
                        }
                    }
                    else
                    {
                        return BadRequest(new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check the Parameters" });
                    }

                }
                else
                {
                    return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
                }


            }

            catch (Exception ex)
            {
                _logger.LogError($"BoothMapping: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Route("ReleaseBooth")]
        [Authorize]
        public async Task<IActionResult> ReleaseBooth(BoothReleaseViewModel boothReleaseViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var mapperdata = _mapper.Map<BoothMaster>(boothReleaseViewModel);
                    var boothReleaseResponse = await _EAMSService.ReleaseBooth(mapperdata);

                    switch (boothReleaseResponse.Status)
                    {
                        case RequestStatusEnum.OK:
                            return Ok(boothReleaseResponse.Message);
                        case RequestStatusEnum.BadRequest:
                            return BadRequest(boothReleaseResponse.Message);
                        case RequestStatusEnum.NotFound:
                            return NotFound(boothReleaseResponse.Message);

                        default:
                            return StatusCode(500, "Internal Server Error");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }

            }


            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("GetBoothById")]
        [Authorize]
        public async Task<IActionResult> GetBoothById(string boothMasterId)
        {
            var boothRecord = await _EAMSService.GetBoothById(boothMasterId);
            if (boothRecord != null)
            {
                var dataMapping = new
                {
                    StateMasterId = boothRecord.StateMasterId,
                    StateName = boothRecord.StateMaster.StateName,
                    DistrictMasterId = boothRecord.DistrictMaster.DistrictMasterId,
                    DistrictName = boothRecord.DistrictMaster.DistrictName,
                    DistrictCode = boothRecord.DistrictMaster.DistrictCode,
                    AssemblyMasterId = boothRecord.AssemblyMasterId,
                    AssemblyName = boothRecord.AssemblyMaster.AssemblyName,
                    AssemblyCode = boothRecord.AssemblyMaster.AssemblyCode,
                    AssemblyType = boothRecord.AssemblyMaster.AssemblyType,
                    fourthLevelHMasterId = boothRecord.FourthLevelH.FourthLevelHMasterId,
                    fourthLevelHName = boothRecord.FourthLevelH.HierarchyName,
                    BoothMasterId = boothRecord.BoothMasterId,
                    BoothName = boothRecord.BoothName,
                    BoothCode_No = boothRecord.BoothCode_No,
                    BoothNoAuxy = boothRecord.BoothNoAuxy,
                    TotalVoters = boothRecord.TotalVoters,
                    IsStatus = boothRecord.BoothStatus,
                    Male = boothRecord.Male,
                    Female = boothRecord.Female,
                    Transgender = boothRecord.Transgender,
                    LocationMasterId = boothRecord.LocationMasterId,
                    ElectionTypeMasterId = boothRecord.ElectionTypeMaster.ElectionTypeMasterId,
                    ElectionTypeName = boothRecord.ElectionTypeMaster.ElectionType,
                    IsPrimaryBooth = boothRecord.IsPrimaryBooth,


                };


                return Ok(dataMapping);
            }
            else
            {
                return NotFound($"[{boothMasterId}] not exist");
            }
        }
        
        [HttpGet]
        [Route("GetBoothDetailForVoterInQueue")]
        public async Task<IActionResult> GetBoothDetailForVoterInQueue(int boothMasterId)
        {
            var boothMasterDetail = await _EAMSService.GetBoothDetailForVoterInQueue(boothMasterId);
            if (boothMasterDetail != null)
            {
                return Ok(boothMasterDetail);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region Event Master

        [HttpGet]
        [Route("GetEventAbbrList")]
        [Authorize]
        public async Task<IActionResult> GetEventAbbrList()
        {
            var eventAbbrList = await _EAMSService.GetEventAbbrList();
            if (eventAbbrList != null)
            {

                return Ok(eventAbbrList);

            }
            else
            {
                return BadRequest("No Record Found");
            }

        }
        [HttpGet]
        [Route("GetEventListById")]
        [Authorize]
        public async Task<IActionResult> GetEventListById(int stateMasterId, int electionTypeMasterId)
        {
            var eventList = await _EAMSService.GetEventListById(stateMasterId, electionTypeMasterId);
            if (eventList != null)
            {
                var mappedEvent = _mapper.Map<List<EventMasterListViewModel>>(eventList);
                if (mappedEvent != null)
                {
                    var data = new
                    {
                        count = mappedEvent.Count,
                        data = mappedEvent
                    };
                    return Ok(data);
                }
                else
                {
                    return BadRequest("No Record Found");
                }
            }
            else
            {
                return BadRequest("No Record Found");
            }
        }

        [HttpPost]
        [Route("AddEvent")]
        [Authorize]
        public async Task<IActionResult> AddEvent(EventMasterViewModel eventMasterViewModel)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return bad request if the model state is invalid
            }

            // Map ViewModel to the EventMaster model
            var eventMaster = _mapper.Map<EventMasterViewModel, EventMaster>(eventMasterViewModel);

            // Ensure mapping succeeded
            if (eventMaster == null)
            {
                return BadRequest("Mapping failed. Invalid data."); // Return bad request with a specific message if mapping fails
            }

            // Add the event using the service
            var result = await _EAMSService.AddEvent(eventMaster);

            // Return the result with a status code based on the service response
            if (result.IsSucceed)
            {
                return Ok(result); // Return a 200 OK with the result on success
            }

            return BadRequest(result.Message); // Return a 400 BadRequest with the error message if the service fails
        }

        [HttpPut]
        [Route("UpdateEvent")]
        [Authorize]
        public async Task<IActionResult> UpdateEvent(UpdateEventMasterViewModel eventMasterViewModel)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return bad request if the model state is invalid
            }

            // Map ViewModel to the EventMaster model
            var eventMaster = _mapper.Map<UpdateEventMasterViewModel, EventMaster>(eventMasterViewModel);

            // Ensure mapping succeeded
            if (eventMaster == null)
            {
                return BadRequest("Mapping failed. Invalid data."); // Return bad request with a specific message if mapping fails
            }

            // Add the event using the service
            var result = await _EAMSService.UpdateEvent(eventMaster);

            // Return the result with a status code based on the service response
            if (result.IsSucceed)
            {
                return Ok(result); // Return a 200 OK with the result on success
            }

            return BadRequest(result.Message); // Return a 400 BadRequest with the error message if the service fails

        }
        [HttpPut]
        [Route("UpdateEventStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateEventStatus(UpdateEventStatusViewModel updateEventStatusViewModel)
        {
            if (ModelState.IsValid)
            {

                var mappedData = _mapper.Map<EventMaster>(updateEventStatusViewModel);
                var isSucced = await _EAMSService.UpdateEventStaus(mappedData);
                if (isSucced.IsSucceed)
                {
                    return Ok(isSucced);
                }
                else
                {
                    return BadRequest(isSucced);
                }
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("GetEventById")]
        [Authorize]
        public async Task<IActionResult> GetEventById(int eventMasterId)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Return the first validation error message
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();
                return BadRequest(errorMessage);
            }

            // Fetch the event by ID
            var result = await _EAMSService.GetEventById(eventMasterId);
            var mappedData = _mapper.Map<EventMasterByIdViewModel>(result);

            // Check if the event was found
            if (result == null)
            {
                return NotFound($"Event with ID {eventMasterId} not found.");
            }

            // Return the event
            return Ok(mappedData);
        }

        [HttpDelete]
        [Route("DeleteEventById")]
        [Authorize]
        public async Task<IActionResult> DeleteEventById(int eventMasterId)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Return the first validation error message
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();
                return BadRequest(errorMessage);
            }

            // Fetch the event by ID
            var result = await _EAMSService.DeleteEventById(eventMasterId);

            // Check if the event was found
            if (result == null)
            {
                return NotFound($"Event with ID {eventMasterId} not found.");
            }

            // Return the event
            return Ok(result);
        }

        [HttpGet]
        [Route("GetBoothInfoinPollDetail")]
        [Authorize(Roles = "SuperAdmin,ARO,SO")]
        public async Task<IActionResult> GetBoothInfoinPollDetail(string eventId, string? soUserId)
        {

            string soId = "";


            if (soUserId is null) // for mobile token
            {
                var soIdClaim = User.Claims.FirstOrDefault(c => c.Type == "SoId");
                if (soIdClaim == null)
                {
                    // Handle the case where the SoId claim is not present
                    return BadRequest("SoId claim not found.");
                }
                else
                {
                    soId = soIdClaim.Value;
                }

            }
            else // for portal
            {
                soId = soUserId;
            }
            var eventWiseBoothList = await _EAMSService.GetBoothInfoinPollDetail(soId, eventId);

            if (eventWiseBoothList != null)
            {
                var data = new
                {
                    count = eventWiseBoothList.Count,
                    //data = eventWiseBoothList.OrderBy(p => Int32.Parse(p.BoothCode))
                    data = eventWiseBoothList.OrderBy(p => p.SortColor).ThenBy(p => Int32.Parse(p.BoothCode))

                };
                return Ok(data);
            }
            else
            {
                return BadRequest("No Record Found");
            }


        }


        [HttpGet]
        [Route("GetBoothListByEventId")]
        [Authorize]
        public async Task<IActionResult> GetBoothListByEventId(string eventId)
        {
            var soIdClaim = User.Claims.FirstOrDefault(c => c.Type == "SoId");

            if (soIdClaim == null)
            {
                // Handle the case where the SoId claim is not present
                return BadRequest("SoId claim not found.");
            }

            var soId = soIdClaim.Value;
            var eventWiseBoothList = await _EAMSService.GetBoothListByEventId(eventId, soId);

            if (eventWiseBoothList != null)
            {
                var data = new
                {
                    count = eventWiseBoothList.Count,
                    data = eventWiseBoothList.OrderBy(p => Int32.Parse(p.BoothCode))
                };
                return Ok(data);
            }
            else
            {
                return BadRequest("No Record Found");
            }

        }

        [HttpGet]
        [Route("GetBoothEventActivityById")]
        [Authorize]
        public async Task<IActionResult> GetBoothEventActivityById()
        {
            var soIdClaim = User.Claims.FirstOrDefault(c => c.Type == "SoId");

            if (soIdClaim == null)
            {
                // Handle the case where the SoId claim is not present
                return BadRequest("SoId claim not found.");
            }

            var soId = soIdClaim.Value;
            var eventWiseBoothList = await _EAMSService.GetBoothEventActivityById(soId);

            if (eventWiseBoothList != null)
            {
                var data = new
                {
                    count = eventWiseBoothList.Count,
                    data = eventWiseBoothList.OrderBy(p => Int32.Parse(p.BoothCode))
                };
                return Ok(data);
            }
            else
            {
                return BadRequest("No Record Found");
            }

        }

        [HttpGet]
        [Route("GetBoothStatusByEventIdforARO")]
        [Authorize]
        public async Task<IActionResult> GetBoothStatusByEventIdforARO(string eventId, string soId)
        {

            var eventWiseBoothList = await _EAMSService.GetBoothListByEventId(eventId, soId);

            if (eventWiseBoothList != null)
            {
                var data = new
                {
                    count = eventWiseBoothList.Count,
                    data = eventWiseBoothList.OrderBy(p => Int32.Parse(p.BoothCode))
                };
                return Ok(data);
            }
            else
            {
                return BadRequest("No Record Found");
            }

        }


        [HttpGet]
        [Route("GetBoothStatusforARO")]
        [Authorize]
        public async Task<IActionResult> GetBoothStatusforARO(string assemblyMasterId, string boothMasterId)
        {

            var eventWiseBoothList = await _EAMSService.GetBoothStatusforARO(assemblyMasterId, boothMasterId);

            if (eventWiseBoothList != null)
            {
                var data = new
                {
                    count = eventWiseBoothList.Count,
                    data = eventWiseBoothList
                };
                return Ok(data);
            }
            else
            {
                return BadRequest("No Record Found");
            }

        }

        #endregion

        #region PC 
        [HttpGet]
        [Route("GetPCList")]
        [Authorize]
        public async Task<IActionResult> GetPCList(string stateMasterId)
        {
            var pcList = await _EAMSService.GetPCList(stateMasterId);
            var mappedData = _mapper.Map<List<PCViewModel>>(pcList);
            if (mappedData != null)
            {
                var pcData = new
                {
                    count = mappedData.Count,
                    data = mappedData
                };
                return Ok(pcData);
            }
            else
            {
                return BadRequest("No Record Found");
            }
        }

        [HttpPost]
        [Route("AddPC")]
        [Authorize]
        public async Task<IActionResult> AddPC(PCViewModel addPc)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<PCViewModel, ParliamentConstituencyMaster>(addPc);
                var electionType = User.Claims.FirstOrDefault(c => c.Type == "ElectionType").Value;
                if (electionType == "LS")
                {
                    mappedData.ElectionTypeId = 1;
                }
                else if (electionType == "VS")
                {
                    mappedData.ElectionTypeId = 2;
                }
                var result = await _EAMSService.AddPC(mappedData);

                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }
            }

            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }


        [HttpPut]
        [Route("UpdatePC")]
        [Authorize]
        public async Task<IActionResult> UpdatePC(PCViewModel pcViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<PCViewModel, ParliamentConstituencyMaster>(pcViewModel);
                var result = await _EAMSService.UpdatePC(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("GetPCById")]
        [Authorize]
        public async Task<IActionResult> GetPCById(string? pcMasterId)
        {
            if (pcMasterId != null)
            {
                var pcList = await _EAMSService.GetPCById(pcMasterId);
                var mappedData = _mapper.Map<PCViewModel>(pcList);
                if (mappedData != null)
                {

                    return Ok(mappedData);
                }
                else
                {
                    return NotFound("Data Not Found");
                }
            }
            else
            {
                return BadRequest(pcMasterId + "is null");
            }
        }

        [HttpGet]
        [Route("GetAssemblyByPCId")]
        [Authorize]
        public async Task<IActionResult> GetAssemblyByPCId(string stateMasterid, string PcMasterId)
        {
            var asembList = await _EAMSService.GetAssemblyByPCId(stateMasterid, PcMasterId);
            var mappedData = _mapper.Map<List<AssemblyMasterViewModel>>(asembList);
            if (mappedData != null)
            {
                var pcData = new
                {
                    count = mappedData.Count,
                    data = mappedData
                };
                return Ok(pcData);
            }
            else
            {
                return BadRequest("No Record Found");
            }
        }

        [HttpGet]
        [Route("GetAssemblyByDistrictId")]
        [Authorize]
        public async Task<IActionResult> GetAssemblyByDistrictId(string stateMasterid, string districtMasterId)
        {
            var asembList = await _EAMSService.GetAssemblyByDistrictId(stateMasterid, districtMasterId);
            var mappedData = _mapper.Map<List<AssemblyMasterViewModel>>(asembList);
            if (mappedData != null)
            {
                var pcData = new
                {
                    count = mappedData.Count,
                    data = mappedData
                };
                return Ok(pcData);
            }
            else
            {
                return BadRequest("No Record Found");
            }
        }
        #endregion

        #region FourthLevelH
        [HttpPost]
        [Route("AddFourthLevelH")]
        [Authorize]
        public async Task<IActionResult> AddFourthLevelH(AddFourthLevelHViewModel addFourthLevelHViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<AddFourthLevelHViewModel, FourthLevelH>(addFourthLevelHViewModel);


                var result = await _EAMSService.AddFourthLevelH(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet("GetFourthLevelHListById")]
        [Authorize]
        public async Task<IActionResult> GetFourthLevelHListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null)
            {
                var getList = await _EAMSService.GetFourthLevelHListById(stateMasterId, districtMasterId, assemblyMasterId);  // Corrected to await the asynchronous method
                var mappedData = _mapper.Map<List<FourthLevelH>, List<ListFourthLevelHViewModel>>(getList);
                if (getList != null)
                {
                    var data = new
                    {
                        count = mappedData.Count,
                        data = mappedData.ToList(),
                        //data = boothList.OrderBy(p => Int32.Parse(p.BoothCode_No)).ToList(),

                    };
                    return Ok(data);

                }
                else
                {
                    return NotFound("Booth Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }

        }

        [HttpPut]
        [Route("UpdateFourthLevelH")]
        [Authorize]
        public async Task<IActionResult> UpdateFourthLevelH(UpdateFourthLevelHViewModel updateFourthLevelHViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<UpdateFourthLevelHViewModel, FourthLevelH>(updateFourthLevelHViewModel);
                var result = await _EAMSService.UpdateFourthLevelH(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet("GetFourthLevelHById")]
        [Authorize]
        public async Task<IActionResult> GetFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null && fourthLevelHMasterId != null)
            {
                var getList = await _EAMSService.GetFourthLevelHById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);  // Corrected to await the asynchronous method
                var mappedData = _mapper.Map<FourthLevelH, ListFourthLevelHViewModel>(getList);
                if (mappedData != null)
                {

                    return Ok(mappedData);

                }
                else
                {
                    return NotFound();

                }
            }
            else
            {

                return BadRequest("Master Id's cannot be null");
            }

        }

        [HttpDelete("DeleteFourthLevelHById")]
        [Authorize]
        public async Task<IActionResult> DeleteFourthLevelHById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null && fourthLevelHMasterId != null)
            {
                var isSucceed = await _EAMSService.DeleteFourthLevelHById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);  // Corrected to await the asynchronous method
                if (isSucceed.IsSucceed)
                {
                    //_logger.LogInformation("Master status updated successfully.");
                    return Ok(isSucceed);
                }
                else
                {
                    // _logger.LogError($"Failed to update master status. Error: {isSucceed.Message}");
                    return BadRequest(isSucceed);
                }
            }
            else
            {

                return BadRequest(" Master Id's cannot be null");
            }

        }

        #endregion

        #region PSZonePanchayat
        [HttpPost]
        [Route("AddPSZonePanchayat")]
        [Authorize]
        public async Task<IActionResult> AddPSZonePanchayat(PSZonePanchayatViewModel addPSZonePanchayatViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<PSZonePanchayatViewModel, PSZonePanchayat>(addPSZonePanchayatViewModel);


                var result = await _EAMSService.AddPSZonePanchayat(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet("GetPSZonePanchayatListById")]
        [Authorize]
        public async Task<IActionResult> GetPSZonePanchayatListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null && fourthLevelHMasterId != null)
            {
                var getBlockPanchayatList = await _EAMSService.GetPSZonePanchayatListById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId);
                var mappeddata = _mapper.Map<List<PSZonePanchayat>, List<ListPSZonePanchayatViewModel>>(getBlockPanchayatList);
                if (mappeddata != null)
                {
                    var data = new
                    {
                        count = mappeddata.Count,
                        data = mappeddata.ToList(),


                    };
                    return Ok(data);

                }
                else
                {
                    return NotFound("Booth Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }

        }

        [HttpPut]
        [Route("UpdatePSZonePanchayat")]
        [Authorize]
        public async Task<IActionResult> UpdatePSZonePanchayat(UpdatePSZonePanchayatViewModel updatePSZonePanchayatViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<UpdatePSZonePanchayatViewModel, PSZonePanchayat>(updatePSZonePanchayatViewModel);
                var result = await _EAMSService.UpdatePSZonePanchayat(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet("GetPSZonePanchayatById")]
        [Authorize]
        public async Task<IActionResult> GetPSZonePanchayatById(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelHMasterId, int psZonePanchayatMasterId)
        {
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null && fourthLevelHMasterId != null && psZonePanchayatMasterId != null)
            {
                var getBlockPanchayat = await _EAMSService.GetPSZonePanchayatById(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelHMasterId, psZonePanchayatMasterId);  // Corrected to await the asynchronous method
                var mappeddata = _mapper.Map<PSZonePanchayat, ListPSZonePanchayatViewModel>(getBlockPanchayat);

                if (mappeddata != null)
                {

                    return Ok(mappeddata);

                }
                else
                {
                    return NotFound("Not Found");

                }
            }
            else
            {

                return BadRequest("Master Id's cannot be null");
            }

        }

        [HttpDelete("DeletePSZonePanchayatById")]
        [Authorize]
        public async Task<IActionResult> DeletePSZonePanchayatById(int psZonePanchayatMasterId)
        {
            if (psZonePanchayatMasterId != null)
            {
                var isSucceed = await _EAMSService.DeletePSZonePanchayatById(psZonePanchayatMasterId);  // Corrected to await the asynchronous method
                if (isSucceed.IsSucceed)
                {
                    //_logger.LogInformation("Master status updated successfully.");
                    return Ok(isSucceed);
                }
                else
                {
                    // _logger.LogError($"Failed to update master status. Error: {isSucceed.Message}");
                    return BadRequest(isSucceed);
                }
            }
            else
            {

                return BadRequest(" Master Id's cannot be null");
            }

        }

        #endregion

        #region  GPPanchayatWards 
        [HttpPost]
        [Route("AddGPPanchayatWards")]
        [Authorize]
        public async Task<IActionResult> AddSarpanchWards(AddGPPanchayatWardsViewModel addSarpanchWardsViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<AddGPPanchayatWardsViewModel, GPPanchayatWards>(addSarpanchWardsViewModel);


                var result = await _EAMSService.AddGPPanchayatWards(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet("GetGPPanchayatWardsListById")]
        [Authorize]
        public async Task<IActionResult> GetGPPanchayatWardsListById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId)
        {
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null && FourthLevelHMasterId != null)
            {
                var list = await _EAMSService.GetGPPanchayatWardsListById(stateMasterId, districtMasterId, assemblyMasterId, FourthLevelHMasterId);  // Corrected to await the asynchronous method

                var mappedData = _mapper.Map<List<GPPanchayatWards>, List<ListGPPanchayatWardsViewModel>>(list);
                if (list != null)
                {
                    var data = new
                    {
                        count = list.Count,
                        data = mappedData.ToList(),
                        //data = boothList.OrderBy(p => Int32.Parse(p.BoothCode_No)).ToList(),

                    };
                    return Ok(data);

                }
                else
                {
                    return NotFound("Booth Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }

        }

        [HttpPut]
        [Route("UpdateGPPanchayatWards")]
        [Authorize]
        public async Task<IActionResult> UpdateGPPanchayatWards(UpdateGPPanchayatWardsViewModel updateGPPanchayatWardsViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<UpdateGPPanchayatWardsViewModel, GPPanchayatWards>(updateGPPanchayatWardsViewModel);
                var result = await _EAMSService.UpdateGPPanchayatWards(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet("GetGPPanchayatWardsById")]
        [Authorize]
        public async Task<IActionResult> GetGPPanchayatWardsById(int stateMasterId, int districtMasterId, int assemblyMasterId, int FourthLevelHMasterId, int gpPanchayatWardsMasterId)
        {
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null && FourthLevelHMasterId != null && gpPanchayatWardsMasterId != null)
            {
                var wardsList = await _EAMSService.GetGPPanchayatWardsById(stateMasterId, districtMasterId, assemblyMasterId, FourthLevelHMasterId, gpPanchayatWardsMasterId);
                var mappeddata = _mapper.Map<GPPanchayatWards, ListGPPanchayatWardsViewModel>(wardsList);

                if (mappeddata != null)
                {

                    return Ok(mappeddata);

                }
                else
                {
                    return NotFound("Not Found");

                }
            }
            else
            {

                return BadRequest("Master Id's cannot be null");
            }

        }

        [HttpDelete("DeleteGPPanchayatWardsById")]
        [Authorize]
        public async Task<IActionResult> DeleteGPPanchayatWardsById(int gpPanchayatWardsMasterId)
        {
            if (gpPanchayatWardsMasterId != null)
            {

                var isDelete = await _EAMSService.DeleteGPPanchayatWardsById(gpPanchayatWardsMasterId);
                if (isDelete != null)
                {

                    return Ok(isDelete);

                }
                else
                {
                    return NotFound("Not Found");

                }
            }
            else
            {

                return BadRequest(" Master Id's cannot be null");
            }

        }

        #endregion

        #region Event Activity
        [HttpPut]
        [Route("UpdateEventActivity")]
        [Authorize]
        public async Task<IActionResult> UpdateEventActivity(UpdateEventActivityViewModel updateEventActivityViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state.");
            }

            // Extract the required claims
            if (!TryGetClaimValue(User, "StateMasterId", out int stateMasterId) ||
                !TryGetClaimValue(User, "DistrictMasterId", out int districtMasterId) ||
                !TryGetClaimValue(User, "AssemblyMasterId", out int assemblyMasterId) ||
                !TryGetClaimValue(User, "ElectionTypeMasterId", out int electionTypeMasterId))
            {
                return BadRequest("Missing or invalid claims.");
            }

            // Map view model to entity
            var mappedData = _mapper.Map<UpdateEventActivity>(updateEventActivityViewModel);

            // Set IDs from claims
            mappedData.StateMasterId = stateMasterId;
            mappedData.DistrictMasterId = districtMasterId;
            mappedData.AssemblyMasterId = assemblyMasterId;
            mappedData.ElectionTypeMasterId = electionTypeMasterId;

            var result = await _EAMSService.UpdateEventActivity(mappedData);
            if (result.IsSucceed == true)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }

        }
       
        [HttpPut]
        [Route("UpdateVTEventActivity")]
        [Authorize]
        public async Task<IActionResult> UpdateVTEventActivity(UpdateVTEventActivityViewModel updateEventActivityViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state.");
            }

            // Extract the required claims
            if (!TryGetClaimValue(User, "StateMasterId", out int stateMasterId) ||
                !TryGetClaimValue(User, "DistrictMasterId", out int districtMasterId) ||
                !TryGetClaimValue(User, "AssemblyMasterId", out int assemblyMasterId) ||
                !TryGetClaimValue(User, "ElectionTypeMasterId", out int electionTypeMasterId))
            {
                return BadRequest("Missing or invalid claims.");
            }

            // Map view model to entity
            var mappedData = _mapper.Map<UpdateEventActivity>(updateEventActivityViewModel);

            mappedData.StateMasterId = stateMasterId;
            mappedData.DistrictMasterId = districtMasterId;
            mappedData.AssemblyMasterId = assemblyMasterId;
            mappedData.ElectionTypeMasterId = electionTypeMasterId;

            var result = await _EAMSService.UpdateEventActivity(mappedData);
            if (result.IsSucceed == true)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }

        }

        [HttpPut]
        [Route("UpdateFinalVoteEventActivity")]
        [Authorize]
        public async Task<IActionResult> UpdateFinalVoteEventActivity(UpdateFinalVoteEventActivityViewModel updateEventActivityViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state.");
            }

            // Extract the required claims
            if (!TryGetClaimValue(User, "StateMasterId", out int stateMasterId) ||
                !TryGetClaimValue(User, "DistrictMasterId", out int districtMasterId) ||
                !TryGetClaimValue(User, "AssemblyMasterId", out int assemblyMasterId) ||
                !TryGetClaimValue(User, "ElectionTypeMasterId", out int electionTypeMasterId))
            {
                return BadRequest("Missing or invalid claims.");
            }

            // Map view model to entity
            var mappedData = _mapper.Map<UpdateEventActivity>(updateEventActivityViewModel);

            mappedData.StateMasterId = stateMasterId;
            mappedData.DistrictMasterId = districtMasterId;
            mappedData.AssemblyMasterId = assemblyMasterId;
            mappedData.ElectionTypeMasterId = electionTypeMasterId;

            var result = await _EAMSService.UpdateEventActivity(mappedData);
            if (result.IsSucceed == true)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }

        }

        private bool TryGetClaimValue(ClaimsPrincipal user, string claimType, out int result)
        {
            result = 0;
            var claimValue = user.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            return !string.IsNullOrEmpty(claimValue) && int.TryParse(claimValue, out result);
        }

        [HttpGet]
        [Route("GetBoothEventListById")]
        [Authorize]
        public async Task<IActionResult> GetBoothEventListById(int boothMasterId)
        {
            if (!TryGetClaimValue(User, "StateMasterId", out int stateMasterId) ||
                !TryGetClaimValue(User, "ElectionTypeMasterId", out int electionTypeMasterId))
            {
                return BadRequest("Missing or invalid claims.");
            }
            var result = await _EAMSService.GetBoothEventListById(stateMasterId, electionTypeMasterId, boothMasterId);
            return Ok(result);
        }

        #region All Events 
        [HttpGet]
        [Route("GetLastUpdatedPollDetail")]
        [Authorize]
        public async Task<IActionResult> GetLastUpdatedPollDetail(int boothMasterId)
        {
         
            var result = await _EAMSService.GetLastUpdatedPollDetail(boothMasterId);
            if (result is not null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest( );
            }

        }

        [HttpPost]
        [Route("AddVoterTurnOut")]
        [Authorize]
        public async Task<IActionResult> AddVoterTurnOut(AddVoterTurnOutViewModel addVoterTurnOut)
        {
            if (ModelState.IsValid)
            {
                //return BadRequest("Enter Voter Turn Out in Next Slot( 5 - 5:30). Thanks for your Support.");
                var addvoter = _mapper.Map<AddVoterTurnOut>(addVoterTurnOut);

                if (addVoterTurnOut.eventid == "6")
                {

                    if (Convert.ToInt32(addVoterTurnOut.voterValue) != null && Convert.ToInt32(addVoterTurnOut.voterValue) > 0)
                    {
                        var result = await _EAMSService.AddVoterTurnOut(addvoter);
                        switch (result.Status)
                        {
                            case RequestStatusEnum.OK:
                                return Ok(result.Message);
                            case RequestStatusEnum.BadRequest:
                                return BadRequest(result.Message);
                            case RequestStatusEnum.NotFound:
                                return NotFound(result.Message);

                            default:
                                return StatusCode(500, "Internal Server Error");
                        }

                    }
                    else
                    {
                        Response rt = new Response { Status = RequestStatusEnum.BadRequest, Message = "Voter Value cannot be null or Zero." };

                        return BadRequest(rt.Message);

                    }


                }

                else
                {
                    Response rt = new Response { Status = RequestStatusEnum.BadRequest, Message = "Event Id not valid" };

                    // RequestStatusEnum.BadRequest:
                    return BadRequest(rt.Message);
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }


        }


        [HttpGet]
        [Route("GetVoterInQueue")]
        [Authorize]
        public async Task<IActionResult> GetVoterInQueue(string boothMasterId)
        {

            var result = await _EAMSService.GetVoterInQueue(boothMasterId);
            if (result is not null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Route("GetFinalVotes")]
        [Authorize]
        public async Task<IActionResult> GetFinalVotes(int boothMasterId)
        {

            var result = await _EAMSService.GetFinalVotes(boothMasterId);
            if (result is not null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Final Vote is not active");
            }

        }
        #endregion

        #region Event Count for Dashboard
        [HttpGet]
        [Route("GetDistrictWiseEventListById")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,PC")]
        public async Task<IActionResult> EventListDistrictWiseById(string? stateId)
        {
            if (string.IsNullOrEmpty(stateId) || stateId.Contains("undefined"))
            {
                return BadRequest("Parameter miss match");
            }

            string stateMasterId = stateId ?? User.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            if (string.IsNullOrEmpty(stateMasterId))
            {
                return BadRequest("StateMasterId is required.");
            }

            var eventDistrictWiseList = await _EAMSService.GetEventListDistrictWiseById(stateMasterId);
            if (eventDistrictWiseList != null)
            {
                return Ok(eventDistrictWiseList);
            }
            else
            {
                return NotFound();
            }
        }



        [HttpGet]
        [Route("GetPCWiseEventListById")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,PC")]
        public async Task<IActionResult> EventListPCWiseById(string? stateId)
        {
            string stateMasterId;
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            var stateMasterIdC = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId").Value;
            if (stateId != null)
            {
                stateMasterId = stateId;
            }
            else
            {
                stateMasterId = stateMasterIdC.ToString();
            }
            var eventPCWiseList = await _EAMSService.GetEventListPCWiseById(stateMasterId, userId);
            if (eventPCWiseList is not null)
                return Ok(eventPCWiseList);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetAssemblyWiseEventListById")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,PC")]
        public async Task<IActionResult> EventListAssemblyWiseById(string? stateId, string? districtId)
        {
            var eventAssemblyList = await _EAMSService.GetEventListAssemblyWiseById(stateId, districtId);
            if (eventAssemblyList is not null)
                return Ok(eventAssemblyList);
            else
                return NotFound();
        }


        [HttpGet]
        [Route("GetAssemblyWiseEventListByPCId")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,PC")]
        public async Task<IActionResult> EventListAssemblyWiseByPCId(string? stateId, string? pcId)
        {
            var eventAssemblyList = await _EAMSService.GetEventListAssemblyWiseByPCId(stateId, pcId);
            if (eventAssemblyList is not null)
                return Ok(eventAssemblyList);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetBoothWiseEventListById")]
        [Authorize]
        public async Task<IActionResult> EventListBoothWiseById(string? stateId, string? districtId, string? assemblyId)
        {
            var eventBoothList = await _EAMSService.GetEventListBoothWiseById(stateId, districtId, assemblyId);
            if (eventBoothList is not null)
                return Ok(eventBoothList);
            else
                return NotFound();
        }



        [HttpGet]
        [Route("GetPCBoothWiseEventListById")]
        [Authorize]
        public async Task<IActionResult> EventListBoothWiseByPCId(string? stateId, string? pcId, string? assemblyId)
        {
            var eventBoothList = await _EAMSService.GetEventListBoothWiseByPCId(stateId, pcId, assemblyId);
            if (eventBoothList is not null)
                return Ok(eventBoothList);
            else
                return NotFound();
        }
        #endregion

        #endregion  

        #region Event Wise Booth Status
        [HttpGet]
        [Route("EventWiseBoothStatus")]
        [Authorize]
        public async Task<IActionResult> EventWiseBoothStatus()
        {
            //try
            //{
            var soIdClaim = User.Claims.FirstOrDefault(c => c.Type == "SoId");
            if (soIdClaim == null)
            {
                // Handle the case where the SoId claim is not present
                return BadRequest("SoId claim not found.");
            }

            var soId = soIdClaim.Value;
            var result = await _EAMSService.EventWiseBoothStatus(soId);

            return Ok(result);
            //}
            //catch(Exception a1)
            //{
            //    return BadRequest(a1.Message);
            //}
        }

        #endregion

        #region Event Slot Management
        [HttpPost]
        [Route("AddEventSlot")]
        [Authorize]
        public async Task<IActionResult> AddEventSlot(SlotManagementViewModel slotManagementViewModel)
        {
            if (ModelState.IsValid)
            {
                if (slotManagementViewModel?.slotTimes != null && slotManagementViewModel.slotTimes.Any())
                {
                    // Set the isLastSlot property for the last slot to true
                    var lastSlot = slotManagementViewModel.slotTimes.Last();
                    lastSlot.IsLastSlot = true;
                }

                var slotManagements = _mapper.Map<List<SlotManagementMaster>>(slotManagementViewModel);

                var result = await _EAMSService.AddEventSlot(slotManagements);

                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }

            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("GetEventSlotListById")]
        [Authorize]
        public async Task<IActionResult> GetEventSlotList(int stateMasterId, int electionTypeMasterId, int EventId)
        {
            var result = await _EAMSService.GetEventSlotList(stateMasterId, electionTypeMasterId, EventId);
            if (result is null)
            {
                return BadRequest(result);
            }
            return Ok(result);



        }
        #endregion

        #region UserList
        [HttpGet]
        [Route("GetUserList")]
        [Authorize]
        public async Task<IActionResult> GetUserList(string userName, string type)
        {
            var userList = await _EAMSService.GetUserList(userName, type);
            var data = new
            {
                count = userList.Count,
                data = userList
            };
            return Ok(data);

        }
        #endregion

        #region PollInterruption

        [HttpPost]
        [Route("AddPollInterruption")]
        [Authorize]
        public async Task<IActionResult> AddPollInterruption(InterruptionViewModel interruptionViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<PollInterruption>(interruptionViewModel);
                var result = await _EAMSService.AddPollInterruption(mappedData);

                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }

            }

            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }


        [HttpGet]
        [Route("GetPollInterruptionbyId")]
        [Authorize]
        public async Task<IActionResult> GetPollInterruptionbyId(string boothMasterId)
        {
            PollInterruption? data = await _EAMSService.GetPollInterruptionbyId(boothMasterId);
            PollInterruption pollInterruptionData = new PollInterruption();
            if (data == null)
            {

                pollInterruptionData.BoothMasterId = Convert.ToInt32(boothMasterId);
                pollInterruptionData.Flag = "Fresh";
                pollInterruptionData.IsPollInterrupted = false;

            }
            else
            {
                pollInterruptionData = data;

            }

            return Ok(pollInterruptionData);

        }


        [HttpGet]
        [Route("GetPollInterruptionHistoryById")]
        [Authorize]
        public async Task<IActionResult> GetPollInterruptionHistoryById(string boothMasterId)
        {
            var data = await _EAMSService.GetPollInterruptionHistoryById(boothMasterId);
            if (data.Count > 0)
            {
                return Ok(data);
            }
            else
            {
                return BadRequest("No Data Found !");
            }


        }

        [HttpGet]
        [Route("GetPollInterruptionDashboard")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,ARO,SubARO,PC")]
        public async Task<IActionResult> GetPollInterruptionDashboard()
        {

            ClaimsIdentity newClaimsIdentity = new ClaimsIdentity(User.Identity);
            var dataRecord = await _EAMSService.GetPollInterruptionDashboard(newClaimsIdentity);
            var filteredPending = dataRecord.Where(p => p.isPollInterrupted == true).ToList();
            var filteredResolved = dataRecord.Where(p => p.isPollInterrupted == false).ToList();
            if (dataRecord != null)
            {
                var data = new
                {
                    TotalInterruptions = dataRecord.Count,
                    Pending = filteredPending.Count,
                    Resolved = filteredResolved.Count,
                    PendingList = filteredPending,
                    ResolvedList = filteredResolved,
                    All = dataRecord
                };
                return Ok(data);

            }
            else
            {
                return NotFound("No Interruptions");

            }

        }

        [HttpGet]
        [Route("GetPollInterruptionforSO")]
        [Authorize(Roles = "SO")]
        public async Task<IActionResult> GetBoothListBySoIdfoInterruption()
        {

            ClaimsIdentity newClaimsIdentity = new ClaimsIdentity(User.Identity);
            var dataRecord = await _EAMSService.GetBoothListBySoIdfoInterruption(newClaimsIdentity);
            var IspollInterruptedList = dataRecord.Where(p => p.isPollInterrupted == true).ToList();
            //var filteredResolved = dataRecord.Where(p => p.isPollInterrupted == false).ToList();
            if (dataRecord != null)
            {
                var data = new
                {
                    count = IspollInterruptedList.Count,
                    Pending = IspollInterruptedList,

                };
                return Ok(data);

            }
            else
            {
                return NotFound("No Interruptions");

            }

        }
        #endregion

        #region PSO Form

        [HttpPost]
        [Route("AddPSOForm")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> AddPSOForm(PollingStationViewModel pollingStationViewModel)
        {
            if (ModelState.IsValid)
            {
                ClaimsIdentity newClaimsIdentity = new ClaimsIdentity(User.Identity);
                var mappedData = _mapper.Map<PollingStationMaster>(pollingStationViewModel);
                try
                {
                    if (pollingStationViewModel.StateCode != null && pollingStationViewModel.StateName != null && pollingStationViewModel.DistrictCode != null && pollingStationViewModel.DistrictName != null)
                    {
                        var distrctRecord = await _EAMSService.GetDistrictRecordById(pollingStationViewModel.DistrictMasterId.ToString());
                        if (distrctRecord != null)
                        {
                            if (distrctRecord.DistrictMasterId == pollingStationViewModel.DistrictMasterId && distrctRecord.StateMasterId == pollingStationViewModel.StateMasterId)
                            {
                                var boothRecord = await _EAMSService.GetBoothById(pollingStationViewModel.BoothMasterId.ToString());
                                if (boothRecord != null)
                                {

                                    if (boothRecord.BoothMasterId == pollingStationViewModel.BoothMasterId && boothRecord.AssemblyMasterId == pollingStationViewModel.AssemblyMasterId && boothRecord.StateMasterId == pollingStationViewModel.StateMasterId && boothRecord.DistrictMasterId == pollingStationViewModel.DistrictMasterId)
                                    {
                                        // check already entered in database
                                        if (pollingStationViewModel.PCasterId != null)
                                        {
                                            var getPcRecord = await _EAMSService.GetPCById(pollingStationViewModel.PCasterId.ToString());
                                            if (getPcRecord != null)
                                            {
                                                if (getPcRecord.StateMasterId == pollingStationViewModel.StateMasterId && getPcRecord.StateMasterId == boothRecord.StateMasterId)
                                                {
                                                    // get assembly 
                                                    var assemblyRecord = await _EAMSService.GetAssemblyById(pollingStationViewModel.AssemblyMasterId.ToString());
                                                    if (assemblyRecord != null)
                                                    {
                                                        var assemblyRecordbyPC = await _EAMSService.GetAssemblyByPCId(pollingStationViewModel.StateMasterId.ToString(), pollingStationViewModel.PCasterId.ToString());

                                                        if (assemblyRecordbyPC != null)
                                                        {
                                                            bool exists = assemblyRecordbyPC.Any(assembly => assembly.AssemblyMasterId.ToString() == pollingStationViewModel.AssemblyMasterId.ToString() && assembly.AssemblyMasterId == boothRecord.AssemblyMasterId && assembly.AssemblyMasterId == assemblyRecord.AssemblyMasterId);

                                                            if (exists)
                                                            {
                                                                // The value exists in the list
                                                                var dataRecord = await _EAMSService.PollingStationRecord(Convert.ToInt32(pollingStationViewModel.BoothMasterId));
                                                                var pollingStationMaster = _mapper.Map<PollingStationMaster>(pollingStationViewModel);
                                                                if (dataRecord == true)
                                                                {
                                                                    var pollingStationGenderList = _mapper.Map<List<PollingStationGender>>(pollingStationViewModel.PollingStationGenderViewModel);
                                                                    pollingStationMaster.PollingStationGender = pollingStationGenderList;
                                                                    var enumValues = Enum.GetValues(typeof(PSFormEnum)).Cast<PSFormEnum>();
                                                                    var missingEnumValues = enumValues.Where(enumValue => pollingStationViewModel.PollingStationGenderViewModel.All(g => g.Type != (int)enumValue)).ToList();

                                                                    if (missingEnumValues.Any())
                                                                    {
                                                                        // Construct a message with missing enum values
                                                                        var missingValuesMessage = $"The following enum values are missing in polling Station Gender Section: {string.Join(", ", missingEnumValues)}";

                                                                        // Return the message indicating missing enum values
                                                                        return BadRequest(missingValuesMessage);
                                                                    }


                                                                    pollingStationMaster.PollingStationGender = pollingStationGenderList;


                                                                    var result = await _EAMSService.AddPSOForm(pollingStationMaster, newClaimsIdentity);


                                                                    switch (result.Status)
                                                                    {
                                                                        case RequestStatusEnum.OK:
                                                                            return Ok(result.Message);
                                                                        case RequestStatusEnum.BadRequest:
                                                                            return BadRequest(result.Message);
                                                                        case RequestStatusEnum.NotFound:
                                                                            return NotFound(result.Message);

                                                                        default:
                                                                            return StatusCode(500, "Internal Server Error");
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    return BadRequest("Booth Record Already Entered");
                                                                }

                                                            }
                                                            else
                                                            {
                                                                return BadRequest("Assembly Records does not Exists in the specified State & PC, Kindly check exact State,PC and Assembly.");

                                                            }
                                                        }
                                                        else
                                                        {
                                                            return BadRequest("Assembly does not Exists in the specified State & PC, Kindly check exact State,PC and Assembly.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return BadRequest("Assembly does not Exist.");
                                                    }
                                                }
                                                else
                                                {
                                                    return BadRequest("PC Record does not Exist in State you have entered.");

                                                }
                                            }
                                            else
                                            {
                                                return BadRequest("PC Record does not Exists.");
                                            }
                                        }
                                        else
                                        {
                                            return BadRequest("Please Pass PC Master Id.");
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest("Please check combination of State, District, Assembly, Booth Values, Currently they ate Invalid");
                                    }
                                }
                                else
                                {
                                    return BadRequest("Booth does not Exists.");

                                }
                            }
                            else
                            {
                                return BadRequest("District does not exist in the State");
                            }
                        }
                        else
                        {
                            return BadRequest("District Record Not Found");
                        }

                    }

                    else
                    {
                        return BadRequest("State/District Name & Code cant be null");
                    }

                }
                catch (DbUpdateException)
                {
                    // Handle exception appropriately
                    return StatusCode(500, "Internal Server Error");
                }
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());

            }


        }

        [HttpPut]
        [Route("UpdatePSOFormbyPSId")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> UpdatePSOFormbyPSId(PollingStationViewModel pollingStationViewModel)
        {
            if (ModelState.IsValid)
            {
                ClaimsIdentity newClaimsIdentity = new ClaimsIdentity(User.Identity);
                var mappedData = _mapper.Map<PollingStationMaster>(pollingStationViewModel);
                try
                {
                    if (pollingStationViewModel.BoothMasterId > 0)
                    {
                        if (pollingStationViewModel.PollingStationMasterId > 0)
                        {

                            var isBoothFreezed = await _EAMSService.GetPollingStationRecordById(pollingStationViewModel.PollingStationMasterId);
                            if (isBoothFreezed == false)
                            {
                                if (pollingStationViewModel.StateCode != null && pollingStationViewModel.StateName != null && pollingStationViewModel.DistrictCode != null && pollingStationViewModel.DistrictName != null)
                                {
                                    var distrctRecord = await _EAMSService.GetDistrictRecordById(pollingStationViewModel.DistrictMasterId.ToString());
                                    if (distrctRecord != null)
                                    {
                                        if (distrctRecord.DistrictMasterId == pollingStationViewModel.DistrictMasterId && distrctRecord.StateMasterId == pollingStationViewModel.StateMasterId)
                                        {
                                            var boothRecord = await _EAMSService.GetBoothById(pollingStationViewModel.BoothMasterId.ToString());
                                            if (boothRecord != null)
                                            {

                                                if (boothRecord.BoothMasterId == pollingStationViewModel.BoothMasterId && boothRecord.AssemblyMasterId == pollingStationViewModel.AssemblyMasterId && boothRecord.StateMasterId == pollingStationViewModel.StateMasterId && boothRecord.DistrictMasterId == pollingStationViewModel.DistrictMasterId)
                                                {
                                                    // check already entered in database
                                                    if (pollingStationViewModel.PCasterId != null)
                                                    {
                                                        var getPcRecord = await _EAMSService.GetPCById(pollingStationViewModel.PCasterId.ToString());
                                                        if (getPcRecord != null)
                                                        {
                                                            if (getPcRecord.StateMasterId == pollingStationViewModel.StateMasterId && getPcRecord.StateMasterId == boothRecord.StateMasterId)
                                                            {
                                                                // get assembly 
                                                                var assemblyRecord = await _EAMSService.GetAssemblyById(pollingStationViewModel.AssemblyMasterId.ToString());
                                                                if (assemblyRecord != null)
                                                                {
                                                                    var assemblyRecordbyPC = await _EAMSService.GetAssemblyByPCId(pollingStationViewModel.StateMasterId.ToString(), pollingStationViewModel.PCasterId.ToString());

                                                                    if (assemblyRecordbyPC != null)
                                                                    {
                                                                        bool exists = assemblyRecordbyPC.Any(assembly => assembly.AssemblyMasterId.ToString() == pollingStationViewModel.AssemblyMasterId.ToString() && assembly.AssemblyMasterId == boothRecord.AssemblyMasterId && assembly.AssemblyMasterId == assemblyRecord.AssemblyMasterId);

                                                                        if (exists)
                                                                        {
                                                                            // The value exists in the list
                                                                            //var dataRecord = await _EAMSService.PollingStationRecord(Convert.ToInt32(pollingStationViewModel.BoothMasterId));
                                                                            var pollingStationMaster = _mapper.Map<PollingStationMaster>(pollingStationViewModel);
                                                                            //if (dataRecord == true)
                                                                            //{
                                                                            var pollingStationGenderList = _mapper.Map<List<PollingStationGender>>(pollingStationViewModel.PollingStationGenderViewModel);
                                                                            pollingStationMaster.PollingStationGender = pollingStationGenderList;
                                                                            var enumValues = Enum.GetValues(typeof(PSFormEnum)).Cast<PSFormEnum>();
                                                                            var missingEnumValues = enumValues.Where(enumValue => pollingStationViewModel.PollingStationGenderViewModel.All(g => g.Type != (int)enumValue)).ToList();

                                                                            if (missingEnumValues.Any())
                                                                            {
                                                                                // Construct a message with missing enum values
                                                                                var missingValuesMessage = $"The following enum values are missing in polling Station Gender Section: {string.Join(", ", missingEnumValues)}";

                                                                                // Return the message indicating missing enum values
                                                                                return BadRequest(missingValuesMessage);
                                                                            }


                                                                            pollingStationMaster.PollingStationGender = pollingStationGenderList;


                                                                            var result = await _EAMSService.UpdatePSoForm(pollingStationMaster, newClaimsIdentity);


                                                                            switch (result.Status)
                                                                            {
                                                                                case RequestStatusEnum.OK:
                                                                                    return Ok(result.Message);
                                                                                case RequestStatusEnum.BadRequest:
                                                                                    return BadRequest(result.Message);
                                                                                case RequestStatusEnum.NotFound:
                                                                                    return NotFound(result.Message);

                                                                                default:
                                                                                    return StatusCode(500, "Internal Server Error");
                                                                            }

                                                                            //}
                                                                            //else
                                                                            //{
                                                                            //    return BadRequest("Booth Record Already Entered");
                                                                            //}

                                                                        }
                                                                        else
                                                                        {
                                                                            return BadRequest("Assembly Records does not Exists in the specified State & PC, Kindly check exact State,PC and Assembly.");

                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        return BadRequest("Assembly does not Exists in the specified State & PC, Kindly check exact State,PC and Assembly.");
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    return BadRequest("Assembly does not Exist.");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                return BadRequest("PC Record does not Exist in State you have entered.");

                                                            }
                                                        }
                                                        else
                                                        {
                                                            return BadRequest("PC Record does not Exists.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return BadRequest("Please Pass PC Master Id.");
                                                    }
                                                }
                                                else
                                                {
                                                    return BadRequest("Please check combination of State, District, Assembly, Booth Values, Currently they ate Invalid");
                                                }
                                            }
                                            else
                                            {
                                                return BadRequest("Booth does not Exists.");

                                            }
                                        }
                                        else
                                        {
                                            return BadRequest("District does not exist in the State");
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest("District Record Not Found");
                                    }

                                }

                                else
                                {
                                    return BadRequest("State/District Name & Code cant be null");
                                }
                            }
                            else
                            {
                                return BadRequest("Booth Record Freezed Already, Can't Change Status");
                            }
                        }
                        else
                        {
                            return BadRequest("Polling Station Id not Valid");
                        }
                    }
                    else
                    {
                        return BadRequest("Booth Id not found!");

                    }
                }
                catch (DbUpdateException)
                {
                    // Handle exception appropriately
                    return StatusCode(500, "Internal Server Error");
                }
            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());

            }


        }

        [HttpGet]
        [Route("GetPsoFormDetail")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> GetPsoFormDetail()
        {

            Claim stateMaster = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
            Claim districtMaster = User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
            Claim assemblyMaster = User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
            string stateMasterId = stateMaster.Value;
            string districtMasterId = districtMaster.Value;
            string assemblyMasterId = assemblyMaster.Value;
            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null)
            {
                var boothList = await _EAMSService.GetPsoFormDetail(stateMasterId, districtMasterId, assemblyMasterId);
                if (boothList != null)
                {
                    var data = new
                    {
                        count = boothList.Count,
                        data = boothList.OrderBy(p => Int32.Parse(p.AssemblyCode)),

                    };
                    return Ok(data);

                }
                else
                {
                    return NotFound("Data Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }


        }

        [HttpGet]
        [Route("GetPsoFormListforARO")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> GetPsoFormListforARO()
        {
            Claim stateMaster = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
            Claim districtMaster = User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
            Claim assemblyMaster = User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
            string stateMasterId = stateMaster.Value;
            string districtMasterId = districtMaster.Value;
            string assemblyMasterId = assemblyMaster.Value;

            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null)
            {
                var boothList = await _EAMSService.GetPSOlistbyARO(stateMasterId, districtMasterId, assemblyMasterId);
                if (boothList != null)
                {
                    var data = new
                    {
                        count = boothList.Count,
                        data = boothList.OrderBy(p => Int32.Parse(p.BoothCodeNo)),

                    };
                    return Ok(data);

                }
                else
                {
                    return NotFound("Data Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }
        }


        [HttpGet]
        [Route("GetPSFormBoothRecordbyId")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> GetPSFormBoothRecordbyId(string pollingStationMasterId)
        {
            Claim stateMaster = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
            Claim districtMaster = User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
            Claim assemblyMaster = User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
            string stateMasterId = stateMaster.Value;
            string districtMasterId = districtMaster.Value;
            string assemblyMasterId = assemblyMaster.Value;

            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null)
            {
                var boothRecordPS = await _EAMSService.GetPSFormRecordbyPSId(pollingStationMasterId);
                if (boothRecordPS != null)
                {

                    return Ok(boothRecordPS);

                }
                else
                {
                    return NotFound("Data Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }
        }

        #endregion

        #region Location Master
        [HttpGet]
        [Route("GetLocationMasterforARO")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> GetLocationMasterforARO()
        {
            Claim stateMaster = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
            Claim districtMaster = User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
            Claim assemblyMaster = User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
            string stateMasterId = stateMaster.Value;
            string districtMasterId = districtMaster.Value;
            string assemblyMasterId = assemblyMaster.Value;


            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null)
            {
                //return Ok(lc);
                var locRecords = await _EAMSService.GetLocationMasterforARO(stateMasterId, districtMasterId, assemblyMasterId);
                if (locRecords != null)
                {

                    return Ok(locRecords);

                }
                else
                {
                    return NotFound("Data Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }
        }


        [HttpGet]
        [Route("GetLocationMasterById")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> GetLocationMasterById(string locationMasterId)
        {
            Claim stateMaster = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId");
            Claim districtMaster = User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId");
            Claim assemblyMaster = User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId");
            string stateMasterId = stateMaster.Value;
            string districtMasterId = districtMaster.Value;
            string assemblyMasterId = assemblyMaster.Value;


            if (stateMasterId != null && districtMasterId != null && assemblyMasterId != null)
            {
                //return Ok(lc);
                var locRecords = await _EAMSService.GetLocationMasterById(locationMasterId);
                if (locRecords != null)
                {

                    return Ok(locRecords);

                }
                else
                {
                    return NotFound("Data Not Found");

                }
            }
            else
            {

                return BadRequest("State, District and Assembly Master Id's cannot be null");
            }
        }

        [HttpPost]
        [Route("AddLocation")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> AddLocation(LocationViewModel locationViewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {// check state,district,assembly active?


                    // check booths filled in assembly
                    var asemRecor = await _EAMSService.GetAssemblyById(locationViewModel.AssemblyMasterId.ToString());
                    //var boothsEntered = await _EAMSService.GetBoothListById(BoothMasterViewModel.StateMasterId.ToString(), BoothMasterViewModel.DistrictMasterId.ToString(), BoothMasterViewModel.AssemblyMasterId.ToString());
                    if (asemRecor != null)
                    {
                        locationViewModel.PCMasterId = asemRecor.PCMasterId;
                        var mappedData = _mapper.Map<LocationViewModel, LocationModel>(locationViewModel);
                        var result = await _EAMSService.AddLocation(mappedData);
                        switch (result.Status)
                        {
                            case RequestStatusEnum.OK:
                                return Ok(result.Message);
                            case RequestStatusEnum.BadRequest:
                                return BadRequest(result.Message);
                            case RequestStatusEnum.NotFound:
                                return NotFound(result.Message);

                            default:
                                return StatusCode(500, "Internal Server Error");
                        }
                    }
                    else
                    {
                        return BadRequest("Assembly Record Not Found");

                    }
                }
                else
                {

                    return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
                }

            }

            catch (Exception ex)
            {
                _logger.LogError($"AddBooth: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Route("UpdateLocation")]
        [Authorize(Roles = "ARO")]
        public async Task<IActionResult> UpdateLocation(LocationViewModel locationViewModel)
        {
            if (ModelState.IsValid)
            {
                if (locationViewModel.BoothMasterId.Count > 0)
                {
                    var mappedData = _mapper.Map<LocationViewModel, LocationModel>(locationViewModel);
                    var result = await _EAMSService.UpdateLocation(mappedData);
                    switch (result.Status)
                    {
                        case RequestStatusEnum.OK:
                            return Ok(result.Message);
                        case RequestStatusEnum.BadRequest:
                            return BadRequest(result.Message);
                        case RequestStatusEnum.NotFound:
                            return NotFound(result.Message);

                        default:
                            return StatusCode(500, "Internal Server Error");
                    }
                }
                else
                {
                    return StatusCode(500, "Select Booths !");
                }

            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        #endregion

        #region HelpDesk 
        [HttpPost]
        [Route("AddHelpDeskInfo")]
        [Authorize]
        public async Task<IActionResult> AddHelpDeskContact(HelpDeskViewModel helpDeskViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<HelpDeskViewModel, HelpDeskDetail>(helpDeskViewModel);

                if (helpDeskViewModel.StateMasterId.ToString() != null && helpDeskViewModel.DistrictMasterId.ToString() != null && helpDeskViewModel.AssemblyMasterId.ToString() != null)
                {
                    var locRecords = await _EAMSService.AddHelpDeskInfo(mappedData);
                    if (locRecords != null)
                    {

                        return Ok(locRecords);

                    }
                    else
                    {
                        return NotFound("Data Not Found");

                    }
                }
                else
                {
                    return BadRequest("State, District and Assembly Master Id's are not valid");

                }

                /*var electionType = User.Claims.FirstOrDefault(c => c.Type == "ElectionType").Value;
                if (electionType == "LS")
                {
                    mappedData.ElectionTypeId = 1;
                }
                else if (electionType == "VS")
                {
                    mappedData.ElectionTypeId = 2;
                }
                var result = await _EAMSService.AddPC(mappedData);

                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }*/
            }

            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }
        [HttpGet]
        [Route("GetHelpDesk")]
        [Authorize]
        public async Task<IActionResult> GetHelpDeskById(string assemblyMasterId)
        {


            var locRecords = await _EAMSService.GetHelpDeskDetail(assemblyMasterId);
            if (locRecords != null)
            {

                return Ok(locRecords);

            }
            else
            {
                return NotFound("Data Not Found");

            }

        }

        //[HttpPut]
        //[Route("UpdatePC")]
        //[Authorize]
        //public async Task<IActionResult> UpdatePC(PCViewModel pcViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var mappedData = _mapper.Map<PCViewModel, ParliamentConstituencyMaster>(pcViewModel);
        //        var result = await _EAMSService.UpdatePC(mappedData);
        //        switch (result.Status)
        //        {
        //            case RequestStatusEnum.OK:
        //                return Ok(result.Message);
        //            case RequestStatusEnum.BadRequest:
        //                return BadRequest(result.Message);
        //            case RequestStatusEnum.NotFound:
        //                return NotFound(result.Message);

        //            default:
        //                return StatusCode(500, "Internal Server Error");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
        //    }
        //}

        //[HttpGet]
        //[Route("GetPCById")]
        //[Authorize]
        //public async Task<IActionResult> GetPCById(string? pcMasterId)
        //{
        //    if (pcMasterId != null)
        //    {
        //        var pcList = await _EAMSService.GetPCById(pcMasterId);
        //        var mappedData = _mapper.Map<PCViewModel>(pcList);
        //        if (mappedData != null)
        //        {

        //            return Ok(mappedData);
        //        }
        //        else
        //        {
        //            return NotFound("Data Not Found");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest(pcMasterId + "is null");
        //    }
        //}

        #endregion

        #region GetDashBoardCount
        [HttpGet]
        [Route("GetDashBoardCount")]
        [Authorize]
        public async Task<IActionResult> GetDashBoardCount()
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;

            var getDashboardRecord = await _EAMSService.GetDashBoardCount(claimsIdentity);
            return Ok(getDashboardRecord);
        }
        #endregion

        #region QIS

        [HttpPost]
        [Route("AddQIS")]
        [Authorize]
        public async Task<IActionResult> AddQIS(QISViewModel addQISViewModel)
        {
            if (ModelState.IsValid)
            {
                // Retrieve state, assembly, and district IDs from the token claims
                var stateMasterIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
                var assemblyMasterIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AssemblyMasterId")?.Value;
                var districtMasterIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "DistrictMasterId")?.Value;
                var bloMasterIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BLOMasterId")?.Value;
                var bloMobileNumberClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone")?.Value;

                // Ensure all required claims are present
                if (stateMasterIdClaim != null && assemblyMasterIdClaim != null && districtMasterIdClaim != null && bloMasterIdClaim != null && bloMobileNumberClaim != null)
                {
                    // Convert claim values to appropriate types
                    if (int.TryParse(stateMasterIdClaim, out int stateMasterId) &&
                        int.TryParse(assemblyMasterIdClaim, out int assemblyMasterId) &&
                        int.TryParse(districtMasterIdClaim, out int districtMasterId) &&
                        int.TryParse(bloMasterIdClaim, out int soMasterId))
                    {
                        // Assign values to the view model
                        addQISViewModel.StateMasterId = stateMasterId;
                        addQISViewModel.AssemblyMasterId = assemblyMasterId;
                        addQISViewModel.DistrictMasterId = districtMasterId;
                        addQISViewModel.BLOMasterId = soMasterId;
                        addQISViewModel.BLOMobileNumber = bloMobileNumberClaim;

                        // Check if QueueEnterTime is not greater than current time
                        if (addQISViewModel.QueueEnterDateTime > DateTime.Now)
                        {

                            return BadRequest("Queue enter time cannot be greater than current time or Equal to current time.");
                        }

                        // Proceed with mapping and other operations
                        var mappedData = _mapper.Map<QIS>(addQISViewModel);
                        var result = await _EAMSService.AddQueue(mappedData);
                        if (result.IsSucceed == true)
                        {
                            return Ok(result);
                        }
                        else
                        {
                            return BadRequest(result);
                        }
                    }
                    else
                    {
                        return BadRequest("Invalid claim value types.");
                    }
                }
                else
                {
                    return BadRequest("One or more required claims are missing.");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route("AddQISForNonBLO")]
        public async Task<IActionResult> AddQISForNonBLO(int stateMasterId, int districtMasterId, int assemblyMasterId, int QueueCount, int BoothMasterId, string BoothCode, string bloMobileNumber)
        {
            if (ModelState.IsValid)
            {
                QISViewModel qISViewModel = new QISViewModel();
                // Assign values to the view model
                qISViewModel.StateMasterId = stateMasterId;
                qISViewModel.AssemblyMasterId = assemblyMasterId;
                qISViewModel.DistrictMasterId = districtMasterId;
                qISViewModel.BoothMasterId = BoothMasterId;
                qISViewModel.BoothCode = BoothCode;
                qISViewModel.QueueCount = QueueCount;
                qISViewModel.BLOMasterId = 0;
                qISViewModel.BLOMobileNumber = bloMobileNumber;

                // Check if QueueEnterTime is not greater than current time
                if (qISViewModel.QueueEnterDateTime > DateTime.Now)
                {

                    return BadRequest("Queue enter time cannot be greater than current time or Equal to current time.");
                }

                // Proceed with mapping and other operations
                var mappedData = _mapper.Map<QIS>(qISViewModel);
                var result = await _EAMSService.AddQueue(mappedData);
                if (result.IsSucceed == true)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }


            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        [HttpGet]
        [Route("GetQISList")]
        public async Task<IActionResult> GetQISList(string stateMasterId, string districtMasterId, string assemblyMasterId, string boothMasterId)
        {
            var result = await _EAMSService.GetQISList(stateMasterId, districtMasterId, assemblyMasterId, boothMasterId);
            if (result is not null)
            {
                return Ok(result);

            }
            else
            {
                return NotFound();
            }
        }

        #endregion

        #region BLOMaster
        [HttpGet]
        [Route("GetBLOBoothById")]
        [Authorize]
        public async Task<IActionResult> GetBLOBoothById()
        {
            var bloMasterIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BLOMasterId")?.Value;
            var record = await _EAMSService.GetBLOBoothById(bloMasterIdClaim);
            if (record is not null)
            {
                return Ok(record);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetBLOById")]
        [Authorize]
        public async Task<IActionResult> GetbloById(string bloMasterId)
        {
            var soRecord = await _EAMSService.GetBLObyId(bloMasterId);
            if (soRecord != null)
            {



                return Ok(soRecord);
            }
            else
            {
                return NotFound($"[{bloMasterId}] not exist");
            }

        }


        [HttpPost]
        [Route("AddBLOUser")]
        public async Task<IActionResult> AddBLOUser(AddBLOViewModel addBLOViewModel)
        {
            if (ModelState.IsValid)
            {

                var mappedData = _mapper.Map<BLOMaster>(addBLOViewModel);

                var result = await _EAMSService.AddBLOOfficer(mappedData);

                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }


            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpPut]
        [Route("UpdateBLOUser")]
        public async Task<IActionResult> UpdateBLOUser(UpdateBLOViewModel updateBLOViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<BLOMaster>(updateBLOViewModel);

                var result = await _EAMSService.UpdateBLOOfficer(mappedData);
                switch (result.Status)
                {
                    case RequestStatusEnum.OK:
                        return Ok(result.Message);
                    case RequestStatusEnum.BadRequest:
                        return BadRequest(result.Message);
                    case RequestStatusEnum.NotFound:
                        return NotFound(result.Message);

                    default:
                        return StatusCode(500, "Internal Server Error");
                }



            }
            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }

        [HttpGet]
        [Route("GetBLOListById")]
        [Authorize]
        public async Task<IActionResult> BLOOfficersListById(string stateMasterId, string districtMasterId, string assemblyMasterId)
        {
            var soList = await _EAMSService.GetBlosListById(stateMasterId, districtMasterId, assemblyMasterId);  // Corrected to await the asynchronous method
            if (soList != null)
            {
                var data = new
                {
                    count = soList.Count,
                    data = soList
                };
                return Ok(data);
            }
            else
            {
                return BadRequest("No Record Found");
            }

        }

        [HttpPost]
        [Route("BLOBoothMapping")]
        [Authorize]
        public async Task<IActionResult> BLOBoothMapping(BLOBoothViewModel boothMappingViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (boothMappingViewModel.BoothMasterId != null && boothMappingViewModel.BoothMasterId.Any() && boothMappingViewModel.IsAssigned == true && !string.IsNullOrWhiteSpace(boothMappingViewModel.AssignedTo))
                    {

                        List<BoothMaster> boothMasters = new List<BoothMaster>();

                        foreach (var boothMasterId in boothMappingViewModel.BoothMasterId)
                        {
                            var boothMaster = new BoothMaster
                            {
                                BoothMasterId = boothMasterId,
                                StateMasterId = boothMappingViewModel.StateMasterId,
                                DistrictMasterId = boothMappingViewModel.DistrictMasterId,
                                AssemblyMasterId = boothMappingViewModel.AssemblyMasterId,
                                AssignedToBLO = boothMappingViewModel.AssignedTo,
                                IsAssigned = boothMappingViewModel.IsAssigned,
                            };

                            boothMasters.Add(boothMaster);
                        }

                        // Assuming _EAMSService.BoothMapping is an asynchronous method returning Task<Response>
                        var result = await _EAMSService.BLOBoothMapping(boothMasters);
                        switch (result.Status)
                        {
                            case RequestStatusEnum.OK:
                                return Ok(result.Message);
                            case RequestStatusEnum.BadRequest:
                                return BadRequest(result.Message);
                            case RequestStatusEnum.NotFound:
                                return NotFound(result.Message);

                            default:
                                return StatusCode(500, "Internal Server Error");
                        }
                    }
                    else
                    {
                        return BadRequest(new Response { Status = RequestStatusEnum.BadRequest, Message = "Please Check the Parameters" });
                    }

                }
                else
                {
                    return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
                }


            }

            catch (Exception ex)
            {
                _logger.LogError($"BoothMapping: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }



        [HttpGet]
        [Route("GetBoothListByBLOId")]
        [Authorize]
        public async Task<IActionResult> GetBoothListByBLO(string stateMasterId, string districtMasterId, string assemblyMasterId, string bloId)
        {
            var boothList = await _EAMSService.GetAssignedBoothListByBLOId(stateMasterId, districtMasterId, assemblyMasterId, bloId);  // Corrected to await the asynchronous method
            var mappedData = _mapper.Map<List<FieldOfficerBoothViewModel>>(boothList);
            var getUnassignedBoothList = await _EAMSService.GetUnassignedBLOBoothListById(stateMasterId, districtMasterId, assemblyMasterId);  // Corrected to await the asynchronous method
            var unAssignedMappedData = _mapper.Map<List<CombinedMasterViewModel>>(getUnassignedBoothList);
            var data = new
            {
                AssignedCount = mappedData.Count,
                UnAssignedCount = unAssignedMappedData.Count,
                Assigned = mappedData.OrderBy(p => Int32.Parse(p.BoothCode_No)),
                Unassigned = unAssignedMappedData
            };
            return Ok(data);
        }

        [HttpPut]
        [Route("ReleaseBoothBLO")]
        [Authorize]
        public async Task<IActionResult> ReleaseBoothforBLO(BoothReleaseViewModel boothReleaseViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var mapperdata = _mapper.Map<BoothMaster>(boothReleaseViewModel);
                    var boothReleaseResponse = await _EAMSService.ReleaseBoothBLO(mapperdata);

                    switch (boothReleaseResponse.Status)
                    {
                        case RequestStatusEnum.OK:
                            return Ok(boothReleaseResponse.Message);
                        case RequestStatusEnum.BadRequest:
                            return BadRequest(boothReleaseResponse.Message);
                        case RequestStatusEnum.NotFound:
                            return NotFound(boothReleaseResponse.Message);

                        default:
                            return StatusCode(500, "Internal Server Error");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }

            }


            else
            {
                return BadRequest(ModelState.Values.SelectMany(d => d.Errors.Select(d => d.ErrorMessage)).FirstOrDefault());
            }
        }
        #endregion

        #region GetBoothByLocation
        [HttpGet]
        [Route("GetBoothByLocation")]
        public async Task<IActionResult> GetBoothByLocation(string latitude, string longitude)
        {
            var boothList = await _EAMSService.GetBoothByLocation(latitude, longitude);

            if (boothList != null)
            {
                var data = new
                {
                    count = boothList.Count,
                    data = boothList.ToList(),
                    //data = boothList.OrderBy(p => Int32.Parse(p.BoothCode_No)).ToList(),

                };
                return Ok(data);

            }
            else
            {
                return NotFound("Data Not Found");

            }

        }
        #endregion

        #region GetMobileVersions
        [HttpGet]
        [Route("GetMobileVersion")]
        public async Task<IActionResult> GetMobileVersion(string StateMasterId)
        {
            var record = await _EAMSService.GetMobileVersionById(StateMasterId);

            if (record != null)
            {
                return Ok(record);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Route("AddMobileVersion")]
        public async Task<IActionResult> AddMobileVersion(MobileVersionViewModel mobileVersionViewModel)
        {
            var mappedData = _mapper.Map<MobileVersion>(mobileVersionViewModel);

            if (mappedData != null)
            {
                var result = await _EAMSService.AddMobileVersion(mappedData);
                if (result.IsSucceed == true)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {

                return BadRequest();
            }

        }


        #endregion

        #region Election Type Master
        [HttpGet]
        [Route("GetElectionType")]
        [Authorize]
        public async Task<IActionResult> GetAllElectionTypes()
        {
            var elecTypeList = await _EAMSService.GetAllElectionTypes();  // Corrected to await the asynchronous method
            if (elecTypeList != null)
            {
                var data = new
                {
                    count = elecTypeList.Count,
                    data = elecTypeList
                };
                return Ok(data);
            }
            else
            {
                return NotFound("Data Not Found");
            }



        }

        #endregion

    }
}