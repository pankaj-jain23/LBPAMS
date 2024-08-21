using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;

namespace EAMS_BLL.Services
{
    public class ServicePlusService : IServicePlusService
    {
        private readonly IServicePlusRepository _servicePlusRepository;
        public ServicePlusService(IServicePlusRepository servicePlusRepository)
        {
            _servicePlusRepository = servicePlusRepository;

        }

    }
}
