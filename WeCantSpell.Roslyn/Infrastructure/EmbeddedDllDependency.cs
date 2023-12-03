using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WeCantSpell.Roslyn.Infrastructure
{
    public static class EmbeddedDllDependency
    {
        private static bool s_isInitialized;
        public static void Init()
        {
            if (s_isInitialized) return;
            s_isInitialized = true;
            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            {
                AssemblyName name = new(args.Name);
                Assembly? loadedAssembly = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(a => a.GetName().FullName == name.FullName);
                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                var resourceName = $"{typeof(SpellingAnalyzerCSharp).Namespace}.{name.Name}.dll";

                using Stream? resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                if (resourceStream == null)
                {
                    return null;
                }

                using var memoryStream = new MemoryStream();
                resourceStream.CopyTo(memoryStream);

                return Assembly.Load(memoryStream.ToArray());
            };
        }
    }
}
