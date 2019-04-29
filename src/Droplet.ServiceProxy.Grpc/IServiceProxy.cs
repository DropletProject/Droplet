

using Grpc.Core;
using System.Threading.Tasks;

namespace Droplet.ServiceProxy.Grpc
{
    public interface IServiceProxy
    {
        Task<TService> CreateAsync<TService>(string serviceName) where TService : ClientBase;
    }

    public interface IServiceProxy<TService> where TService : ClientBase
    {
        TService Service { get; }
    }



}
