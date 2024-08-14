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
                SarpanchWardsMasterId = kyc.SarpanchWardsMasterId,
                BoothMasterId = kyc.BoothMasterId,
                CandidateName = kyc.CandidateName,
                FatherName = kyc.FatherName,
                NominationPdfPath = !string.IsNullOrEmpty(kyc.NominationPdfPath)
                    ? $"{baseUrl}/{Path.GetFileName(kyc.NominationPdfPath)}"
                    : null,

            }).ToList();

            return Ok(kycResponses);
        }
        #endregion

        #region ResultDecelration
        [HttpPost("AddResultDec")]
        public async Task<IActionResult> AddResult([FromForm] ResultViewModel resultViewModel)
        {
            var rslt = _mapper.Map<Rsult>(resultViewModel);
            if (rslt == null)
            {
                return BadRequest("All fields are mandatory");
            }
            // call your service method to add result details
            var addresult = await _eamsService.AddRsltDetails(rslt);
            if (addresult.IsSucceed == true)
            {
                return Ok(new { Message = "Result data added successfully" });
            }
            else
            {
                return BadRequest("Failed to add Result data.");
            }
        }
        #endregion
    }
}
