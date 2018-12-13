using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Droplet.Discovery.Consul
{
    public class ConsulServiceRegistrar : IServiceRegistrar
    {
        private readonly IConsulClient _consul;

        public ConsulServiceRegistrar(IConsulClient consul)
        {
            _consul = consul;
        }

       
        public async Task DeregisterServiceAsync(string serviceId)
        {
            await _consul.Agent.ServiceDeregister(serviceId);
        }

        public async Task<ServiceInformation> RegisterServiceAsync(string serviceName, string version, string host, int port, IEnumerable<string> tags = null)
        {
            var serviceId = GetServiceId(serviceName,host,port);

            var tagList = VersionHelper.SetVersionToTags(tags, version);

            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Tags = tagList.ToArray(),
                Address = host,
                Port = port,
                Check = new AgentCheckRegistration()
                {
                    TCP = $"{host}:{port}",
                    Status = HealthStatus.Passing,
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
                }
            };

            await _consul.Agent.ServiceRegister(registration);

            return new ServiceInformation
            {
                Name = registration.Name,
                Id = registration.ID,
                Host = registration.Address,
                Port = registration.Port,
                Version = version,
                Tags = tagList
            };
        }

        private string GetServiceId(string serviceName, string host, int port)
        {
            return $"{serviceName}.{host}.{port}";
        }

       
    }
}
