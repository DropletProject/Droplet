using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Discovery
{
    
    public interface IServiceRegistrar
    {
       
        Task<ServiceInformation> RegisterServiceAsync(string serviceName, string version, string host, int port, IEnumerable<string> tags = null);
       
        Task DeregisterServiceAsync(string serviceId);
    }
}
