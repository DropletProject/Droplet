using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.AutoDI
{
    public static class TypeExtension
    {
        public static ComponentAttribute GetComponentAttr(this Type @this)
        {
            return @this.GetCustomAttribute<ComponentAttribute>();
        }
    }
}
