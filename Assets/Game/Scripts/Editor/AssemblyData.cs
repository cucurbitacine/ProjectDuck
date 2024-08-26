using System.Collections.Generic;
using System.Linq;
using UnityEditor.Compilation;

namespace Game.Scripts.Editor
{
    public struct AssemblyData
    {
        public Assembly assembly;
        public HashSet<Assembly> depends;
        public HashSet<Assembly> refers;

        public static List<AssemblyData> Data { get; } = new List<AssemblyData>();
        
        public static void Refresh()
        {
            Data.Clear();
            
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);

            foreach (var assembly in assemblies)
            {
                var assemblyData = new AssemblyData(assembly, assemblies);
                
                Data.Add(assemblyData);
            }
            
            Data.Sort(Comparison);
        }
        
        private static int Comparison(AssemblyData x, AssemblyData y)
        {
            if (x.refers.Count < y.refers.Count)
            {
                return -1;
            }
            
            if (x.refers.Count > y.refers.Count)
            {
                return 1;
            }

            return 0;
        }
        
        public AssemblyData(Assembly assembly)
        {
            this.assembly = assembly;

            depends = new HashSet<Assembly>();
            refers = new HashSet<Assembly>();
        }

        public AssemblyData(Assembly assembly, params Assembly[] library) : this(assembly)
        {
            foreach (var referencedAssembly in assembly.assemblyReferences)
            {
                refers.Add(referencedAssembly);
            }

            foreach (var assemblyFromLibrary in library)
            {
                var references = assemblyFromLibrary.assemblyReferences;

                if (references.Any(r => string.Equals(r.name, assembly.name)))
                {
                    depends.Add(assemblyFromLibrary);
                }
            }
        }
    }
}