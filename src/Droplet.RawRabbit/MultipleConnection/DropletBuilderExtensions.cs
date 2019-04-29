using Droplet.Bootstrapper;
using Droplet.RawRabbit.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.Instantiation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using RawRabbit.Common;

namespace Droplet.RawRabbit.MultipleConnection
{
    public static class DropletBuilderExtensions
    {
        /// <summary>
        /// 使用多个连接的RawRabbit
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="multipleRawRabbitConfigurationBuilderAction"></param>
        /// <param name="rawRabbitBuilderAction"></param>
        /// <returns></returns>
        public static DropletBuilder UseMultipleConnectionRawRabbit(this DropletBuilder builder, 
            Action<MultipleRawRabbitConfigurationBuilder> multipleRawRabbitConfigurationBuilderAction, 
            Action<RawRabbitBuilder> rawRabbitBuilderAction = null)
        {
            var multipleRawRabbitConfigurationBuilder = new MultipleRawRabbitConfigurationBuilder();
            multipleRawRabbitConfigurationBuilderAction.Invoke(multipleRawRabbitConfigurationBuilder);
            var rawRabbitBuilder = new RawRabbitBuilder(builder);
            rawRabbitBuilderAction?.Invoke(rawRabbitBuilder);

            Dictionary<string, RawRabbitOptions> rawRabbitOptionsDic = new Dictionary<string, RawRabbitOptions>();
            foreach (var item in multipleRawRabbitConfigurationBuilder.RawRabbitConfigurationDic)
            {
                if(item.Value.IsDefault)
                {
                    builder.UseRawRabbit(item.Value.Configuration, rawRabbitBuilderAction);
                }
                rawRabbitOptionsDic.Add(item.Key, new RawRabbitOptions() {
                    ClientConfiguration = item.Value.Configuration,
                    Plugins = p =>
                    {
                        if (rawRabbitBuilder.ClientBuilderAction == null)
                        {
                            p.UseExtensionRetryLater();
                        }
                        else
                        {
                            rawRabbitBuilder.ClientBuilderAction(p.UseExtensionRetryLater());
                        }
                    },
                });
            }

            builder.ServiceCollection.AddSingleton<IBusClientFactory>(new BusClientFactory(rawRabbitOptionsDic));

            return builder;
        }
    }

    public class MultipleRawRabbitConfigurationBuilder
    {

        internal Dictionary<string, InternalRawRabbitConfiguration> RawRabbitConfigurationDic { get; set; } = new Dictionary<string, InternalRawRabbitConfiguration>();

        public MultipleRawRabbitConfigurationBuilder()
        {
        }

        public MultipleRawRabbitConfigurationBuilder AddRawRabbitConfiguration(string configName, RawRabbitConfiguration rawRabbitConfiguration,bool isDefault = false)
        {
            if(RawRabbitConfigurationDic.ContainsKey(configName))
            {
                throw new ArgumentException($"{configName} Already exist");
            }
            if(isDefault)
            {
                if(RawRabbitConfigurationDic.Values.Any(s => s.IsDefault))
                {
                    throw new ArgumentException($"Default RawRabbitConfiguration already exists");
                }
            }
            RawRabbitConfigurationDic.Add(configName,new InternalRawRabbitConfiguration(rawRabbitConfiguration, isDefault));
            return this;
        }

        public MultipleRawRabbitConfigurationBuilder AddRawRabbitConfiguration(IConfiguration configuration, string configName, string configurationSection, bool isDefault = false)
        {
            var connectString = configuration.GetConnectionString(configurationSection);
            connectString = connectString.Replace(@"amqp://", "");
            var rawRabbitConfiguration = ConnectionStringParser.Parse(connectString);
            return AddRawRabbitConfiguration(configName, rawRabbitConfiguration, isDefault);
        }
    }

    internal class InternalRawRabbitConfiguration
    {
        internal InternalRawRabbitConfiguration(RawRabbitConfiguration configuration, bool isDefault = false)
        {
            Configuration = configuration;
            IsDefault = isDefault;
        }
        public RawRabbitConfiguration Configuration { get; set; }

        public bool IsDefault { get; set; }
    }
}
