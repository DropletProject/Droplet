using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.ServiceProxy.Grpc.Internal
{
    internal class ServiceNameSelector : IServiceNameSelector
    {
        private readonly IDictionary<Type, string> _clientDic;
       
        public ServiceNameSelector( IDictionary<Type, string> clientDic)
        {
            _clientDic = clientDic;
        }
        public string Get(Type type)
        {
            if (!_clientDic.TryGetValue(type, out string serviceName))
            {
                throw new ArgumentException($"无法找到服务{type.FullName}的名称");
            }
            return serviceName;
        }
    }
}
