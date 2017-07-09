using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp
{
    public abstract class CSharpTestBase
    {
        private static readonly string PathBase = $"{typeof(CSharpTestBase).Namespace}.Files";
        private static readonly string ProjectNameSingleFileSample = nameof(ProjectNameSingleFileSample);

        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).GetTypeInfo().Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).GetTypeInfo().Assembly.Location);

        protected Stream OpenCodeFileStream(string embeddedResourceFileName) =>
            typeof(CSharpTestBase).GetTypeInfo().Assembly.GetManifestResourceStream(PathBase + "." + embeddedResourceFileName);

        protected async Task<string> ReadCodeFileAsStringAsync(string embeddedResourceFileName)
        {
            using (var stream = OpenCodeFileStream(embeddedResourceFileName))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            {
                return await reader.ReadToEndAsync();
            }
        }

        protected async Task<TextAndVersion> ReadCodeFileAsSTextAndVersionAsync(string embeddedResourceFileName) =>
            TextAndVersion.Create(
                SourceText.From(await ReadCodeFileAsStringAsync(embeddedResourceFileName)),
                VersionStamp.Default,
                embeddedResourceFileName);

        protected async Task<Project> ReadCodeFileAsProjectAsync(string embeddedResourceFileName) =>
            CreateProjectWithFiles(new[] { await ReadCodeFileAsSTextAndVersionAsync(embeddedResourceFileName) });

        protected Project CreateProjectWithFiles(IEnumerable<TextAndVersion> files)
        {
            var projectId = ProjectId.CreateNewId(debugName: ProjectNameSingleFileSample);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, ProjectNameSingleFileSample, ProjectNameSingleFileSample, LanguageNames.CSharp)
                .AddMetadataReference(projectId, CorlibReference)
                .AddMetadataReference(projectId, SystemCoreReference)
                .AddMetadataReference(projectId, CSharpSymbolsReference)
                .AddMetadataReference(projectId, CodeAnalysisReference);

            foreach (var file in files)
            {
                var documentId = DocumentId.CreateNewId(projectId, debugName: file.FilePath);
                solution = solution.AddDocument(documentId, file.FilePath, file.Text);
            }

            return solution.GetProject(projectId);
        }

        protected Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(Project project, params DiagnosticAnalyzer[] analyzers) =>
            GetDiagnosticsAsync(project, ImmutableArray.CreateRange(analyzers));

        protected async Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(Project project, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            var compilation = await project.GetCompilationAsync();
            return await compilation
                .WithAnalyzers(analyzers)
                .GetAnalyzerDiagnosticsAsync();
        }
    }
}
