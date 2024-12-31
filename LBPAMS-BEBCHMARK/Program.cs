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
          BenchmarkRunner.Run<ApiBenchmark>( );
    }
}