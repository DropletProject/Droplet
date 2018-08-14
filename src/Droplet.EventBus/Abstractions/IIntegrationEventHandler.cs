using Droplet.EventBus.Events;
using System.Threading.Tasks;

namespace Droplet.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> 
        where TIntegrationEvent: IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

  
}
