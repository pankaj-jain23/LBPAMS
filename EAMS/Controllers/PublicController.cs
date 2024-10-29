using AutoMapper;
using EAMS.ViewModels;
using EAMS.ViewModels.PublicModels;
using EAMS.ViewModels.ReportViewModel;
using EAMS_ACore;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Interfaces;
using EAMS_ACore.Models.PublicModels;
using EAMS_ACore.ReportModels;
using EAMS_BLL.Services;
using LBPAMS.ViewModels.PublicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace EAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PublicController> _logger;
        private readonly IEamsService _eamsService;

        public PublicController(IMapper mapper, ILogger<PublicController> logger, IEamsService eamsService)
        {
            _mapper = mapper;
            _logger = logger;
            _eamsService = eamsService;
        }

        #region KYC
        [HttpPost("AddKYCDetails")]
        public async Task<IActionResult> AddKyc([FromForm] KycViewModel kycViewModel)
        {


            if (kycViewModel.NominationPdf == null || kycViewModel.NominationPdf.Length == 0)
            {
                return BadRequest("PDF file is missing.");
            }


            if (!kycViewModel.NominationPdf.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are allowed.");
            }

            const long MaxFileSize = 7 * 1024 * 1024;

            // Check if the file exceeds the maximum size
            if (kycViewModel.NominationPdf.Length > MaxFileSize)
            {
                return BadRequest($"File size exceeds the 7 MB limit.");
            }
            // Generate a unique file name for the PDF file
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(kycViewModel.NominationPdf.FileName);
            var folderPath = @"C:\inetpub\wwwroot\LBPAMSDOC\kyc";
            var filePath = Path.Combine(folderPath, fileName);

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await kycViewModel.NominationPdf.CopyToAsync(stream);

            }

            // Map the ViewModel to the Model
            var kyc = _mapper.Map<Kyc>(kycViewModel);
            var fullpathName = Path.Combine("kyc", fileName); // Store relative path
            kyc.NominationPdfPath = $"{fullpathName.Replace("\\", "/")}";
            // Call your service method to add KYC details
            var result = await _eamsService.AddKYCDetails(kyc);

            // Check if adding GP Voter details was successful
            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);

            }
            return Ok(result.Message);
        }

        [HttpGet("GetKYCDetails")]
        public async Task<IActionResult> GetKYCDetails()
        {
            var kycList = await _eamsService.GetKYCDetails();

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}/LBPAMSDOC/kyc";
            var kycResponses = kycList.Select(kyc => new KycResponseViewModel
            {
                KycMasterId = kyc.KycMasterId,
                StateMasterId = kyc.StateMasterId,
                DistrictMasterId = kyc.DistrictMasterId,
                ElectionTypeMasterId = kyc.ElectionTypeMasterId,
                AssemblyMasterId = kyc.AssemblyMasterId,
                FourthLevelHMasterId = kyc.FourthLevelHMasterId,
                GPPanchayatWardsMasterId = kyc.GPPanchayatWardsMasterId,
                CandidateName = kyc.CandidateName,
                FatherName = kyc.FatherName,
                IsUnOppossed = kyc.IsUnOppossed,
                Age = kyc.Age,
                NominationPdfPath = !string.IsNullOrEmpty(kyc.NominationPdfPath)
                    ? $"{baseUrl}/{Path.GetFileName(kyc.NominationPdfPath)}"
                    : null,

            }).ToList();

            return Ok(kycResponses);
        }


        [HttpPut("UpdateKycDetails")]
        public async Task<IActionResult> UpdateKyc([FromForm] UpdateKycViewModel updateKycViewModel)
        {
            // Check if the uploaded file is a PDF
            if (updateKycViewModel.NominationPdf != null)
            {
                if (!updateKycViewModel.NominationPdf.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !Path.GetExtension(updateKycViewModel.NominationPdf.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Only PDF files are allowed.");
                }
            }
            const long MaxFileSize = 7 * 1024 * 1024; // 7 MB in bytes
            // Check if the file exceeds the maximum size
            if (updateKycViewModel.NominationPdf != null && updateKycViewModel.NominationPdf.Length > MaxFileSize)
            {
                return BadRequest($"File size exceeds the 7 MB limit.");
            }
            // Update properties of the existing KYC object (except NominationPdfPath)
            var mappedData = _mapper.Map<Kyc>(updateKycViewModel);

            // Handle file upload (if applicable):
            if (mappedData.NominationPdfPath != null && mappedData.NominationPdfPath.Length > 0)
            {
                // Generate a unique file name for the PDF file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateKycViewModel.NominationPdf.FileName);
                var folderPath = @"C:\inetpub\wwwroot\LBPAMSDOC\kyc";
                var filePath = Path.Combine(folderPath, fileName);

                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save the new file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateKycViewModel.NominationPdf.CopyToAsync(stream);
                }

                // Update NominationPdfPath with the new file name
                mappedData.NominationPdfPath = Path.Combine("kyc", fileName);

                // Optionally, delete the old file if needed
                if (!string.IsNullOrEmpty(mappedData.NominationPdfPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", mappedData.NominationPdfPath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }

            // Call your service method to update KYC details
            var result = await _eamsService.UpdateKycDetails(mappedData);

            if (result.IsSucceed)
            {
                return Ok(new { Message = "KYC data updated successfully." });
            }
            else
            {
                if (result.Message.Contains("Age must be 21 or above"))
                {
                    return BadRequest("Age must be 21 or above.");
                }
                else if (result.Message.Contains("UnOpposed Sarpanch already exists"))
                {
                    return BadRequest("UnOpposed Sarpanch already exists.");
                }
                else if (result.Message.Contains("UnOpposed Panch already exists"))
                {
                    return BadRequest("UnOpposed Panch already exists.");
                }
                else
                {
                    return BadRequest("Failed to update KYC data.");
                }
            }
        }
       
        [HttpGet("GetKYCDetailByAssemblyId")]
        public async Task<IActionResult> GetKYCDetailByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            // Determine whether to call the method with user ID or not
            var roresult = userRole?.Contains("RO") == true
                ? await _eamsService.GetKYCDetailByAssemblyId(electionType, stateMasterId, districtMasterId, assemblyMasterId, userId)
                : await _eamsService.GetKYCDetailByAssemblyId(electionType, stateMasterId, districtMasterId, assemblyMasterId);

            if (roresult == null)
            {
                return NotFound();
            }

            // Prepare the response data
            var data = new
            {
                count = roresult.Count,
                Sarpacnh = roresult.Where(k => k.GPPanchayatWardsMasterId == 0).ToList(),
                Panch = roresult.Where(k => k.GPPanchayatWardsMasterId != 0).ToList()
            };

            return Ok(data);
        }


        [HttpGet("GetKYCDetailByFourthAndWardId")]
        public async Task<IActionResult> GetKYCDetailByFourthAndWardId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelMasterId, int? wardMasterId)
        {


            // Determine whether to call the method with user ID or not
            var roresult = await _eamsService.GetKYCDetailByFourthAndWardId(electionType, stateMasterId, districtMasterId, assemblyMasterId, fourthLevelMasterId, wardMasterId);

            if (roresult == null)
            {
                return NotFound();
            }

            // Prepare the response data
            var data = new
            {
                count = roresult.Count,
                Sarpacnh = roresult.Where(k => k.GPPanchayatWardsMasterId == 0).ToList(),
                Panch = roresult.Where(k => k.GPPanchayatWardsMasterId != 0).ToList()
            };

            return Ok(data);
        }


        [HttpGet("GetKycById")]
        public async Task<IActionResult> GetKycById(int KycMasterId)
        {
            if (KycMasterId == null)
            {
                return BadRequest("Master Id is null");
            }
            else
            {


                var resutlt = await _eamsService.GetKycById(KycMasterId);
                if (resutlt is not null)
                {
                    return Ok(resutlt);
                }
                else
                {
                    return BadRequest(resutlt);
                }
            }
        }

        [HttpDelete("DeleteKycById")]
        public async Task<IActionResult> DeleteKycById(int KycMasterId)
        {
            if (KycMasterId == null)
            {
                return BadRequest("Master Id is null");
            }
            else
            {


                var resutlt = await _eamsService.DeleteKycById(KycMasterId);
                if (resutlt.IsSucceed == true)
                {
                    return Ok(resutlt);
                }
                else
                {
                    return BadRequest(resutlt);
                }
            }
        }

        #endregion

        #region UnOpposed 

        [HttpPost("AddUnOpposedDetails")]
        public async Task<IActionResult> AddUnOpposedDetails([FromForm] UnOpposedViewModel unOppoedViewModel)
        {
            if (unOppoedViewModel.NominationPdf == null || unOppoedViewModel.NominationPdf.Length == 0)
            {
                return BadRequest("PDF file is missing.");
            }
            const long MaxFileSize = 7 * 1024 * 1024; // 7 MB in bytes
            // Check if the file exceeds the maximum size
            if (unOppoedViewModel.NominationPdf.Length > MaxFileSize)
            {
                return BadRequest($"File size exceeds the 7 MB limit.");
            }
            // Generate a unique file name for the PDF file
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(unOppoedViewModel.NominationPdf.FileName);
            var folderPath = @"C:\inetpub\wwwroot\LBPAMSDOC\unopposed";
            var filePath = Path.Combine(folderPath, fileName);

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await unOppoedViewModel.NominationPdf.CopyToAsync(stream);
            }

            // Map the ViewModel to the Model
            var mappedData = _mapper.Map<UnOpposed>(unOppoedViewModel);
            var fullpathName = Path.Combine("unopposed", fileName); // Store relative path
            mappedData.NominationPdfPath = $"{fullpathName.Replace("\\", "/")}";
            // Call your service method to add KYC details
            // Call your service method to add KYC details
            var result = await _eamsService.AddUnOpposedDetails(mappedData);

            // Check if adding KYC details was successful
            if (result.IsSucceed == true)
            {

                return Ok(new { Message = "UnOpposed data added successfully" });
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpGet("GetUnOpposedDetails")]
        public async Task<IActionResult> GetUnOpposedDetails()
        {
            var list = await _eamsService.GetUnOpposedDetails();

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}/LBPAMSDOC/unopposed";

            var kycResponses = list.Select(unOpposed => new UnOpposedResponseViewModel
            {
                UnOpposedMasterId = unOpposed.UnOpposedMasterId,
                StateMasterId = unOpposed.StateMasterId,
                DistrictMasterId = unOpposed.DistrictMasterId,
                ElectionTypeMasterId = unOpposed.ElectionTypeMasterId,
                AssemblyMasterId = unOpposed.AssemblyMasterId,
                FourthLevelHMasterId = unOpposed.FourthLevelHMasterId,
                GPPanchayatWardsMasterId = unOpposed.GPPanchayatWardsMasterId,
                PSZonePanchayatMasterId = unOpposed.PSZonePanchayatMasterId,
                CandidateName = unOpposed.CandidateName,
                FatherName = unOpposed.FatherName,
                NominationPdfPath = !string.IsNullOrEmpty(unOpposed.NominationPdfPath)
                    ? $"{baseUrl}/{Path.GetFileName(unOpposed.NominationPdfPath)}"
                    : null,

            }).ToList();

            return Ok(kycResponses);
        }

        [HttpGet("GetUnOpposedDetailsByAssemblyId")]
        public async Task<IActionResult> GetUnOpposedDetailsByAssemblyId(int electionType, int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var result = await _eamsService.GetUnOpposedDetailsByAssemblyId(electionType, stateMasterId, districtMasterId, assemblyMasterId);

            if (result != null)
            {
                var data = new
                {
                    count = result.Count,
                    Sarpacnh = result.Where(k => k.GPPanchayatWardsMasterId == 0).ToList(),
                    Panch = result.Where(k => k.GPPanchayatWardsMasterId != 0).ToList()

                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("UpdateUnOpposedDetails")]
        public async Task<IActionResult> UpdateUnOpposedDetails([FromForm] UpdateUnOpposedViewModel updateUnOpposedViewModel)
        {
            const long MaxFileSize = 7 * 1024 * 1024; // 7 MB in bytes
            // Check if the file exceeds the maximum size
            if (updateUnOpposedViewModel.NominationPdf != null && updateUnOpposedViewModel.NominationPdf.Length > MaxFileSize)
            {
                return BadRequest($"File size exceeds the 7 MB limit.");
            }
            // Update properties of the existing KYC object (except NominationPdfPath)
            var mappedData = _mapper.Map<UnOpposed>(updateUnOpposedViewModel);

            // Handle file upload (if applicable):
            if (mappedData.NominationPdfPath != null && mappedData.NominationPdfPath.Length > 0)
            {
                // Generate a unique file name for the PDF file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateUnOpposedViewModel.NominationPdf.FileName);
                var folderPath = @"C:\inetpub\wwwroot\LBPAMSDOC\unopposed";
                var filePath = Path.Combine(folderPath, fileName);

                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save the new file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateUnOpposedViewModel.NominationPdf.CopyToAsync(stream);
                }

                // Update NominationPdfPath with the new file name
                mappedData.NominationPdfPath = Path.Combine("unopposed", fileName);

                // Optionally, delete the old file if needed
                if (!string.IsNullOrEmpty(mappedData.NominationPdfPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", mappedData.NominationPdfPath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }

            // Call your service method to update KYC details
            var result = await _eamsService.UpdateUnOpposedDetails(mappedData);

            if (result.IsSucceed == true)
            {
                return Ok(new { Message = "UnOpposed data updated successfully" });
            }
            else
            {
                return BadRequest("Failed to update UnOpposed data.");
            }
        }

        [HttpGet("GetUnOpposedById")]
        public async Task<IActionResult> GetUnOpposedById(int unOpposedMasterId)
        {
            if (unOpposedMasterId == null)
            {
                return BadRequest("Master Id is null");
            }
            else
            {


                var resutlt = await _eamsService.GetUnOpposedById(unOpposedMasterId);
                if (resutlt is not null)
                {
                    return Ok(resutlt);
                }
                else
                {
                    return NotFound(resutlt);
                }
            }
        }

        [HttpDelete("DeleteUnOpposedById")]
        public async Task<IActionResult> DeleteUnOpposedById(int unOpposedMasterId)
        {
            if (unOpposedMasterId == null)
            {
                return BadRequest("Master Id is null");
            }
            else
            {

                var resutlt = await _eamsService.DeleteUnOpposedById(unOpposedMasterId);
                if (resutlt.IsSucceed == true)
                {
                    return Ok(resutlt);
                }
                else
                {
                    return BadRequest(resutlt);
                }
            }
        }

        #endregion

        #region GPVoter 
        [HttpPost("AddGPVoterDetails")]
        public async Task<IActionResult> AddGPVoterDetails([FromForm] GPVoterViewModel gpVoterPdfViewModel)
        {
            if (gpVoterPdfViewModel.GPVoterPdf == null || gpVoterPdfViewModel.GPVoterPdf.Length == 0)
            {
                return BadRequest("PDF file is missing.");
            }
            if (gpVoterPdfViewModel.GPVoterPdf != null)
            {
                if (!gpVoterPdfViewModel.GPVoterPdf.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                    !Path.GetExtension(gpVoterPdfViewModel.GPVoterPdf.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Only PDF files are allowed.");
                }
            }
            const long MaxFileSize = 7 * 1024 * 1024; // 7 MB in bytes

            // Check if the file exceeds the maximum size
            if (gpVoterPdfViewModel.GPVoterPdf.Length > MaxFileSize)
            {
                return BadRequest($"File size exceeds the 7 MB limit.");
            }
            // Generate a unique file name for the PDF file
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(gpVoterPdfViewModel.GPVoterPdf.FileName);
            var staticFolderPath = @"C:\inetpub\wwwroot\LBPAMSDOC\GPVoter";

            // Ensure the GPVoter directory exists, create if it doesn't
            if (!Directory.Exists(staticFolderPath))
            {
                Directory.CreateDirectory(staticFolderPath);
            }

            var filePath = Path.Combine(staticFolderPath, fileName);

            // Save the file to the static path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await gpVoterPdfViewModel.GPVoterPdf.CopyToAsync(stream);
            }

            // Map the ViewModel to the Model
            var mappedData = _mapper.Map<GPVoter>(gpVoterPdfViewModel);
            var fullpathName = Path.Combine("GPVoter", fileName);
            mappedData.GPVoterPdfPath = $"{fullpathName.Replace("\\", "/")}";
            mappedData.GPVoterCreatedAt = DateTime.UtcNow;
            mappedData.GPVoterUpdatedAt = DateTime.UtcNow;
            mappedData.GPVoterDeletedAt = DateTime.UtcNow;
            var result = await _eamsService.AddGPVoterDetails(mappedData);

            // Check if adding GP Voter details was successful
            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);

            }
            return Ok(result.Message);

        }
        [HttpPut("UpdateGPVoterDetails")]
        public async Task<IActionResult> UpdateGPVoterDetails([FromForm] UpdateGPVoterViewModel updateGPVoterViewModel)
        {
            // Fetch the existing GPVoter record from the database
            var existingGPVoter = await _eamsService.GetGPVoterById(updateGPVoterViewModel.GPVoterMasterId);
            if (existingGPVoter == null)
            {
                return NotFound("GP Voter not found.");
            }
            if (updateGPVoterViewModel.GPVoterPdf != null)
            {
                if (!updateGPVoterViewModel.GPVoterPdf.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !Path.GetExtension(updateGPVoterViewModel.GPVoterPdf.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Only PDF files are allowed.");
                }
            }
            // Define the maximum allowed size (7 MB)
            const long MaxFileSize = 7 * 1024 * 1024; // 7 MB in bytes

            // Check if the file exceeds the maximum size
            if (updateGPVoterViewModel.GPVoterPdf != null && updateGPVoterViewModel.GPVoterPdf.Length > MaxFileSize)
            {
                return BadRequest($"File size exceeds the 7 MB limit.");
            }
            // Map the ViewModel to the Model
            var mappedData = _mapper.Map<GPVoter>(updateGPVoterViewModel);

            // Define the static folder pathC:\inetpub\wwwroot\LBPAMSDOC
            var staticFolderPath = @"C:\inetpub\wwwroot\LBPAMSDOC\GPVoter";

            // Handle file upload (if applicable)
            if (updateGPVoterViewModel.GPVoterPdf != null && updateGPVoterViewModel.GPVoterPdf.Length > 0)
            {
                // If there is an existing file path, delete the old file
                if (!string.IsNullOrEmpty(existingGPVoter.GPVoterPdfPath))
                {
                    var oldFilePath = Path.Combine(staticFolderPath, Path.GetFileName(existingGPVoter.GPVoterPdfPath));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Generate a unique file name for the new PDF file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateGPVoterViewModel.GPVoterPdf.FileName);
                var filePath = Path.Combine(staticFolderPath, fileName);

                // Ensure the GPVoter directory exists, create if it doesn't
                if (!Directory.Exists(staticFolderPath))
                {
                    Directory.CreateDirectory(staticFolderPath);
                }

                // Save the new file to the static path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateGPVoterViewModel.GPVoterPdf.CopyToAsync(stream);
                }

                // Update GPVoterPath with the new file name
                mappedData.GPVoterPdfPath = Path.Combine("GPVoter", fileName).Replace("\\", "/");
            }

            // Update the timestamp for when the record was updated
            mappedData.GPVoterUpdatedAt = DateTime.UtcNow;

            // Call your service method to update GP Voter details
            var result = await _eamsService.UpdateGPVoterDetails(mappedData);
            // Check if adding GP Voter details was successful
            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);

            }
            return Ok(result.Message);
        }

        [HttpGet("GetGPVoterById")]
        public async Task<IActionResult> GetGPVoterById(int gpVoterMasterId)
        {
            var result = await _eamsService.GetGPVoterById(gpVoterMasterId);

            if (result != null)
            {
                var data = new
                {
                    gpVoter = result
                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetGPVoterListById")]
        public async Task<IActionResult> GetGPVoterListById(int stateMasterId, int districtMasterId, int assemblyMasterId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;



            // Determine whether to call the method with user ID or not
            var result = userRole?.Contains("RO") == true
                ? await _eamsService.GetGPVoterListById(stateMasterId, districtMasterId, assemblyMasterId, userId)
                : await _eamsService.GetGPVoterListById(stateMasterId, districtMasterId, assemblyMasterId);



            if (result == null)
            {
                return NotFound();
            }

            // Prepare the response data
            var data = new
            {
                count = result.Count,
                gpVoter = result,
            };

            return Ok(data);
        }
        [HttpDelete("DeleteGPVoterById")]
        public async Task<IActionResult> DeleteGPVoterById(int gpVoterMasterId)
        {
            if (gpVoterMasterId == null)
            {
                return BadRequest("Master Id is null");
            }
            else
            {

                var resutlt = await _eamsService.DeleteGPVoterById(gpVoterMasterId);
                if (resutlt.IsSucceed == true)
                {
                    return Ok(resutlt);
                }
                else
                {
                    return BadRequest(resutlt);
                }
            }
        }
        #endregion

        #region Result Declaration for Portal
        [HttpPost("AddResultDeclarationForPortal")]
        [Authorize]
        public async Task<IActionResult> AddResultDeclarationForPortal(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId, ResultDeclarationViewModel resultDeclarationViewModel)
        {
            if (resultDeclarationViewModel.resultDeclarationLists == null || !resultDeclarationViewModel.resultDeclarationLists.Any())
            {
                return BadRequest("No data provided.");
            }

            // Retrieve claims efficiently
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

            string userId =  userClaims.GetValueOrDefault("UserId").ToString();


            var mappedData = _mapper.Map<List<ResultDeclaration>>(resultDeclarationViewModel.resultDeclarationLists);

            // Assign common values
            mappedData.ForEach(resultDeclaration =>
            {
                resultDeclaration.StateMasterId = stateMasterId;
                resultDeclaration.DistrictMasterId = districtMasterId;
                resultDeclaration.AssemblyMasterId = assemblyMasterId;
                resultDeclaration.ElectionTypeMasterId = electionTypeMasterId;
                resultDeclaration.ResultDeclaredByPortal = userId;
            });

            // Save the mapped data
            var result = await _eamsService.AddResultDeclarationDetails(mappedData);

            // Handle the result
            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
        [HttpPut("UpdateResultDeclarationForPortal")]
        [Authorize]
        public async Task<IActionResult> UpdateResultDeclarationForPortal(int stateMasterId, int districtMasterId, int assemblyMasterId, int electionTypeMasterId, UpdateResultDeclarationViewModel updateResultDeclarationViewModel)
        {
            if (updateResultDeclarationViewModel == null)
            {
                return BadRequest("No data provided.");
            }

            // Retrieve claims efficiently
            //var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            //string userId = Convert.ToInt32(userClaims.GetValueOrDefault("UserId")).ToString();

            //// Map ViewModel to Entity
            //var mappedData = _mapper.Map<ResultDeclaration>(updateResultDeclarationViewModel);

            //// Assign common values
            //mappedData.StateMasterId = stateMasterId;
            //mappedData.DistrictMasterId = districtMasterId;
            //mappedData.AssemblyMasterId = assemblyMasterId;
            //mappedData.ElectionTypeMasterId = electionTypeMasterId;
            //mappedData.ResultDeclaredByPortal = userId;

            //// Call service method to update the record
            //var result = await _eamsService.UpdateResultDeclarationDetails(mappedData);

            //// Handle the result
            //if (!result.IsSu)
            //{
            //    return BadRequest(result.Message);
            //}

            //return Ok(result.Message);
            // Retrieve claims efficiently
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

            string userId = userClaims.GetValueOrDefault("UserId").ToString();

            // Check if all assigned booths have polls ended
            //var pollCheckResponse = await _eamsService.CheckIfAllBoothsPollEnded(fieldOfficerMasterId);

            //if (!pollCheckResponse.IsSucceed)
            //{
            //    return BadRequest(pollCheckResponse.Message);
            //}
            // Map ViewModel to Entity
            var mappedData = _mapper.Map<List<ResultDeclaration>>(updateResultDeclarationViewModel.updateResultDeclarationLists);

            // Assign common values
            mappedData.ForEach(resultDeclaration =>
            {
                resultDeclaration.StateMasterId = stateMasterId;
                resultDeclaration.DistrictMasterId = districtMasterId;
                resultDeclaration.AssemblyMasterId = assemblyMasterId;
                resultDeclaration.ElectionTypeMasterId = electionTypeMasterId;
                resultDeclaration.ResultDeclaredByPortal = userId;
            });

            // Save the mapped data
            var result = await _eamsService.UpdateResultDeclarationForPortal(mappedData);

            // Handle the result
            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpGet("GetResultByBoothId")]
        //[Authorize]
        public async Task<IActionResult> GetResultByBoothId(int boothMasterId)
        {
            if (boothMasterId is 0)
            {
                return BadRequest("Booth MasterId is Required");
            }
            var result = await _eamsService.GetResultByBoothId(boothMasterId);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);

        }

        [HttpGet("GetBoothResultListByFourthLevelId")]
        //[Authorize]
        public async Task<IActionResult> GetBoothResultListByFourthLevelId(int fourthLevelHMasterId)
        {
            if (fourthLevelHMasterId is 0)
            {
                return BadRequest("Booth MasterId is Required");
            }
            var result = await _eamsService.GetBoothResultListByFourthLevelId(fourthLevelHMasterId);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);

        }
        [HttpGet("GetResultByWardId")]
        //[Authorize]
        public async Task<IActionResult> GetResultByWardId(int wardMasterId)
        {
            if (wardMasterId is 0)
            {
                return BadRequest("Booth MasterId is Required");
            }
            var result = await _eamsService.GetResultByWardId(wardMasterId);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);

        }

        [HttpGet("GetWardResultListByFourthLevelId")]
        //[Authorize]
        public async Task<IActionResult> GetWardResultListByFourthLevelId(int fourthLevelHMasterId)
        {
            if (fourthLevelHMasterId is 0)
            {
                return BadRequest("Booth MasterId is Required");
            }
            var result = await _eamsService.GetWardResultListByFourthLevelId(fourthLevelHMasterId);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);

        }
        #endregion

        #region ResultDeclaration For Mobile

        [HttpPost("AddResultDeclarationDetails")]
        [Authorize]
        public async Task<IActionResult> AddResultDeclarationDetails(ResultDeclarationViewModel resultDeclarationViewModel)
        {
            if (resultDeclarationViewModel.resultDeclarationLists == null || !resultDeclarationViewModel.resultDeclarationLists.Any())
            {
                return BadRequest("No data provided.");
            }

            // Retrieve claims efficiently
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

            int stateMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("StateMasterId"));
            int districtMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("DistrictMasterId"));
            int assemblyMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("AssemblyMasterId"));
            int electionTypeMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("ElectionTypeMasterId"));
            int fieldOfficerMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("FieldOfficerMasterId"));

            // Check if all assigned booths have polls ended
            //var pollCheckResponse = await _eamsService.CheckIfAllBoothsPollEnded(fieldOfficerMasterId);

            //if (!pollCheckResponse.IsSucceed)
            //{
            //    return BadRequest(pollCheckResponse.Message);
            //}
            // Map ViewModel to Entity
            var mappedData = _mapper.Map<List<ResultDeclaration>>(resultDeclarationViewModel.resultDeclarationLists);

            // Assign common values
            mappedData.ForEach(resultDeclaration =>
            {
                resultDeclaration.StateMasterId = stateMasterId;
                resultDeclaration.DistrictMasterId = districtMasterId;
                resultDeclaration.AssemblyMasterId = assemblyMasterId;
                //resultDeclaration.FourthLevelHMasterId = fourthLevelMasterId;
                resultDeclaration.ElectionTypeMasterId = electionTypeMasterId;
                resultDeclaration.ResultDeclaredByMobile = fieldOfficerMasterId.ToString();
            });

            // Save the mapped data
            var result = await _eamsService.AddResultDeclarationDetails(mappedData);

            // Handle the result
            if (!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }



        [HttpGet("GetSarpanchListById")]
        [Authorize]
        public async Task<IActionResult> GetSarpanchListById(int fourthLevelHMasterId)
        {
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

            int stateMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("StateMasterId"));
            int districtMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("DistrictMasterId"));
            int assemblyMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("AssemblyMasterId"));
            //int fourthLevelHMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("FourthLevelHMasterId"));
            int electionTypeMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("ElectionTypeMasterId"));
            var result = await _eamsService.GetSarpanchListById(stateMasterId, districtMasterId, electionTypeMasterId, assemblyMasterId, fourthLevelHMasterId);

            // Check for a message indicating the poll has not ended
            if (result.Any() && !string.IsNullOrEmpty(result.First().Message))
            {
                return BadRequest(result.First().Message); // Return the error message if the poll has not ended
            }

            if (result.Count != 0 || result != null)
            {
                var data = new
                {
                    count = result.Count,
                    resultDeclaration = result.Where(k => k.KycMasterId != 0).ToList(),

                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetSarpanchListByIdForPortal")]
        [Authorize]
        public async Task<IActionResult> GetSarpanchListByIdForPortal(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId)
        {
            var result = await _eamsService.GetSarpanchListById(stateMasterId, districtMasterId, electionTypeMasterId, assemblyMasterId, fourthLevelHMasterId);

            // Check for a message indicating the poll has not ended
            if (result.Any() && !string.IsNullOrEmpty(result.First().Message))
            {
                return BadRequest(result.First().Message); // Return the error message if the poll has not ended
            }

            if (result.Count != 0 || result != null)
            {
                var data = new
                {
                    count = result.Count,
                    resultDeclaration = result.Where(k => k.KycMasterId != 0).ToList(),

                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetPanchListById")]
        [Authorize]
        public async Task<IActionResult> GetPanchListById(int fourthLevelHMasterId, int gPPanchayatWardsMasterId)
        {
            var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

            int stateMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("StateMasterId"));
            int districtMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("DistrictMasterId"));
            int assemblyMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("AssemblyMasterId"));
            //int fourthLevelHMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("FourthLevelHMasterId"));
            int electionTypeMasterId = Convert.ToInt32(userClaims.GetValueOrDefault("ElectionTypeMasterId"));

            var result = await _eamsService.GetPanchListById(stateMasterId, districtMasterId, electionTypeMasterId, assemblyMasterId, fourthLevelHMasterId, gPPanchayatWardsMasterId);

            // Check for a message indicating the poll has not ended
            if (result.Any() && !string.IsNullOrEmpty(result.First().Message))
            {
                return BadRequest(result.First().Message); // Return the error message if the poll has not ended
            }

            if (result.Count != 0 || result != null)
            {
                var data = new
                {
                    count = result.Count,
                    resultDeclaration = result.Where(k => k.KycMasterId != 0).ToList(),

                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetPanchListByIdForPortal")]
        [Authorize]
        public async Task<IActionResult> GetPanchListByIdForPortal(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gPPanchayatWardsMasterId)
        {

            var result = await _eamsService.GetPanchListById(stateMasterId, districtMasterId, electionTypeMasterId, assemblyMasterId, fourthLevelHMasterId, gPPanchayatWardsMasterId);

            // Check for a message indicating the poll has not ended
            if (result.Any() && !string.IsNullOrEmpty(result.First().Message))
            {
                return BadRequest(result.First().Message); // Return the error message if the poll has not ended
            }

            if (result.Count != 0 || result != null)
            {
                var data = new
                {
                    count = result.Count,
                    resultDeclaration = result.Where(k => k.KycMasterId != 0).ToList(),

                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPut("UpdateResultDeclarationDetails")]
        [Authorize]
        public async Task<IActionResult> UpdateResultDeclarationDetails(UpdateResultDeclarationViewModel updateResultDeclarationViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<UpdateResultDeclarationViewModel, ResultDeclaration>(updateResultDeclarationViewModel);
                mappedData.ResultDecUpdatedAt = DateTime.UtcNow;
                var result = await _eamsService.UpdateResultDeclarationDetails(mappedData);
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


        [HttpGet("GetResultDeclarationById")]
        [Authorize]
        public async Task<IActionResult> GetResultDeclarationById(int resultDeclarationMasterId)
        {
            if (resultDeclarationMasterId == null)
            {
                return BadRequest("Master Id is null");
            }
            else
            {


                var resutlt = await _eamsService.GetResultDeclarationById(resultDeclarationMasterId);
                if (resutlt is not null)
                {
                    return Ok(resutlt);
                }
                else
                {
                    return NotFound(resutlt);
                }
            }
        }

        [HttpDelete("DeleteResultDeclarationById")]
        [Authorize]
        public async Task<IActionResult> DeleteResultDeclarationById(int resultDeclarationMasterId)
        {
            if (resultDeclarationMasterId == null)
            {
                return BadRequest("Master Id is null");
            }
            else
            {

                var resutlt = await _eamsService.DeleteResultDeclarationById(resultDeclarationMasterId);
                if (resutlt.IsSucceed == true)
                {
                    return Ok(resutlt);
                }
                else
                {
                    return BadRequest(resutlt);
                }
            }
        }

        [HttpGet("GetResultDeclarationsByElectionType")]
        public async Task<IActionResult> GetResultDeclarationsByElectionType(int stateMasterId, int districtMasterId, int electionTypeMasterId, int assemblyMasterId, int fourthLevelHMasterId, int gpPanchayatWardsMasterId)
        {
            var result = await _eamsService.GetResultDeclarationsByElectionType(stateMasterId, districtMasterId, electionTypeMasterId, assemblyMasterId, fourthLevelHMasterId, gpPanchayatWardsMasterId);

            if (result.Count != 0 || result != null)
            {
                var data = new
                {
                    count = result.Count,
                    resultDeclaration = result.Where(k => k.ResultDeclarationMasterId != 0).ToList(),

                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

    }
}
