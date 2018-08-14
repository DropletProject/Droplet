using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.AutoDI.ServiceSelector
{
    public class SelfServiceSelector : IServiceSelector
    {
        public IEnumerable<Type> SelectServices(Type component)
        {
            yield return component;
        }
    }
}
