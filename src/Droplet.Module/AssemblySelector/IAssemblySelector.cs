using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.Module.AssemblySelector
{
    public interface IAssemblySelector
    {
        List<Assembly> SelectModuleAssembly(List<Assembly> waitSelectAssemblies);
    }
}
