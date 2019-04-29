using Microsoft.Extensions.Logging;
using Droplet.RawRabbit;
using Droplet.RawRabbit.AutoSubscribe;
using RawRabbit.Logging;
using System;
using System.Reflection;
using System.Linq;
using Droplet.Module;
using Microsoft.Extensions.DependencyInjection;
using Droplet.RawRabbit.MultipleConnection;
using RawRabbit;

namespace Droplet.RawRabbit.AutoSubscribe
{
    public static class AutoSubscribeExtensions
    {
        public static void UseAutoSubscribe(this IServiceProvider provider)
        {
            var loggerFactory = provider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
            {
                var logProvider = new MicrosoftLogProvider(provider.GetService<ILoggerFactory>().CreateLogger);
                LogProvider.SetCurrentLogProvider(logProvider);
            }

            var subscriber = provider.GetService<IAutoSubscriber>();
            var moduleFinder = provider.GetService<IModuleFinder>();
            subscriber.Subscribe(moduleFinder.GetModuleAssemblies().ToArray());
        }

        public static void StartAutoSubscribe(this IServiceProvider provider)
        {
            var loggerFactory = provider.GetService<ILoggerFactory>();
            if(loggerFactory != null)
            {
                var logProvider = new MicrosoftLogProvider(provider.GetService<ILoggerFactory>().CreateLogger);
                LogProvider.SetCurrentLogProvider(logProvider);
            }
            

            var busClientFactory = provider.GetService<IBusClientFactory>();
            var assemblies = provider.GetService<IModuleFinder>().GetModuleAssemblies();
            var dispatcher = provider.GetService<IAutoSubscriberMessageDispatcher>();
            if (busClientFactory == null)//如果没有注入IBusClientFactory表示没有启用多连接
            {
                provider.GetService<IAutoSubscriber>().Subscribe(assemblies.ToArray());
            }
            else
            {
                var classTypes = ConsumeAsyncTypeFinder.Get(assemblies.ToArray());
                var logger = provider.GetService<ILoggerFactory>().CreateLogger<DefaultAutoSubscriber>();
                foreach (var classType in classTypes)
                {
                    var connectionAttribute = classType.GetCustomAttribute<ConnectionAttribute>();
                    IBusClient busClient;
                    if (connectionAttribute == null)
                    {
                        busClient = provider.GetService<IBusClient>();
                    }
                    else
                    {
                        busClient = busClientFactory.Create(connectionAttribute.Name);
                        
                    }
                    var autoSubscriber = new DefaultAutoSubscriber(busClient, dispatcher, logger);
                    autoSubscriber.Subscribe(classType);
                }
            }
        }
    }
}
