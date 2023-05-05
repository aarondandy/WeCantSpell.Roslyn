using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace WeCantSpell.Roslyn.Performance.Bench
{
    [MemoryDiagnoser]
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
            } while (directory != null);

            return null;
        }

        public Solution Solution;

        public void Setup()
        {
            MSBuildLocator.RegisterDefaults();
            var workspace = MSBuildWorkspace.Create();
            const string fileName = "WeCantSpell.Roslyn.sln";
            var solutionFilePath = SearchForFile(fileName);
            if (solutionFilePath == null)
                throw new InvalidOperationException($"Can't find {fileName} in current directory or its parents");
            Solution = workspace.OpenSolutionAsync(solutionFilePath).GetAwaiter().GetResult();
        }

        [GlobalSetup]
        public void SetupBench()
        {
            Setup();
        }

        [Benchmark(Description = "Measure how quickly a solution can be processed.")]
        public void Benchmark()
        {
            var analyzer = new SpellingAnalyzerCSharp(LengthWordChecker.Four);
            var allDiagnosticsByProject = Task.WhenAll(
                    Solution.Projects.Select(p => FindSpellingMistakesForProject(p, analyzer))
                )
                .GetAwaiter()
                .GetResult();
        }

        private async Task<ImmutableArray<Diagnostic>> FindSpellingMistakesForProject(
            Project project,
            DiagnosticAnalyzer analyzer
        )
        {
            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
            var diagnostics = await (compilation ?? throw new InvalidOperationException())
                .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer))
                .GetAnalyzerDiagnosticsAsync()
                .ConfigureAwait(false);

            return diagnostics;
        }

        public class LengthWordChecker : ISpellChecker
        {
            public static LengthWordChecker Two { get; } = new LengthWordChecker(2);

            public static LengthWordChecker Four { get; } = new LengthWordChecker(4);

            public LengthWordChecker(int length) => Length = length;

            public int Length { get; }

            public bool Check(string word) => word != null && (word.Length % Length) != 0;

            public IEnumerable<string> Suggest(string word) => Enumerable.Empty<string>();
        }
    }
}
