using AutoMapper;
using EAMS_ACore.Interfaces;
using EAMS.ViewModels.PublicModels;
using EAMS_ACore.Models.PublicModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LBPAMS.ViewModels.PublicModels;
using EAMS.ViewModels;

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
            kyc.NominationPdfPath = Path.Combine("pdfs", fileName); // Store relative path

            // Call your service method to add KYC details
            var result = await _eamsService.AddKYCDetails(kyc);

            // Check if adding KYC details was successful
            if (result.IsSucceed == true)
            {
                // Construct the base URL for PDF paths
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}/pdfs";

                // Construct the full URL for NominationPdfPath
                var fullPdfPath = $"{baseUrl}/{fileName}";

                // Return success response with the full PDF path
                return Ok(new { Message = "KYC data added successfully", NominationPdfPath = fullPdfPath });
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
            var baseUrl = $"{request.Scheme}://{request.Host}/pdfs";

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
            mappedData.NominationPdfPath = Path.Combine("pdfs", fileName); // Store relative path

            // Call your service method to add KYC details
            var result = await _eamsService.AddUnOpposedDetails(mappedData);

            // Check if adding KYC details was successful
            if (result.IsSucceed == true)
            {
                // Construct the base URL for PDF paths
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}/pdfs";

                // Construct the full URL for NominationPdfPath
                var fullPdfPath = $"{baseUrl}/{fileName}";

                // Return success response with the full PDF path
                return Ok(new { Message = "UnOpposed data added successfully", NominationPdfPath = fullPdfPath });
            }
            else
            {
                return BadRequest("Failed to add KYC data.");
            }
        }

        [HttpGet("GetUnOpposedDetails")]
        public async Task<IActionResult> GetUnOpposedDetails()
        {
            var list = await _eamsService.GetUnOpposedDetails();

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}/pdfs";

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



        #endregion

    }
}
