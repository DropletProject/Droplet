using Microsoft.Extensions.Logging;
using Droplet.RawRabbit.AutoSubscribe;
using System;
using Droplet.Bootstrapper;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoSubscribe(this IServiceCollection services, Action<AutoSubscribeConfiguration> action = null)
        {
            var config = new AutoSubscribeConfiguration();
            action?.Invoke(config);
            if(config.NeedAckExceptionTypes!= null && config.NeedAckExceptionTypes.Length > 0)
            {
                foreach (var type in config.NeedAckExceptionTypes)
                {
                    if(!typeof(Exception).IsAssignableFrom(type))
                    {
                        throw new ArgumentException($"NeedAckExceptionTypes只能是继承于Exception");
                    }
                }
            }
            if(config.LogForMinExecutionDuration <=0)
            {
                throw new ArgumentException($"LogForMinExecutionDuration必须大于0");
            }
            services.AddLogging();
            services.AddSingleton<IAutoSubscriber, DefaultAutoSubscriber>();
            services.AddSingleton<IAutoSubscriberMessageDispatcher>(provider=> {
                return new DefaultAutoSubscriberMessageDispatcher(
                    provider,
                    provider.GetService<ILogger<DefaultAutoSubscriberMessageDispatcher>>(),
                    config
                    );
            });
            return services;
        }
    }

}
