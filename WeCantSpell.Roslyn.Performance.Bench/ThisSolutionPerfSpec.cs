using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using NBench;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WeCantSpell.Roslyn.Performance.Bench
{
    public class ThisSolutionPerfSpec
    {
        static string SearchForFile(string fileName)
        {
            var directory = new DirectoryInfo(".");
            do
            {
                var filePath = Path.Combine(directory.FullName, fileName);
                if (File.Exists(filePath))
                {
                    return filePath;
                }

                directory = directory.Parent;
            }
            while (directory != null);

            return null;
        }

        public Solution Solution;

        protected Counter ProjectsChecked;

        public void Setup()
        {
            var workspace = MSBuildWorkspace.Create();
            var solutionFilePath = SearchForFile("WeCantSpell.sln");
            Solution = workspace.OpenSolutionAsync(solutionFilePath).GetAwaiter().GetResult();
        }

        [PerfSetup]
        public void SetupBench(BenchmarkContext context)
        {
            Setup();

            ProjectsChecked = context.GetCounter(nameof(ProjectsChecked));
        }

        [PerfBenchmark(
            NumberOfIterations = 3,
            RunMode = RunMode.Throughput,
            TestMode = TestMode.Measurement,
            Description = "Measure how quickly a solution can be processed."
            )]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        [TimingMeasurement]
        [CounterMeasurement(nameof(ProjectsChecked))]
        public void Benchmark()
        {
            var analyzer = new SpellingAnalyzerCSharp(LengthWordChecker.Four);
            var allDiagnosticsByProject = Task.WhenAll(Solution.Projects.Select(p => FindSpellingMistakesForProject(p, analyzer)))
                .GetAwaiter()
                .GetResult();
        }

        async Task<ImmutableArray<Diagnostic>> FindSpellingMistakesForProject(Project project, SpellingAnalyzerCSharp analyzer)
        {
            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
            var diagnostics = await compilation
                .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer))
                .GetAnalyzerDiagnosticsAsync()
                .ConfigureAwait(false);

            ProjectsChecked.Increment();

            return diagnostics;
        }

        public class LengthWordChecker : ISpellChecker
        {
            public static LengthWordChecker Two { get; } = new LengthWordChecker(2);

            public static LengthWordChecker Four { get; } = new LengthWordChecker(4);

            public LengthWordChecker(int length) => Length = length;

            public int Length { get; }

            public bool Check(string word) =>
                word != null && (word.Length % Length) != 0;

            public IEnumerable<string> Suggest(string word) => Enumerable.Empty<string>();
        }
    }
}
