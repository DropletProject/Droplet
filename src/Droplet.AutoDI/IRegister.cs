using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.AutoDI
{
    public interface IRegister
    {
        void Register(Type component, Type service, ComponentAttribute attr = null);
    }
}
