using Droplet.AutoDI;
using Droplet.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Droplet.Bootstrapper
{
    public static class DropletBuilderExtensions
    {
        public static DropletBuilder UseAutoDI(this DropletBuilder @this, Action<AutoDIBuilder> action = null)
        {
            var dotnetRegister = new DotnetRegister(@this.ServiceCollection);
            var registrar = new ComponentRegistrar(dotnetRegister);
            registrar.RegisterAssembly(@this.RegisterAssemblies.ToArray());
            var builder = new AutoDIBuilder(registrar, @this.RegisterAssemblies);
            action?.Invoke(builder);

            return @this;
        }
    }

    public class AutoDIBuilder
    {
        private readonly IComponentRegistrar _registrar;
        private readonly List<Assembly> _assemblies;


        public AutoDIBuilder(IComponentRegistrar registrar, List<Assembly> assemblies)
        {
            _registrar = registrar;
            _assemblies = assemblies;
        }

        public AutoDIBuilder RegisterByInterface(Type baseType, ComponentAttribute attr)
        {
            foreach (var aAssembly in _assemblies)
            {
                var childTypes = aAssembly.GetTypes().Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);
                foreach(var aChildType in childTypes)
                {
                    _registrar.RegisterComponent(aChildType, attr);
                }
            }

            return this;
        }
    }
}
