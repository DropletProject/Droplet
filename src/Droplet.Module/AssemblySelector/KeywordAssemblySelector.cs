using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.Module.AssemblySelector
{
    public class KeywordAssemblySelector : IAssemblySelector
    {
        private string _keyword;

        public KeywordAssemblySelector(string keyword)
        {
            _keyword = keyword;
        }

        public List<Assembly> SelectModuleAssembly(List<Assembly> waitSelectAssemblies)
        {
            return waitSelectAssemblies.FindAll(p => p.FullName.Contains(_keyword));
        }
    }
}
