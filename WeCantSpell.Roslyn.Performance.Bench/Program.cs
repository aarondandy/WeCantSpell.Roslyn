using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace WeCantSpell.Roslyn.Performance.Bench
{
    internal static class Program
    {
        private static void Main()
        {
            var config = ManualConfig
                .CreateMinimumViable()
                .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()))
                .AddLogger(ConsoleLogger.Default)
                .AddColumn(
                    TargetMethodColumn.Method,
                    StatisticColumn.Median,
                    StatisticColumn.StdDev,
                    StatisticColumn.Q1,
                    StatisticColumn.Q3
                );
            BenchmarkRunner.Run<ThisSolutionPerfSpec>(config);
        }
    }
}
