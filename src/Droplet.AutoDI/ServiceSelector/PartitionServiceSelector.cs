using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.AutoDI.ServiceSelector
{
    public class PartitionServiceSelector : IServiceSelector
    {
        public IEnumerable<Type> SelectServices(Type component)
        {
            return selectServices(component).ToServiceType();
        }

        private IEnumerable<Type> selectServices(Type component)
        {
            var interfaces = component.GetInterfaces().Where(p => p.IsInterface);
            foreach (var aInterface in interfaces)
            {
                var name = aInterface.Name;
                if (aInterface.Name.StartsWith("I"))
                    name = name.Substring(1);

                if (component.Name.Contains(name))
                {
                    yield return aInterface;
                }
            }
        }
    }
}
