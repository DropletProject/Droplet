using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext.Subscribe;
using Droplet.RawRabbit;
using Droplet.RawRabbit.AutoSubscribe;
using RawRabbit.Instantiation;
using RawRabbit.Middleware;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace RawRabbit
{
    public static class ExtensionRetryLaterPlugin
    {
        public static IClientBuilder UseExtensionRetryLater(this IClientBuilder builder)
        {
            builder.UseAttributeRouting().UseRetryLater().Register(
                pipe: p => {
                    p.Replace<RetryLaterMiddleware, ExtensionRetryLaterMiddleware>(args: new RetryLaterOptions()
                    {
                        DeliveryArgsFunc = pipeContext => {
                            var deliveryEventArgs = pipeContext.GetDeliveryEventArgs();
                            //var consumerConfiguration = pipeContext.GetConsumerConfiguration();
                            //if (consumerConfiguration != null && consumerConfiguration.Queue != null)
                            //{
                            //	deliveryEventArgs.RoutingKey = consumerConfiguration.Queue.Name;
                            //	deliveryEventArgs.Exchange = consumerConfiguration.Queue.Name;
                            //}
                            return deliveryEventArgs;
                        }
                    });
                });
            builder.Register(pipe: p => {
                p.Replace<HeaderDeserializationMiddleware, HeaderDeserializationMiddleware>(args: new HeaderDeserializationOptions()
                {
                    HeaderKeyFunc = c => PropertyHeaders.Context,
                    HeaderTypeFunc = c => c.GetMessageContextType(),
                    ContextSaveAction = (pipeCtx, msgCtx) => pipeCtx.Properties.TryAdd(PipeKey.MessageContext, msgCtx),
                    DeliveryArgsFunc = pipeContext => {
                        var deliveryEventArgs = pipeContext.GetDeliveryEventArgs();
                        //增加这个是因为RawRabbit中HeaderDeserializationMiddleware有一个BUG，在Headers==null的时候会引发一个异常
                        if (deliveryEventArgs.BasicProperties.Headers == null)
                        {
                            deliveryEventArgs.BasicProperties.Headers = new Dictionary<string, object>();
                        }
                        return deliveryEventArgs;
                    }
                });
            });
            return builder;
        }
    }
}
