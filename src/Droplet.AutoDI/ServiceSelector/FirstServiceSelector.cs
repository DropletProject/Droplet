using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Droplet.AutoDI.ServiceSelector
{
    public class FirstServiceSelector : IServiceSelector
    {
        public IEnumerable<Type> SelectServices(Type component)
        {
            var interfaces = component.GetInterfaces().Where( (p, i)=>p.IsInterface && i == 0);
            return interfaces.ToServiceType();
        }
    }
}
