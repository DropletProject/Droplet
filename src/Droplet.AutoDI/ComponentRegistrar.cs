using Droplet.AutoDI.ServiceSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Droplet.AutoDI
{
    public class ComponentRegistrar : IComponentRegistrar
    {
        private IRegister _register;

        public ComponentRegistrar(IRegister register)
        {
            _register = register;
        }

        public void RegisterComponent(Type component)
        {
            var selectors = GetServiceSelectors(component);
            var services = GetServices(component, selectors);

            foreach (var aService in services)
            {
                _register.Register(component, aService);
            }
        }

        public void RegisterAssembly(params Assembly[] assemblies)
        {
            foreach (var aAssemby in assemblies)
            {
                var allTypes = aAssemby.GetTypes().Where(p => p.GetComponentAttr() != null);
                foreach (var aComponent in allTypes)
                {
                    RegisterComponent(aComponent);
                }
            }
        }

        private List<IServiceSelector> GetServiceSelectors(Type component)
        {
            var attr = component.GetComponentAttr();

            var serviceSelector = new List<IServiceSelector>();

            if (attr.RegisterService.HasFlag(RegisterServiceType.All))
            {
                serviceSelector.Add(new AllServiceSelector());
                return serviceSelector;
            }

            if (attr.RegisterService.HasFlag(RegisterServiceType.First))
                serviceSelector.Add(new FirstServiceSelector());
            if (attr.RegisterService.HasFlag(RegisterServiceType.Partition))
                serviceSelector.Add(new PartitionServiceSelector());
            if (attr.RegisterService.HasFlag(RegisterServiceType.Self))
                serviceSelector.Add(new SelfServiceSelector());

            return serviceSelector;
        }

        private List<Type> GetServices(Type component, List<IServiceSelector> selectors)
        {
            var services = new List<Type>();
            foreach (var aSelector in selectors)
            {
                services.AddRange(aSelector.SelectServices(component));
            }

            return services;
        }
    }
}
