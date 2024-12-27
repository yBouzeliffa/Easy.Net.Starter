using System.Reflection;

namespace Easy.Net.Starter.Extensions
{
    public static class AssemblyExtensions
    {
        public static Stream? FindAndGetFileFromEmbeddedResource(this string fileNameToFind, string globalAssemblyName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains(globalAssemblyName)).ToArray();
            Stream script = null;
            foreach (Assembly assembly in assemblies)
            {
                string[] assemblyFiles = assembly.GetManifestResourceNames();
                foreach (string assemblyFile in assemblyFiles)
                {
                    if (assemblyFile.Contains(fileNameToFind))
                    {
                        return assembly.GetManifestResourceStream(assemblyFile);
                    }
                }
            }
            return null;
        }

        public static Assembly? GetAssemblyFromClass(this string className)
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in assemblies)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.Name == className)
                        {
                            return assembly;
                        }
                    }
                }

                Console.WriteLine("The specified class was not found in the loaded assemblies.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has occurred: " + ex.Message);
                return null;
            }
        }

        public static string? GetNamespaceFromClass(this string className)
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in assemblies)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.Name == className)
                        {
                            return type.FullName;
                        }
                    }
                }

                Console.WriteLine("The specified class was not found in the loaded assemblies.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has occurred: " + ex.Message);
                return null;
            }
        }
    }
}
