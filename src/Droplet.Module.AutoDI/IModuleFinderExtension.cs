using Droplet.AutoDI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Module.AutoDI
{
    public static class IModuleFinderExtension
    {
        public static void AutoRegisterComponent(this IModuleFinder @this, IRegister register)
        {
            var registrar = new ComponentRegistrar(register);
            registrar.RegisterAssembly(@this.GetModuleAssemblies().ToArray());
        }
    }
}
