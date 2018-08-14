using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.AutoDI.ServiceSelector
{
    public class AllServiceSelector : IServiceSelector
    {
        public IEnumerable<Type> SelectServices(Type component)
        {
            var allInerfaces = component.GetInterfaces().Where(p => p.IsInterface).ToList();
            allInerfaces.Add(component);
            return allInerfaces;
        }
    }
}
