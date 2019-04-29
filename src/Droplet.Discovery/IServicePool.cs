
using System.Threading.Tasks;

namespace Droplet.Discovery
{
    public interface IServicePool
    {
        Task<ServiceInformation> GetAsync(string name, string version = "");
    }

   
}
