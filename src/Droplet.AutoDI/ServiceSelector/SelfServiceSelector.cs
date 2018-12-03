using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.AutoDI.ServiceSelector
{
    public class SelfServiceSelector : IServiceSelector
    {
        public IEnumerable<Type> SelectServices(Type component)
        {
            if (component.IsGenericType)
            {
                yield return component.GetGenericTypeDefinition();
            }
            else
            {
                yield return component;
            }
        }
    }
}
