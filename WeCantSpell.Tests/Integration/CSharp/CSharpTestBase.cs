using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public abstract class CSharpTestBase
    {
        private static readonly string PathBase = "WeCantSpell.Tests.Integration.CSharp.Files";
        private static readonly string ProjectNameSingleFileSample = nameof(ProjectNameSingleFileSample);

        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Microsoft.CodeAnalysis.Compilation).Assembly.Location);

        protected Stream OpenCodeFileStream(string embeddedResourceFileName)
        {
            var fullEmbeddedResourcePath = PathBase + "." + embeddedResourceFileName;
            return typeof(CSharpTestBase).Assembly.GetManifestResourceStream(fullEmbeddedResourcePath);
        }

        protected async Task<string> ReadCodeFileAsStringAsync(string embeddedResourceFileName)
        {   
            using (var stream = OpenCodeFileStream(embeddedResourceFileName))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            {
                return await reader.ReadToEndAsync();
            }
        }

        protected async Task<TextAndVersion> ReadCodeFileAsSTextAndVersionAsync(string embeddedResourceFileName)
        {
            var sourceText = SourceText.From(await ReadCodeFileAsStringAsync(embeddedResourceFileName));
            var textAndVersion = TextAndVersion.Create(sourceText, VersionStamp.Default, embeddedResourceFileName);
            return textAndVersion;
        }

        protected async Task<Project> ReadCodeFileAsProjectAsync(string embeddedResourceFileName)
        {
            var file = await ReadCodeFileAsSTextAndVersionAsync(embeddedResourceFileName);
            return CreateProjectWithFiles(new[] { file });

        }

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

            foreach(var file in files)
            {
                var documentId = DocumentId.CreateNewId(projectId, debugName: file.FilePath);
                solution = solution.AddDocument(documentId, file.FilePath, file.Text);
            }

            return solution.GetProject(projectId);
        }

        protected Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(Project project, params DiagnosticAnalyzer[] analyzers)
        {
            return GetDiagnosticsAsync(project, ImmutableArray.CreateRange(analyzers));
        }

        protected async Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(Project project, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            var compilation = await project.GetCompilationAsync();
            var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
            return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        }
    }
}
