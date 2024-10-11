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
using Microsoft.AspNetCore.Mvc;

namespace LBPAMS_BEBCHMARK
{
    [MemoryDiagnoser]
    [ShortRunJob]
    [HideColumns(BenchmarkDotNet.Columns.Column.Job)]
    public class ApiBenchmark
    {
        private EAMSController _eamsController;
        private PublicController _publicController;
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
            var mockPublicLogger = new Mock<ILogger<PublicController>>();
            var mockCacheService = new Mock<ICacheService>();

            // Register the mocks with the service collection
            serviceCollection.AddSingleton(mockEamsService.Object);
            serviceCollection.AddSingleton(mockMapper.Object);
            serviceCollection.AddSingleton(mockLogger.Object);
            serviceCollection.AddSingleton(mockPublicLogger.Object);
            serviceCollection.AddSingleton(mockCacheService.Object);

            // Build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Initialize the controllers with the DI container
            _eamsController = new EAMSController(
                _serviceProvider.GetRequiredService<IEamsService>(),
                _serviceProvider.GetRequiredService<IMapper>(),
                _serviceProvider.GetRequiredService<ILogger<EAMSController>>(),
                _serviceProvider.GetRequiredService<ICacheService>()
            );
            _publicController = new PublicController(
                _serviceProvider.GetRequiredService<IMapper>(),
                _serviceProvider.GetRequiredService<ILogger<PublicController>>(),
                _serviceProvider.GetRequiredService<IEamsService>()
            );
        }

        [Benchmark]
        public async Task GetBoothListForFoBenchmark()
        {
            var result = await _eamsController.GetBoothListForFo();
            var okResult = result as OkObjectResult;
            // Optionally check the data
            var data = okResult?.Value; // Check if the returned data is as expected
        }

        [Benchmark]
        public async Task GetKYCDetails()
        {
            var result = await _publicController.GetKYCDetails();
            var okResult = result as OkObjectResult;
            // Optionally check the data
            var data = okResult?.Value; // Check if the returned data is as expected
        }

        [Benchmark]
        public async Task TestStateList()
        {
            // Call the controller method you want to benchmark
            var result = await _eamsController.StateList(); // Use await for async methods
            var okResult = result as OkObjectResult;
            // Optionally check the data
            var data = okResult?.Value; // Check if the returned data is as expected
        }
    }
}
