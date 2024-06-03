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

namespace EAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PPRController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PPRController> _logger;
        private readonly IEamsService _eamsService;
        public PPRController(IMapper mapper, ILogger<PPRController> logger,IEamsService eamsService )
        {
            _mapper = mapper;
            _logger = logger;
            _eamsService = eamsService;
        }

        [HttpPost]
        [Route("AddRandomization")]
        public async Task<IActionResult> AddRandomization(PPRViewModel pPRViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mappedData = _mapper.Map<PPR>(pPRViewModel);
            var result = await _eamsService.AddRandomization(mappedData);

            return result.IsSucceed ? (IActionResult)Ok(result) : BadRequest(result);
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
