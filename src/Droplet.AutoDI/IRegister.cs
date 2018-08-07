using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.AutoDI
{
    public interface IRegister
    {
        void Regiser(Type component, ComponentAttribute attr);
    }
}
