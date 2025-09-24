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
using Microsoft.Extensions.Configuration; // ✅ correct IConfiguration namespace

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
            var mockConfiguration = new Mock<IConfiguration>(); // ✅ added

            // Register the mocks with the service collection
            serviceCollection.AddSingleton(mockEamsService.Object);
            serviceCollection.AddSingleton(mockMapper.Object);
            serviceCollection.AddSingleton(mockLogger.Object);
            serviceCollection.AddSingleton(mockPublicLogger.Object);
            serviceCollection.AddSingleton(mockCacheService.Object);
            serviceCollection.AddSingleton(mockConfiguration.Object); // ✅ added

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
                _serviceProvider.GetRequiredService<IEamsService>(),
                _serviceProvider.GetRequiredService<IConfiguration>() // ✅ added
            );
        }

        [Benchmark]
        public async Task GetBoothListForFoBenchmark(CancellationToken cancellationToken)
        {
            var result = await _eamsController.GetBoothListForFo(cancellationToken);
            var okResult = result as OkObjectResult;
            var data = okResult?.Value;
        }

        [Benchmark]
        public async Task GetKYCDetails()
        {
            var result = await _publicController.GetKYCDetails();
            var okResult = result as OkObjectResult;
            var data = okResult?.Value;
        }

        [Benchmark]
        public async Task TestStateList()
        {
            var result = await _eamsController.StateList();
            var okResult = result as OkObjectResult;
            var data = okResult?.Value;
        }
    }
}
