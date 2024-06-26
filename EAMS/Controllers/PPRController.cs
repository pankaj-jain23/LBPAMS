using AutoMapper;
using EAMS.ViewModels;
using EAMS.ViewModels.Polling_Personal_Randomization_ViewModel;
using EAMS_ACore;
using EAMS_ACore.AuthInterfaces;
using EAMS_ACore.Interfaces;
using EAMS_ACore.Models.Polling_Personal_Randomisation_Models;
using EAMS_ACore.Models.Polling_Personal_Randomization_Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using Npgsql.PostgresTypes;

namespace EAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PPRController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PPRController> _logger;
        private readonly IEamsService _eamsService;
        public PPRController(IMapper mapper, ILogger<PPRController> logger, IEamsService eamsService)
        {
            _mapper = mapper;
            _logger = logger;
            _eamsService = eamsService;
        }

        [HttpPost]
        [Route("AddRandomization")]
        public async Task<IActionResult> AddRandomization(PPRViewModel pprViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isRandomizationAdded = await _eamsService.GetCurrentRoundByRandomizationById(pprViewModel.StateMasterId, pprViewModel.DistrictMasterId, pprViewModel.RandomizationTaskDetailMasterId);

            if (isRandomizationAdded != 0 && isRandomizationAdded != null)
            {
                return BadRequest("Randomization already started, please proceed to update randomization.");
            }

            var roundCount = await _eamsService.GetRoundCountByRandomizationTaskId(pprViewModel.RandomizationTaskDetailMasterId, pprViewModel.StateMasterId);

            if (pprViewModel.CurrentRound > roundCount)
            {
                return BadRequest("Round Exceeded");
            }

            var mappedData = _mapper.Map<PPR>(pprViewModel);
            var serviceResponse = await _eamsService.AddRandomization(mappedData);

            if (serviceResponse.IsSucceed)
            {
                return Ok(serviceResponse);
            }

            return BadRequest(serviceResponse);
        }
        [HttpPut]
        [Route("UpdateRandomization")]
        public async Task<IActionResult> UpdateRandomization(PPRViewUpdateModel pprViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var mappedData = _mapper.Map<PPR>(pprViewModel);
            var serviceResponse = await _eamsService.AddRandomization(mappedData);

            if (serviceResponse.IsSucceed)
            {
                return Ok(serviceResponse);
            }

            return BadRequest(serviceResponse);

        }

        [HttpGet]
        [Route("GetRandomizationListByStateId")]
        public async Task<IActionResult> GetRandomizationListByStateId(string stateMasterId)
        {
            if (string.IsNullOrEmpty(stateMasterId))
            {
                return BadRequest("StateMasterId cannot be null or empty.");
            }

            if (int.TryParse(stateMasterId, out int stateId))
            {
                var result = await _eamsService.GetRandomizationListByStateId(stateId);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Failed to retrieve randomization list.");
                }
            }
            else
            {
                return BadRequest("Invalid StateMasterId format.");
            }
        }

        [HttpGet]
        [Route("GetRandomizationById")]
        public async Task<IActionResult> GetRandomizationById(string pprMasterId)
        {
            if (string.IsNullOrEmpty(pprMasterId))
            {
                return BadRequest("pprMasterId cannot be null or empty.");
            }

            if (int.TryParse(pprMasterId, out int pprId))
            {
                var result = await _eamsService.GetRandomizationById(pprId);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Failed to retrieve randomization list.");
                }
            }
            else
            {
                return BadRequest("Invalid StateMasterId format.");
            }
        }


        [HttpGet]
        [Route("GetRandomizationTableListByStateId")]
        public async Task<IActionResult> GetRandomizationTableListByStateId(string stateMasterId)
        {
            if (string.IsNullOrEmpty(stateMasterId))
            {
                return BadRequest("StateMasterId cannot be null or empty.");
            }

            if (int.TryParse(stateMasterId, out int stateId))
            {
                var result = await _eamsService.GetRandomizationTableListByStateId(stateId);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Failed to retrieve randomization list.");
                }
            }
            else
            {
                return BadRequest("Invalid StateMasterId format.");
            }
        }



        [HttpGet]
        [Route("GetRandomizationListByDistrictId")]
        public async Task<IActionResult> GetRandomizationListByDistrictId(string stateMasterId, string districtMasterId)
        {
            if (string.IsNullOrEmpty(stateMasterId) && string.IsNullOrEmpty(districtMasterId))
            {
                return BadRequest("MasterId's cannot be null or empty.");
            }

            if (int.TryParse(stateMasterId, out int stateId) && int.TryParse(districtMasterId, out int districtId))
            {
                var result = await _eamsService.GetRandomizationListByDistrictId(stateId, districtId);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Failed to retrieve randomization list.");
                }
            }
            else
            {
                return BadRequest("Invalid StateMasterId format.");
            }
        }


        [HttpGet]
        [Route("GetRandomizationTaskListByStateId")]
        public async Task<IActionResult> GetRandomizationTaskListByStateId(string stateMasterId)
        {
            if (string.IsNullOrEmpty(stateMasterId))
            {
                return BadRequest("StateMasterId cannot be null or empty.");
            }

            if (int.TryParse(stateMasterId, out int stateId))
            {
                var result = await _eamsService.GetRandomizationTaskListByStateId(stateId);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Failed to retrieve randomization list.");
                }
            }
            else
            {
                return BadRequest("Invalid StateMasterId format.");
            }
        }

        [HttpPost]
        [Route("AddRandomizationTaskDetail")]
        public async Task<IActionResult> AddRandomizationTaskDetail(RandomizationTaskDetailViewModel randomizationTaskDetailViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mappedData = _mapper.Map<RandomizationTaskDetail>(randomizationTaskDetailViewModel);
            var result = await _eamsService.AddRandomizationTaskDetail(mappedData);

            return result.IsSucceed ? (IActionResult)Ok(result) : BadRequest(result);
        }
    }
}
