using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.AutoDI
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<Type> ToServiceType(this IEnumerable<Type> @this)
        {
            foreach (var aType in @this)
            {
                if (aType.IsGenericType)
                {
                    yield return aType.GetGenericTypeDefinition();
                }
                else
                {
                    yield return aType;
                }
            }
        }
    }
}
