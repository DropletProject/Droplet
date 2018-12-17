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
                if (aType.IsGenericType && IsAllParameterGeneric(aType.GenericTypeArguments))
                {
                    yield return aType.GetGenericTypeDefinition();
                }
                else
                {
                    yield return aType;
                }
            }
        }

        private static bool IsAllParameterGeneric(Type[] types)
        {
            foreach (var aType in types)
            {
                if (!aType.IsGenericParameter)
                    return false;
            }

            return true;
        }
    }
}
