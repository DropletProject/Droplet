using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawRabbit.Common;
using System.Threading.Tasks;

namespace Droplet.RawRabbit.AutoSubscribe
{
    public interface IAutoSubscriberMessageDispatcher
    {
        Task<Acknowledgement> DispatchAsync<TMessage, TAsyncConsumer>(TMessage message, RetryMessageContext retryMessageContext)
            where TMessage : class
            where TAsyncConsumer : class, IConsumeAsync<TMessage>;
    }

   

   
}
