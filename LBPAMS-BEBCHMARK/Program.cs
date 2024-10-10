using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using LBPAMS_BEBCHMARK;

class Program
{
    static void Main(string[] args)
    {
        var config = ManualConfig
            .Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithToolchain(InProcessNoEmitToolchain.Instance)); // InProcess No Emit Toolchain

        var summary = BenchmarkRunner.Run<ApiBenchmark>(config);
    }
}