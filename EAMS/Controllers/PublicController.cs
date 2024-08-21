using AutoMapper;
using EAMS.ViewModels;
using EAMS.ViewModels.PublicModels;
using EAMS_ACore.Interfaces;
using EAMS_ACore.Models.PublicModels;
using LBPAMS.ViewModels.PublicModels;
using Microsoft.AspNetCore.Mvc;

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

            // Generate a unique file name for the PDF file
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(kycViewModel.NominationPdf.FileName);
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
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
            var fullpathName = Path.Combine("pdfs", fileName); // Store relative path
            kyc.NominationPdfPath = $"{fullpathName.Replace("\\", "/")}";
            // Call your service method to add KYC details
            var result = await _eamsService.AddKYCDetails(kyc);

            // Check if adding KYC details was successful
            if (result.IsSucceed == true)
            {

                return Ok(new { Message = "KYC data added successfully" });
            }
            else
            {
                return BadRequest("Failed to add KYC data.");
            }
        }

        [HttpGet("GetKYCDetails")]
        public async Task<IActionResult> GetKYCDetails()
        {
            var kycList = await _eamsService.GetKYCDetails();

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}/lbpams/pdfs";

            var kycResponses = kycList.Select(kyc => new KycResponseViewModel
            {
                KycMasterId = kyc.KycMasterId,
                StateMasterId = kyc.StateMasterId,
                DistrictMasterId = kyc.DistrictMasterId,
                ElectionTypeMasterId = kyc.ElectionTypeMasterId,
                AssemblyMasterId = kyc.AssemblyMasterId,
                FourthLevelHMasterId = kyc.FourthLevelHMasterId,
                SarpanchWardsMasterId = kyc.SarpanchWardsMasterId,
                CandidateName = kyc.CandidateName,
                FatherName = kyc.FatherName,
                NominationPdfPath = !string.IsNullOrEmpty(kyc.NominationPdfPath)
                    ? $"{baseUrl}/{Path.GetFileName(kyc.NominationPdfPath)}"
                    : null,

            }).ToList();

            return Ok(kycResponses);
        }

        [HttpPut("UpdateKycDetails")]
        public async Task<IActionResult> UpdateKyc([FromForm] UpdateKycViewModel updateKycViewModel)
        {

            // Update properties of the existing KYC object (except NominationPdfPath)
            var mappedData = _mapper.Map<Kyc>(updateKycViewModel);

            // Handle file upload (if applicable):
            if (mappedData.NominationPdfPath != null && mappedData.NominationPdfPath.Length > 0)
            {
                // Generate a unique file name for the PDF file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateKycViewModel.NominationPdf.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
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
                mappedData.NominationPdfPath = Path.Combine("pdfs", fileName);

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

            if (result.IsSucceed == true)
            {
                return Ok(new { Message = "KYC data updated successfully" });
            }
            else
            {
                return BadRequest("Failed to update KYC data.");
            }
        }

        [HttpGet("GetKYCDetailByFourthLevelHId")]
        public async Task<IActionResult> GetKYCDetailByFourthLevelId(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelhMasterId)
        {
            var result = await _eamsService.GetKYCDetailByFourthLevelId(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelhMasterId);

            if (result.Count != 0 || result != null)
            {
                var data = new
                {
                    count = result.Count,
                    Sarpacnh = result.Where(k => k.SarpanchWardsMasterId == 0).ToList(),
                    Panch = result.Where(k => k.SarpanchWardsMasterId != 0).ToList()

                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
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
                    return NotFound(resutlt);
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

            // Generate a unique file name for the PDF file
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(unOppoedViewModel.NominationPdf.FileName);
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
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
            var fullpathName = Path.Combine("pdfs", fileName); // Store relative path
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
                return BadRequest("Failed to add UnOpposed data.");
            }
        }

        [HttpGet("GetUnOpposedDetails")]
        public async Task<IActionResult> GetUnOpposedDetails()
        {
            var list = await _eamsService.GetUnOpposedDetails();

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}/lbpams/pdfs";

            var kycResponses = list.Select(unOpposed => new UnOpposedResponseViewModel
            {
                UnOpposedMasterId = unOpposed.UnOpposedMasterId,
                StateMasterId = unOpposed.StateMasterId,
                DistrictMasterId = unOpposed.DistrictMasterId,
                ElectionTypeMasterId = unOpposed.ElectionTypeMasterId,
                AssemblyMasterId = unOpposed.AssemblyMasterId,
                FourthLevelHMasterId = unOpposed.FourthLevelHMasterId,
                SarpanchWardsMasterId = unOpposed.SarpanchWardsMasterId,
                CandidateName = unOpposed.CandidateName,
                FatherName = unOpposed.FatherName,
                NominationPdfPath = !string.IsNullOrEmpty(unOpposed.NominationPdfPath)
                    ? $"{baseUrl}/{Path.GetFileName(unOpposed.NominationPdfPath)}"
                    : null,

            }).ToList();

            return Ok(kycResponses);
        }
         
        [HttpGet("GetUnOpposedDetailsByFourthLevelId")]
        public async Task<IActionResult> GetUnOpposedDetailsByFourthLevelId(int stateMasterId, int districtMasterId, int assemblyMasterId, int fourthLevelhMasterId)
        {
            var result = await _eamsService.GetUnOpposedDetailsByFourthLevelId(stateMasterId, districtMasterId, assemblyMasterId, fourthLevelhMasterId);

            if (result.Count != 0 || result != null)
            {
                var data = new
                {
                    count = result.Count,
                    Sarpacnh = result.Where(k => k.SarpanchWardsMasterId == 0).ToList(),
                    Panch = result.Where(k => k.SarpanchWardsMasterId != 0).ToList()

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

            // Update properties of the existing KYC object (except NominationPdfPath)
            var mappedData = _mapper.Map<UnOpposed>(updateUnOpposedViewModel);

            // Handle file upload (if applicable):
            if (mappedData.NominationPdfPath != null && mappedData.NominationPdfPath.Length > 0)
            {
                // Generate a unique file name for the PDF file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateUnOpposedViewModel.NominationPdf.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
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
                mappedData.NominationPdfPath = Path.Combine("pdfs", fileName);

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

    }
}
