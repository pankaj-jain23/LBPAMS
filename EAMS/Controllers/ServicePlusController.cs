using AutoMapper;
using EAMS.Controllers;
using EAMS_ACore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LBPAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicePlusController : ControllerBase
    {
        private readonly ILogger<ServicePlusController> _logger;
        private readonly IServicePlusService _servicePlusService;
        private readonly IMapper _mapper;

        public ServicePlusController(IServicePlusService servicePlusService,  IMapper mapper, ILogger<ServicePlusController> logger)
        {
            _servicePlusService = servicePlusService;
            _mapper = mapper;
            _logger = logger;
        }


    }
}
