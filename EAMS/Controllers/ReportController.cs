using AutoMapper;
using EAMS.ViewModels.ReportViewModel;
using EAMS_ACore.Interfaces;
using EAMS_ACore.Models.CommonModels;
using EAMS_ACore.Models.ElectionType;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.ReportModels;
using LBPAMS.ViewModels.CommonModels;
using LBPAMS.ViewModels.PublicModels;
using LBPAMS.ViewModels.ReportViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IEamsService _EAMSService;
        private readonly IMapper _mapper;
        private string logsDirectory = @"C:\logs"; // Path to your logs directory

        public ReportController(IEamsService eamsService, IMapper mapper, ILogger<ReportController> logger)
        {
            _EAMSService = eamsService;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpPost]
        [Route("GetConsolidatedSarPanchResultDeclarationReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedSarPanchResultDeclarationReport([FromBody] ResultDeclarationReportListViewModel panchResultDeclarationReportListViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }


            var mappedData = _mapper.Map<ResultDeclarationReportListModel>(panchResultDeclarationReportListViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidatedSarPanchResultDeclarationReport(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NotFound();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedPanchResultDeclarationReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedPanchResultDeclarationReport([FromBody] ResultDeclarationReportListViewModel panchResultDeclarationReportListViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }

            // Map the incoming view model to the data model
            var mappedData = _mapper.Map<ResultDeclaration>(panchResultDeclarationReportListViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidatedPanchResultDeclarationReport(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NotFound();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedUnOppossedPanchResultDeclarationReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedUnOppossedPanchResultDeclarationReport([FromBody] ResultDeclarationReportListViewModel panchResultDeclarationReportListViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }

            // Map the incoming view model to the data model
            var mappedData = _mapper.Map<ResultDeclaration>(panchResultDeclarationReportListViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidatedUnOppossedPanchResultDeclarationReport(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NotFound();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedNoKycPanchResultDeclarationReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedNoKycPanchResultDeclarationReport([FromBody] ResultDeclarationReportListViewModel panchResultDeclarationReportListViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }

            // Map the incoming view model to the data model
            var mappedData = _mapper.Map<ResultDeclaration>(panchResultDeclarationReportListViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidatedNoKycPanchResultDeclarationReport(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NotFound();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedElectedSarPanchResultDeclarationReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedElectedSarPanchResultDeclarationReport([FromBody] ResultDeclarationReportListViewModel panchResultDeclarationReportListViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }


            var mappedData = _mapper.Map<ResultDeclarationReportListModel>(panchResultDeclarationReportListViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidatedElectedSarPanchResultDeclarationReport(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NotFound();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedElectedPanchResultDeclarationReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedElectedPanchResultDeclarationReport([FromBody] ResultDeclarationReportListViewModel panchResultDeclarationReportListViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }

            // Map the incoming view model to the data model
            var mappedData = _mapper.Map<ResultDeclaration>(panchResultDeclarationReportListViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidatedElectedPanchResultDeclarationReport(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NotFound();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedUnOppossedSarPanchResultDeclarationReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedUnOppossedSarPanchResultDeclarationReport([FromBody] ResultDeclarationReportListViewModel panchResultDeclarationReportListViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }


            var mappedData = _mapper.Map<ResultDeclarationReportListModel>(panchResultDeclarationReportListViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidatedUnOppossedSarPanchResultDeclarationReport(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NotFound();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedNoKycSarPanchResultDeclarationReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedNoKycSarPanchResultDeclarationReport([FromBody] ResultDeclarationReportListViewModel panchResultDeclarationReportListViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }


            var mappedData = _mapper.Map<ResultDeclarationReportListModel>(panchResultDeclarationReportListViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidatedNoKycSarPanchResultDeclarationReport(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NotFound();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedBoothReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedBoothReport([FromBody] BoothReportViewModel boothReportViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }

            // Map the incoming view model to the data model
            var mappedData = _mapper.Map<BoothReportModel>(boothReportViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidateBoothReports(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NoContent();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetConsolidatedGPWardReport")]
        [Authorize]
        public async Task<IActionResult> GetConsolidatedGPWardReport([FromBody] GPWardReportViewModel gPWardReportViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return BadRequest with validation errors
                return BadRequest(ModelState);
            }

            // Map the incoming view model to the data model
            var mappedData = _mapper.Map<BoothReportModel>(gPWardReportViewModel);

            // Get the consolidated report
            var records = await _EAMSService.GetConsolidateGPWardReports(mappedData);

            // Check if records were found
            if (records == null || !records.Any())
            {
                // Return NoContent if there are no records
                return NoContent();
            }

            // Return Ok with the result
            return Ok(records);
        }

        [HttpPost]
        [Route("GetSOReport")]
        [Authorize]
        public async Task<IActionResult> GetSOReport(BoothReportViewModel boothReportViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<BoothReportModel>(boothReportViewModel);


                //Assembly
                if (mappedData.AssemblyMasterId is not 0)
                {
                    var records = await _EAMSService.GetSOReport(mappedData);

                    return Ok(records);

                }
                else
                {
                    return BadRequest("Assembly Id not found");
                }



            }
            else
            {
                return BadRequest("Invalid data");
            }

        }

        [HttpPost]
        [Route("GetPendingSOReport")]
        //[Authorize]
        public async Task<IActionResult> GetPendingSOReport(BoothReportViewModel boothReportViewModel)
        {

            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<BoothReportModel>(boothReportViewModel);

                var records = await _EAMSService.GetPendingSOReport(mappedData);

                return Ok(records);




            }
            else
            {
                return BadRequest("Invalid data");
            }




        }



        [HttpGet]
        [Route("GetAssemblyWisePendingBooth")]
        [Authorize]
        public async Task<IActionResult> GetAssemblyWisePendingBooth(string stateMasterId)
        {
            var result = await _EAMSService.GetAssemblyWisePendingReports(stateMasterId);

            return Ok(result);
        }

        #region Voter Turn Out Polling station Reports
        [HttpPost]
        [Route("GetVoterTurnOutPollingStationReports")]
        [Authorize]
        public async Task<IActionResult> GetVoterTurnOutPollingStationReports(BoothReportViewModel boothReportViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<BoothReportModel>(boothReportViewModel);

                //if (mappedData.StateMasterId is not 0 && mappedData.DistrictMasterId is 0 && mappedData.PCMasterId is 0 && mappedData.AssemblyMasterId is 0)
                //{
                //    var records = await _EAMSService.GetVoterTurnOutPollingStationReports(mappedData);
                //    return Ok(records);
                //}
                ////District
                //else if (mappedData.StateMasterId is not 0 && mappedData.DistrictMasterId is not 0 && mappedData.PCMasterId is 0 && mappedData.AssemblyMasterId is 0)
                //{
                //    var records = await _EAMSService.GetVoterTurnOutPollingStationReports(mappedData);
                //    return Ok(records);
                //}
                ////PC
                //else if (mappedData.StateMasterId is not 0 && mappedData.DistrictMasterId is 0 && mappedData.PCMasterId is not 0 && mappedData.AssemblyMasterId is 0)
                //{
                //    var records = await _EAMSService.GetVoterTurnOutPollingStationReports(mappedData);
                //    return Ok(records);

                //}
                ////Assembly
                //else if (mappedData.AssemblyMasterId is not 0)
                //{
                var records = await _EAMSService.GetVoterTurnOutPollingStationReports(mappedData);
                return Ok(records);

                // }




            }
            else
            {

            }

            return Ok();
        }
        #endregion

        #region Voter Turn Out Consolidated

        /* [HttpPost]
         [Route("GetSlotBasedVTReport")]

         public async Task<IActionResult> GetSlotBasedVoterTurnOutReport(SlotVTReportViewModel slotVTReportViewModel)
         {
             if (ModelState.IsValid)
             {
                 if (Convert.ToInt32(slotVTReportViewModel.SlotMasterId) > 0)
                 {
                     var mappedData = _mapper.Map<SlotVTReportModel>(slotVTReportViewModel);
                     //State
                     if (mappedData.StateMasterId is not 0 && mappedData.DistrictMasterId is 0 && mappedData.PCMasterId is 0 && mappedData.AssemblyMasterId is 0)
                     {
                         var records = await _EAMSService.GetSlotBasedVoterTurnOutReport(mappedData);
                         return Ok(records);
                     }
                     //District
                     else if (mappedData.StateMasterId is not 0 && mappedData.DistrictMasterId is not 0 && mappedData.PCMasterId is 0 && mappedData.AssemblyMasterId is 0)
                     {
                         var records = await _EAMSService.GetSlotBasedVoterTurnOutReport(mappedData);
                         return Ok(records);
                     }
                     //PC
                     else if (mappedData.StateMasterId is not 0 && mappedData.DistrictMasterId is 0 && mappedData.PCMasterId is not 0 && mappedData.AssemblyMasterId is 0)
                     {
                         var records = await _EAMSService.GetSlotBasedVoterTurnOutReport(mappedData);
                         return Ok(records);

                     }
                     //Assembly
                     else if (mappedData.AssemblyMasterId is not 0)
                     {
                         var records = await _EAMSService.GetSlotBasedVoterTurnOutReport(mappedData);

                         return Ok(records);

                     }
                 }
                 else
                 {
                     return BadRequest("Slot Management Id is not Valid");

                 }



             }
             else
             {
                 return BadRequest("Input Model Is not Valid");

             }

             return Ok();
         }*/




        #endregion




        #region Voter Turn Out Consolidated
        [HttpPost]
        [Route("GetVoterTurnOutConsolidatedReports1")]
        [Authorize]
        public async Task<IActionResult> GetVoterTurnOutConsolidatedReport([FromBody] BoothReportViewModel boothReportViewModel)
        {
            List<VTReportModel> test = new List<VTReportModel>();
            if (ModelState.IsValid)
            {

                var mappedData = _mapper.Map<BoothReportModel>(boothReportViewModel);

                //State DistrictACWise
                if (mappedData.StateMasterId is not 0 && mappedData.Type == "DistrictACWise")
                {
                    var records = await _EAMSService.GetVoterTurnOutConsolidatedReports(mappedData);
                    if (records != null)
                    {
                        return Ok(records);
                    }
                    else
                    {
                        return NotFound();
                    }

                }

                //State PCACWise
                else if (mappedData.StateMasterId is not 0 && mappedData.Type == "PCACWise")
                {
                    var records = await _EAMSService.GetVoterTurnOutConsolidatedReports(mappedData);
                    if (records != null)
                    {
                        return Ok(records);
                    }
                    else
                    {
                        return NotFound();
                    }


                }

                //State DetailedDistrictACWise
                else if (mappedData.StateMasterId is not 0 && mappedData.Type == "DetailedDistrictACWise")

                {
                    var records = await _EAMSService.GetVoterTurnOutConsolidatedReports(mappedData);
                    if (records != null)
                    {
                        return Ok(records);
                    }
                    else
                    {
                        return NotFound();
                    }

                }




            }
            else
            {

            }

            return Ok();
        }

        #endregion



        [HttpGet]
        [Route("LastLog")]
        public async Task<IActionResult> GetLogs()
        {
            try
            {
                // Get the most recent log file
                var mostRecentLogFile = Directory.GetFiles(logsDirectory)
                                                  .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                                                  .FirstOrDefault();

                if (mostRecentLogFile != null)
                {
                    // Read the content of the most recent log file
                    string logs = System.IO.File.ReadAllText(mostRecentLogFile);
                    return Ok(logs);
                }
                else
                {
                    return NotFound("No log files found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("UploadAPK")]
        public async Task<IActionResult> UploadAPK(IFormFile apkFile)
        {

            if (apkFile == null || apkFile.Length == 0)
            {
                return BadRequest("APK file is not provided.");
            }

            try
            {
                // Ensure the directory exists, create if it doesn't
                string directoryPath = @"c:/LiveAPK";
                Directory.CreateDirectory(directoryPath);
                _logger.LogInformation("Worked");
                // Generate a unique filename for the APK
                // string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(apkFile.FileName);
                string filePath = Path.Combine(directoryPath, "PAMS-LIVE.apk");

                // Save the uploaded APK file to the designated folder
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await apkFile.CopyToAsync(stream);
                }

                // Return a success response
                return Ok("APK file uploaded successfully.");
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the upload process
                // You can replace Console.WriteLine with your logging mechanism
                Console.WriteLine($"Error uploading APK file: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload APK file.");
            }
        }

        [HttpGet]
        [Route("GetLatestAPK")]
        public IActionResult GetLatestAPK([FromServices] IWebHostEnvironment hostingEnvironment)
        {
            try
            {
                string directoryPath = @"c:/LiveAPK";

                // Create the directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                var latestAPK = directoryInfo.GetFiles("*.apk")
                                              .OrderByDescending(f => f.CreationTime)
                                              .FirstOrDefault();

                if (latestAPK != null)
                {
                    // Read the APK file content into a byte array
                    byte[] apkBytes;
                    using (FileStream fileStream = new FileStream(latestAPK.FullName, FileMode.Open, FileAccess.Read))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            apkBytes = memoryStream.ToArray();
                        }
                    }

                    // Return the APK file content as the response
                    return File(apkBytes, "application/vnd.android.package-archive", "PAMS-LIVE.apk");
                }
                else
                {
                    return NotFound("No APK file found.");
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the retrieval process
                Console.WriteLine($"Error retrieving latest APK: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve latest APK.");
            }
        }



        [HttpGet]
        [Route("GetSectorOfficersListNotDownloadedApp")]
        //[Authorize]
        public async Task<IActionResult> GetSectorOfficersListNotDownloadedApps(string stateMasterId)
        {


            var soList = await _EAMSService.AppNotDownload(stateMasterId);  // Corrected to await the asynchronous method
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



        #region Voter Turn Out Screens Disrict,AAssembly,BoothWise
        [HttpGet]
        [Route("GetSlotBasedVTOutReportDistrictWise")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin")]
        public async Task<IActionResult> GetSlotBasedVTOutReports(string? stateId, string? electionTypeId)
        {
            // Retrieve claims for stateMasterId and electionTypeMasterId
            var stateMasterIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId")?.Value;
            var electionTypeMasterIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ElectionTypeMasterId")?.Value;

            // Validate claims or fallback to parameters
            if (stateMasterIdClaim == null && string.IsNullOrEmpty(stateId))
                return BadRequest("StateMasterId claim or parameter is missing.");

            if (electionTypeMasterIdClaim == null && string.IsNullOrEmpty(electionTypeId))
                return BadRequest("ElectionTypeMasterId claim or parameter is missing.");

            // Use the provided stateId and electionTypeId or fallback to claims if not provided
            string stateMasterId = stateId ?? stateMasterIdClaim;
            string electionTypeMasterId = electionTypeId ?? electionTypeMasterIdClaim;

            // Call the service
            var eventDistrictWiseList = await _EAMSService.GetVoterTurnOutSlotBasedReport(stateMasterId, electionTypeMasterId);
            if (eventDistrictWiseList is not null)
                return Ok(eventDistrictWiseList);

            return NotFound();
        }


        [HttpGet]
        [Route("GetSlotBasedVTOutReportAssemblyWise")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,LocalBodiesAdmin,RO")]
        public async Task<IActionResult> GetSlotVTReporttAssemblyWise(string? stateId, string? districtId, string? electionTypeId)
        {
            var eventAssemblyList = await _EAMSService.GetSlotVTReporttAssemblyWise(stateId, districtId, electionTypeId);
            if (eventAssemblyList is not null)
                return Ok(eventAssemblyList);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetSlotBasedVTReportBoothWise")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,LocalBodiesAdmin,RO")]
        public async Task<IActionResult> GetSlotVTReportsBoothWise(string? stateId, string? districtId, string? assemblyId, string? electionTypeId)
        {
            var eventBoothList = await _EAMSService.GetSlotVTReportBoothWise(stateId, districtId, assemblyId, electionTypeId);
            if (eventBoothList is not null)
                return Ok(eventBoothList);
            else
                return NotFound();
        }
        #endregion


        #region SOCount and EVENtWise Count Pendency

        [HttpGet]
        [Route("GetDistrictWiseSOEvenCountReport")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,")]
        public async Task<IActionResult> EventListDistrictWiseById(string? stateId)
        {
            string stateMasterId;
            var stateMasterIdC = User.Claims.FirstOrDefault(c => c.Type == "StateMasterId").Value;

            if (stateId != null)
            {
                stateMasterId = stateId;
            }
            else
            {
                stateMasterId = stateMasterIdC.ToString();
            }

            var eventDistrictWiseList = await _EAMSService.GetDistrictWiseSOCountEventWiseCount(stateMasterId);
            if (eventDistrictWiseList is not null)
                return Ok(eventDistrictWiseList);
            else
                return NotFound();
        }




        [HttpGet]
        [Route("GetAssemblyWiseSOEvenCountReport")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin")]
        public async Task<IActionResult> GetAssemblyWiseSOCountPendency(string stateId, string districtId, string electionTypeMasterId)
        {
            var eventAssemblyList = await _EAMSService.GetAssemblyWiseSOCountEventWiseCount(stateId, districtId, electionTypeMasterId);
            if (eventAssemblyList is not null)
                return Ok(eventAssemblyList);
            else
                return NotFound();
        }



        //[HttpGet]
        //[Route("GetSOBoothWiseEvenCountReport")]
        //[Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,ARO")]
        //public async Task<IActionResult> GetSOWiseSOEvenCountPendency(string stateId, string districtId, string assemblyId)
        //{
        //    var eventAssemblyList = await _EAMSService.GetBoothWiseSOEventWiseCount(stateId, districtId,assemblyId);
        //    if (eventAssemblyList is not null)
        //        return Ok(eventAssemblyList);
        //    else
        //        return NotFound();
        //}


        [HttpGet]
        [Route("GetSOBoothWiseEvenCountReport")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,LocalBodiesAdmin,RO")]
        public async Task<IActionResult> GetSOWiseSOEvenCountPendency(string soMasterId)
        {
            var eventAssemblyList = await _EAMSService.GetBoothWiseSOEventWiseCount(soMasterId);
            if (eventAssemblyList is not null)
                return Ok(eventAssemblyList);
            else
                return NotFound();
        }
        [HttpGet]
        [Route("GetSONamesEventWiseCount")]
        [Authorize(Roles = "ECI,SuperAdmin,StateAdmin,DistrictAdmin,LocalBodiesAdmin,RO")]
        public async Task<IActionResult> GetSONamesEventWiseCounts(string stateMasterId, string districtMasterId, string assemblymasterId, string electionTypeMasterId)
        {
            var eventAssemblyList = await _EAMSService.GetSONamesEventWiseCount(stateMasterId, districtMasterId, assemblymasterId, electionTypeMasterId);
            if (eventAssemblyList is not null)
                return Ok(eventAssemblyList);
            else
                return NotFound();
        }




        #endregion


        #region BLO Report based on District

        [HttpPost]
        [Route("GetBLOQueueCountReportBoothWise")]
        [Authorize]
        public async Task<IActionResult> GetBLOQueueCountRepor([FromBody] BoothReportViewModel boothReportViewModel)
        {
            if (ModelState.IsValid)
            {

                var mappedData = _mapper.Map<BoothReportModel>(boothReportViewModel);

                //State DistrictACWise
                if (mappedData.StateMasterId is not 0 && mappedData.Type == "DistrictACWise")
                {
                    var records = await _EAMSService.GetBLOQueueCount(mappedData);
                    if (records != null)
                    {
                        return Ok(records);
                    }
                    else
                    {
                        return NotFound();
                    }

                }




            }
            else
            {

            }

            return Ok();
        }

        [HttpPost]
        [Route("GetUnassignedBLOList")]
        [Authorize]
        public async Task<IActionResult> GetUnassignedBLO([FromBody] BoothReportViewModel boothReportViewModel)
        {

            if (ModelState.IsValid)
            {

                var mappedData = _mapper.Map<BoothReportModel>(boothReportViewModel);

                //State DistrictACWise
                if (mappedData.StateMasterId is not 0 && mappedData.Type == "DistrictACWise")
                {
                    var records = await _EAMSService.GetUnassignedBLOs(mappedData);
                    if (records != null)
                    {
                        return Ok(records);
                    }
                    else
                    {
                        return NotFound();
                    }

                }




            }
            else
            {

            }

            return Ok();
        }



        [HttpPost]
        [Route("GetassignedUnassignedBLOList")]
        [Authorize]
        public async Task<IActionResult> GetassignedUnassignedBLOList([FromBody] BoothReportViewModel boothReportViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Handle invalid model state
                return BadRequest(ModelState);
            }

            var mappedData = _mapper.Map<BoothReportModel>(boothReportViewModel);

            // State DistrictACWise
            if (mappedData.StateMasterId != 0 && mappedData.Type == "DistrictACWise")
            {
                // Fetch unassigned and assigned BLOs concurrently
                var unassignedTask = _EAMSService.GetUnassignedBLOs(mappedData);
                var assignedTask = _EAMSService.GetAssignedBLOs(mappedData);

                // Await both tasks simultaneously
                await Task.WhenAll(unassignedTask, assignedTask);

                var unassignedBlos = await unassignedTask;
                var assignedBlos = await assignedTask;

                if (unassignedBlos != null || assignedBlos != null)
                {
                    // Return the data if found
                    return Ok(new { UnassignedBLOs = unassignedBlos, AssignedBLOs = assignedBlos });
                }
                else
                {
                    // Return 404 if no records found
                    return NotFound();
                }
            }

            // Handle other cases if needed

            return Ok(); // Default response
        }


        [HttpGet]
        [Route("GetBLOQueueCountReportBoothWiseOpen")]

        public async Task<IActionResult> GetBLOQueueCountReportBoothWiseOpen(string stateMasterId, string districtMasterId)
        {
            List<BLOBoothAssignedQueueCount> test = new List<BLOBoothAssignedQueueCount>();
            if (ModelState.IsValid)
            {



                //State DistrictACWise
                if (stateMasterId != "" && districtMasterId != "")
                {
                    var records = await _EAMSService.GetBLOQueueCountOpen(stateMasterId, districtMasterId);
                    if (records != null)
                    {
                        return Ok(records);
                    }
                    else
                    {
                        return NotFound();
                    }

                }




            }
            else
            {

            }

            return Ok();
        }



        #endregion


        #region CompletedVoterList
        [HttpPost("GetCompletedVTList")]
        public async Task<IActionResult> GetCompletedVTList(CommonReportViewModel commonReportViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Map the ViewModel to the Model
                    var mappedData = _mapper.Map<CommonReportModel>(commonReportViewModel);

                    // Fetch the data from the service
                    var records = await _EAMSService.GetCompletedVTList(mappedData);

                    // Check if records were found
                    if (records == null || !records.Any())
                    {
                        return NotFound(new
                        {
                            Message = "No records found for the provided criteria.",
                            Success = false,
                            Data = records
                        });
                    }

                    // Return the data with a success response
                    return Ok(new
                    {
                        Message = "Records fetched successfully.",
                        Success = true,
                        Data = records
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception (assumes a logger is available)
                    _logger.LogError(ex, "Error occurred while fetching the completed VT list.");

                    // Return an internal server error response
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Message = "An error occurred while processing your request. Please try again later.",
                        Success = false,
                        Error = ex.Message
                    });
                }
            }

            // Return bad request response for invalid model state
            return BadRequest(new
            {
                Message = "Invalid request data.",
                Success = false,
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
            });
        }

        #endregion


        #region Result Declartion DashBoard
        [HttpGet("GetResultByStateId")]
        [Authorize]
        public async Task<IActionResult> GetResultByStateId()
        {
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            int stateMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("StateMasterId"));
            int electionTypeMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("ElectionTypeMasterId"));
            var result = await _EAMSService.GetResultByStateId(stateMasterId, electionTypeMasterId);
            return Ok(result);

        }
        [HttpGet("GetResultByDistrictId")]
        [Authorize]
        public async Task<IActionResult> GetResultByDistrictId(int districtMasterId)
        {
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            int stateMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("StateMasterId"));
            int electionTypeMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("ElectionTypeMasterId"));
            var result = await _EAMSService.GetResultByDistrictId(stateMasterId, districtMasterId, electionTypeMasterId);
            return Ok(result);

        }
        [HttpGet("GetResultByAssemblyId")]
        [Authorize]
        public async Task<IActionResult> GetResultByAssemblyId(int districtMasterId, int assemblyMasterId)
        {
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            int stateMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("StateMasterId"));
            int electionTypeMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("ElectionTypeMasterId"));
            var result = await _EAMSService.GetResultByAssemblyId(stateMasterId, districtMasterId, assemblyMasterId, electionTypeMasterId);
            return Ok(result);

        }
        [HttpGet("GetResultByFourthLevelId")]
        [Authorize]
        public async Task<IActionResult> GetResultByFourthLevelId(int districtMasterId, int assemblyMasterId, int fourthLevelMasterId)
        {
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            int stateMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("StateMasterId"));
            int electionTypeMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("ElectionTypeMasterId"));
            var result = await _EAMSService.GetResultByFourthLevelId(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelMasterId, electionTypeMasterId);
            return Ok(result);

        }
        [HttpGet]
        [Route("GetPartyWiseResultByStateId")]
        public async Task<IActionResult> GetPartyWiseResultByStateId()
        {
            try
            {
                var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
                int stateMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("StateMasterId"));
                int electionTypeMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("ElectionTypeMasterId"));

                var results = await _EAMSService.GetPartyWiseResultByStateId(stateMasterId, electionTypeMasterId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion
    }
}
