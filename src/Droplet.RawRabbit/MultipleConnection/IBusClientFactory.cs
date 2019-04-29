using RawRabbit;

namespace Droplet.RawRabbit.MultipleConnection
{
    public interface IBusClientFactory
    {
        IBusClient Create(string configName);
    }

    
}
