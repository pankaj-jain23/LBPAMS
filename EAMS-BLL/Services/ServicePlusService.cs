using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_BLL.Services
{
    public class ServicePlusService: IServicePlusService
    {
        private readonly IServicePlusRepository _servicePlusRepository;
        public ServicePlusService(IServicePlusRepository servicePlusRepository)
        {
            _servicePlusRepository = servicePlusRepository;

        }
       
    }
}
