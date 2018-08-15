using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Droplet.Module.AssemblySelector
{
    public class SimiliarAssemblySelector : IAssemblySelector
    {
        private string[] _systemPrefix = { "System.", "Microsoft.", "mscorlib" };
        private Assembly _entryAssembly;


        public SimiliarAssemblySelector()
        {

        }

        public SimiliarAssemblySelector(Assembly entryAssembly)
        {
            _entryAssembly = entryAssembly;
        }

        public List<Assembly> SelectModuleAssembly(List<Assembly> waitSelectAssemblies)
        {
            var assemblySameCountClt = BuildAssemblySameCounts(filterSystemAssemblies(waitSelectAssemblies));
            var noEntryAssemblies = assemblySameCountClt.GetMaxTotalCountAssemblies();
            noEntryAssemblies.Add(_entryAssembly);

            return noEntryAssemblies;
        }

        private List<Assembly> filterSystemAssemblies(List<Assembly> waitSelectAssemblies)
        {
            List<Assembly> filteredAssemblies = waitSelectAssemblies;
            foreach (var aSystemPrefix in _systemPrefix)
            {
                filteredAssemblies = filteredAssemblies.Where(p => !p.FullName.StartsWith(aSystemPrefix)).ToList();
            }
            return filteredAssemblies;
        }

        private AssemblySameCountCollection BuildAssemblySameCounts(List<Assembly> waitBuildAssemblies)
        {
            if (_entryAssembly == null)
                _entryAssembly = Assembly.GetEntryAssembly();
            var entryAssemblyName = _entryAssembly.FullName;

            var assemblySameCounts = new AssemblySameCountCollection();
            foreach (var aAssembly in waitBuildAssemblies)
            {
                if (entryAssemblyName == aAssembly.FullName)
                    continue;

                var samecount = GetFirstNameSameCount(entryAssemblyName, aAssembly.FullName);

                var updateSameCount = assemblySameCounts.FirstOrDefault(p => p.SameCount == samecount);
                if (updateSameCount == null)
                {
                    updateSameCount = new AssemblySameCount(samecount);
                    assemblySameCounts.Add(updateSameCount);
                }

                updateSameCount.Add(aAssembly);
            }

            return assemblySameCounts;
        }

        private int GetFirstNameSameCount(string source, string compare)
        {
            var sameCount = 0;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] == compare[i])
                    sameCount++;
                else
                    break;
            }

            return sameCount;
        }

        private class AssemblySameCountCollection : List<AssemblySameCount>
        {
            public List<Assembly> GetMaxTotalCountAssemblies()
            {
                var similiarAssemblies = this.Where(p => p.SameCount > 2);
                if (similiarAssemblies.Count() == 0)
                    return new List<Assembly>();

                var maxTotalCount = this.FirstOrDefault(f => f.TotalCount == similiarAssemblies.Max(m => m.TotalCount));
                if (maxTotalCount == null)
                    return new List<Assembly>();

                return maxTotalCount.Assemblies;
            }
        }

        private class AssemblySameCount
        {
            public int SameCount { get; set; }
            public int TotalCount { get; set; }
            public List<Assembly> Assemblies { get; set; }

            public AssemblySameCount(int sameCount)
            {
                Assemblies = new List<Assembly>();
                SameCount = sameCount;
            }

            public void Add(Assembly addAssembly)
            {
                Assemblies.Add(addAssembly);
                TotalCount++;
            }
        }
    }
}
