using RawRabbit;
using RawRabbit.Instantiation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Droplet.RawRabbit.MultipleConnection
{
    public class BusClientFactory : IBusClientFactory
    {
        private readonly Dictionary<string, RawRabbitOptions> _rawRabbitOptionsDic;

        private readonly Dictionary<string, IBusClient> _busClientDic;


        public BusClientFactory(Dictionary<string, RawRabbitOptions> rawRabbitOptionsDic)
        {
            _rawRabbitOptionsDic = rawRabbitOptionsDic;
            _busClientDic = new Dictionary<string, IBusClient>();
        }

        private object _lock = new object();

        public IBusClient Create(string configName)
        {
            if (!_busClientDic.ContainsKey(configName))
            {
                lock (_lock)
                {
                    if (_busClientDic.ContainsKey(configName))
                    {
                        return _busClientDic[configName];
                    }

                    if (!_rawRabbitOptionsDic.ContainsKey(configName))
                    {
                        throw new ArgumentException($"No RawRabbitConfiguration found with name:{configName}");
                    }
                    var busClient = RawRabbitFactory.CreateSingleton(_rawRabbitOptionsDic[configName]);
                    _busClientDic.Add(configName, busClient);
                }
            }
            return _busClientDic[configName];
        }
    }
}
