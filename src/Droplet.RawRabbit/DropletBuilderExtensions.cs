using Droplet.RawRabbit.AutoSubscribe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplet.Bootstrapper
{
    public static class DropletBuilderExtensions
    {
        public static DropletBuilder UseRawRabbit(this DropletBuilder builder, RawRabbitConfiguration rawRabbitConfiguration, Action<RawRabbitBuilder> action = null)
        {
            var rawRabbitBuilder = new RawRabbitBuilder(builder);
            action?.Invoke(rawRabbitBuilder);
          
            builder.ServiceCollection.AddRawRabbit(new RawRabbitOptions
            {
                ClientConfiguration = rawRabbitConfiguration,
                Plugins = p =>
                {
                    if(rawRabbitBuilder.ClientBuilderAction == null)
                    {
                        p.UseExtensionRetryLater();
                    }
                    else
                    {
                        rawRabbitBuilder.ClientBuilderAction(p.UseExtensionRetryLater());
                    }
                },
            });

            return builder;
        }

        public static DropletBuilder UseRawRabbit(this DropletBuilder builder, 
            IConfiguration configuration, string configurationSection = "RawRabbit", 
            Action<RawRabbitBuilder> action = null)
        {
            var connectString = configuration.GetConnectionString(configurationSection);
            connectString = connectString.Replace(@"amqp://","");
            var rawRabbitConfiguration = ConnectionStringParser.Parse(connectString);
            rawRabbitConfiguration.PublishConfirmTimeout = TimeSpan.FromSeconds(5);
            return builder.UseRawRabbit(rawRabbitConfiguration, action);
        }

        public static DropletBuilder UseRawRabbit(this DropletBuilder builder, 
            IConfiguration configuration, string configurationSection = "RawRabbit",
            Action<RawRabbitConfiguration> configurationAction = null, 
            Action<RawRabbitBuilder> builderAction = null)
        {
            var connectString = configuration.GetConnectionString(configurationSection);
            connectString = connectString.Replace(@"amqp://", "");
            var rawRabbitConfiguration = ConnectionStringParser.Parse(connectString);
            rawRabbitConfiguration.PublishConfirmTimeout = TimeSpan.FromSeconds(5);
            configurationAction?.Invoke(rawRabbitConfiguration);
            return builder.UseRawRabbit(rawRabbitConfiguration, builderAction);
        }
    }

    public class RawRabbitBuilder
    {
        protected readonly DropletBuilder _dropletBuilder;

        public RawRabbitBuilder(DropletBuilder dropletBuilder)
        {
            _dropletBuilder = dropletBuilder;
        }

        internal Action<IClientBuilder> ClientBuilderAction { get; private set; }

        public RawRabbitBuilder UseAutoSubscribe(Action<AutoSubscribeConfiguration> action = null)
        {
            _dropletBuilder.ServiceCollection.AddAutoSubscribe(action);
            _dropletBuilder.ServiceCollection.Scan(
                s => s.FromAssemblies(_dropletBuilder.RegisterAssemblies)
                    .AddClasses(classes => classes.AssignableTo(typeof(IConsumeAsync<>))).AsImplementedInterfaces().WithTransientLifetime()
                );
            return this;
        }

        public RawRabbitBuilder UsePlugins(Action<IClientBuilder> action)
        {
            ClientBuilderAction = action;
            return this;
        }


    }
}
