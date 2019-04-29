using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.RawRabbit.AutoSubscribe
{
    public interface IAutoSubscriber
    {
        /// <summary>
        /// 从程序集中找到自动订阅的类
        /// </summary>
        /// <param name="assemblies"></param>
        void Subscribe(params Assembly[] assemblies);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TAsyncConsumer"></typeparam>
        void Subscribe<TMessage, TAsyncConsumer>()
            where TMessage : class
            where TAsyncConsumer : class, IConsumeAsync<TMessage>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerType"></param>
        void Subscribe(Type consumerType);

    }
}
