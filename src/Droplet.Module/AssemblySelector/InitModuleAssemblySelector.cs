using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Droplet.Module.AssemblySelector
{
    public class InitModuleAssemblySelector : IAssemblySelector
    {
        public List<Assembly> SelectModuleAssembly(List<Assembly> waitSelectAssemblies)
        {
            var moduleAssemblies = new List<Assembly>();

            foreach (var aAssembly in waitSelectAssemblies)
            {
                if (isContainsInitModule(aAssembly))
                    moduleAssemblies.Add(aAssembly);
            }

            return moduleAssemblies;
        }

        private bool isContainsInitModule(Assembly checkAssembly)
        {
            if (checkAssembly.GetTypes().Count(p => typeof(IInitModule).IsAssignableFrom(p) && !p.IsInterface) > 0)
                return true;

            return false;
        }
    }
}
