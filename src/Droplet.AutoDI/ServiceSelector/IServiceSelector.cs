using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.AutoDI.ServiceSelector
{
    public interface IServiceSelector
    {
        IEnumerable<Type> SelectServices(Type component);
    }
}
