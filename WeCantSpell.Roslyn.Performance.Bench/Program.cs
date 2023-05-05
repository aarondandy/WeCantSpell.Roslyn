using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace WeCantSpell.Roslyn.Performance.Bench
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ManualConfig.CreateMinimumViable()
                .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)))
                .AddLogger(ConsoleLogger.Default)
                .AddColumn(TargetMethodColumn.Method, StatisticColumn.Median, StatisticColumn.StdDev,
                    StatisticColumn.Q1, StatisticColumn.Q3);
            BenchmarkRunner.Run<ThisSolutionPerfSpec>(config);
        }
    }
}
