using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Discovery
{
  
    public interface IServiceDiscovery
    {

        Task<IEnumerable<ServiceInformation>> GetServicesAsync(string name, string version = "");
       
    }
}
