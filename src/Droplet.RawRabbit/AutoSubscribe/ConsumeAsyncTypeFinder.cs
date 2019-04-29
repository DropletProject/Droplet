using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Droplet.RawRabbit.AutoSubscribe
{
    public class ConsumeAsyncTypeFinder
    {
        public static IEnumerable<Type> Get(params Assembly[] assemblies)
        {
            var consumeType = typeof(IConsumeAsync<>);
            var classTypes = assemblies.SelectMany(
                a => a.GetTypes().Where(
                    t => t.GetTypeInfo().IsClass &&
                    !t.GetTypeInfo().IsAbstract && t.GetInterfaces().Any(g=> g.IsGenericType && g.GetGenericTypeDefinition() == consumeType)
                    )
                );
            return classTypes;
        }

        
    }
}
