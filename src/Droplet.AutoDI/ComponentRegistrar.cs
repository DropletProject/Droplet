using Droplet.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Droplet.AutoDI
{
    public class ComponentRegistrar : IComponentRegistrar
    {
        private IModuleFinder _moduleFinder;
        private IRegister _register;

        public ComponentRegistrar(IModuleFinder moduleFinder, IRegister register)
        {
            _moduleFinder = moduleFinder;
            _register = register;
        }

        public void AutoRegister()
        {
           var moduleAssemblies =  _moduleFinder.GetModuleAssemblies();
            foreach (var aAssembly in moduleAssemblies)
            {
                registerAssembly(aAssembly);
            }
        }

        private void registerAssembly(Assembly assembly)
        {
            var allTypes = assembly.GetTypes().Where(p => getComponentAttr(p) != null);
            foreach (var aComponent in allTypes)
            {
                var attr = getComponentAttr(aComponent);
                _register.Regiser(aComponent, attr);
            }
        }

        private ComponentAttribute getComponentAttr(Type type)
        {
            return type.GetCustomAttribute<ComponentAttribute>();
        }
    }
}
