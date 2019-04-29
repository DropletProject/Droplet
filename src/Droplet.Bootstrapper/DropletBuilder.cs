using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.Bootstrapper
{
    public class DropletBuilder
    {
        public DropletBuilder(IServiceCollection services, List<Assembly> assembly)
        {
            ServiceCollection = services;
            RegisterAssemblies = assembly;
        }

        public List<Assembly> RegisterAssemblies { get; private set; }

        public IServiceCollection ServiceCollection { get; private set; }
    }
}
