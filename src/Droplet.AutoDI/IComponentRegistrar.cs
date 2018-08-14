using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.AutoDI
{
    public interface IComponentRegistrar
    {
        void RegisterComponent(Type component);
        void RegisterAssembly(params Assembly[] assemblies);
    }
}
