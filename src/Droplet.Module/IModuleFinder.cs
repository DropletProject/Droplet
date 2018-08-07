using Droplet.Module.AssemblySelector;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.Module
{
    public interface IModuleFinder
    {


        List<Assembly> GetModuleAssemblies();
    }
}
