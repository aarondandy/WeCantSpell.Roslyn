using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using WeCantSpell.Roslyn.Performance.Bench;

namespace WeCantSpell.Roslyn.Performance.Harness
{
    class Program
    {
        static void Main(string[] args)
        {
            var spec = new ThisSolutionPerfSpec();
            spec.Setup();

            for (var i = 0; i < 1000; i++)
            {
                RunTestAsync(spec.Solution).GetAwaiter().GetResult();
            }
        }

        static async Task RunTestAsync(Solution solution)
        {
            var analyzer = new SpellingAnalyzerCSharp(ThisSolutionPerfSpec.LengthWordChecker.Two);
            await Task.WhenAll(solution.Projects.Select(p => FindSpellingMistakesForProject(p, analyzer))).ConfigureAwait(false);
        }

        static async Task<ImmutableArray<Diagnostic>> FindSpellingMistakesForProject(Project project, SpellingAnalyzerCSharp analyzer)
        {
            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
            var diagnostics = await compilation
                .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer))
                .GetAnalyzerDiagnosticsAsync()
                .ConfigureAwait(false);

            return diagnostics;
        }
    }
}
