using Droplet.AutoMapper;
using Droplet.Bootstrapper;
using System;

namespace Droplet.Bootstrapper
{
    public static class DropletBuilderExtensions
    {
        public static DropletBuilder UseAutoMapper(this DropletBuilder @this)
        {
            var registrar = new AutoMapperRegistrar(@this.ServiceCollection, @this.RegisterAssemblies.ToArray());
            registrar.CreateMap();

            return @this;
        }
    }
}
