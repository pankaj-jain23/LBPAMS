using AutoMapper;
using BenchmarkDotNet.Attributes;
using EAMS.Controllers;
using EAMS_ACore.IExternal;
using EAMS_ACore.Interfaces;
using EAMS_BLL.ExternalServices;
using EAMS_BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LBPAMS_BEBCHMARK
{
    public class ApiBenchmark
    {
        private EAMSController _eamsController;

        [GlobalSetup]
        public void Setup()
        {
            // Set up DI, services, and the controller
            var services = new ServiceCollection();

            // Register your services and controllers
            services.AddScoped<IEamsService, EamsService>();

          
            services.AddLogging();  // ILogger dependency
            services.AddScoped<ICacheService, CacheService>();  // Your cache service registration

            var serviceProvider = services.BuildServiceProvider();

            // Get the controller instance
            var eamsService = serviceProvider.GetRequiredService<IEamsService>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            var logger = serviceProvider.GetRequiredService<ILogger<EAMSController>>();
            var cacheService = serviceProvider.GetRequiredService<ICacheService>();

            _eamsController = new EAMSController(eamsService, mapper, logger, cacheService);
        }

        [Benchmark]
        public void TestYourApiMethod()
        {
            // Call your controller method
            var result = _eamsController.StateList().Result;  // or .Get() for sync
        }
    }


}
