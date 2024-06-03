using AutoMapper;
 
using EAMS.ViewModels;
using EAMS_ACore.HelperModels;
using EAMS_ACore; 
using EAMS_ACore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfflineEventcontroller : ControllerBase
    {
        private readonly ILogger<OfflineEventcontroller> _logger;
        private readonly IEamsService _EAMSService;
        private readonly IMapper _mapper;

        public OfflineEventcontroller(IEamsService eamsService, IMapper mapper, ILogger<OfflineEventcontroller> logger)
        {
            _EAMSService = eamsService;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpPost]
        [Route("Event")]
        public async Task<IActionResult> Event(string par1, string par2, string par3)
        {
            return Ok($"{par1} {par2} {par3}");
        }

    }
}
