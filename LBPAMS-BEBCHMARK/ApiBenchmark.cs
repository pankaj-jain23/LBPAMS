using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using EAMS.Controllers;
using EAMS.Helper;
using EAMS_ACore.IExternal;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRepository;
using EAMS_BLL.ExternalServices;
using EAMS_BLL.Services;
using EAMS_DAL.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace LBPAMS_BEBCHMARK
{
    [MemoryDiagnoser]

    [HideColumns(BenchmarkDotNet.Columns.Column.Job)]
    public class ApiBenchmark
    {
        private EAMSController _eamsController;
        private ServiceProvider _serviceProvider;

        [GlobalSetup]
        public void Setup()
        {
            // Create a new service collection
            var serviceCollection = new ServiceCollection();

            // Add your mocked services
            var mockEamsService = new Mock<IEamsService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<EAMSController>>();
            var mockCacheService = new Mock<ICacheService>();

            // Register the mocks with the service collection
            serviceCollection.AddSingleton(mockEamsService.Object);
            serviceCollection.AddSingleton(mockMapper.Object);
            serviceCollection.AddSingleton(mockLogger.Object);
            serviceCollection.AddSingleton(mockCacheService.Object);

            // Build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Initialize the controller with the DI container
            _eamsController = new EAMSController(
                _serviceProvider.GetRequiredService<IEamsService>(),
                _serviceProvider.GetRequiredService<IMapper>(),
                _serviceProvider.GetRequiredService<ILogger<EAMSController>>(),
                _serviceProvider.GetRequiredService<ICacheService>()
            );
        }

        [Benchmark]
      
        public void TestStateList()
        {
            // Call the controller method you want to benchmark
            var result = _eamsController.StateList().Result; // Use Result for async methods
        }
    }
    
}
