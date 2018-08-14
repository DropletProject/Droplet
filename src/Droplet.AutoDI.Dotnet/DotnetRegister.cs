using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Droplet.AutoDI.Dotnet
{
    public class DotnetRegister : IRegister
    {
        private IServiceCollection _services;

        public DotnetRegister(IServiceCollection services)
        {
            _services = services;
        }

        public void Register(Type component, Type service)
        {
            var attr = component.GetComponentAttr();
            switch (attr.LiftTime)
            {
                case LifetimeType.Scopped:
                    _services.AddScoped(service, component);
                    break;
                case LifetimeType.Singleton:
                    _services.AddSingleton(service, component);
                    break;
                default:
                    _services.AddTransient(service, component);
                    break;
            }
        }
    }
}
