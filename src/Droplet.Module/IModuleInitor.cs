using Droplet.Module.AssemblySelector;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Module
{
    public interface IModuleInitor
    {
        IModuleFinder AddFinder(IAssemblySelector finder);
        void Init();
    }
}
