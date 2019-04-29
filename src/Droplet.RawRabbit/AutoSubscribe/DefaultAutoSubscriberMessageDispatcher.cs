using Droplet.Data.Uow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawRabbit.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Droplet.RawRabbit.AutoSubscribe
{
    public class DefaultAutoSubscriberMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 需要ack的异常类型
        /// </summary>
        private readonly Type[] _needAckExceptionTypes;
        /// <summary>
        /// 重试次数
        /// </summary>
        private readonly int _retryCount;
        /// <summary>
        /// 重试间隔
        /// </summary>
        private readonly Func<int, TimeSpan> _rertyIntervalFunc;
        /// <summary>
        /// 执行时长大于多少输出日志
        /// </summary>
        private readonly int _logForMinExecutionDuration;

        public DefaultAutoSubscriberMessageDispatcher(IServiceProvider serviceProvider, ILogger<DefaultAutoSubscriberMessageDispatcher> logger, AutoSubscribeConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            _serviceProvider = serviceProvider;
            _logger = logger;

            _needAckExceptionTypes = configuration.NeedAckExceptionTypes?? new Type[] { };
            _retryCount = configuration.RetryCount;
            _rertyIntervalFunc = configuration.RertyIntervalFunc ?? GetRertyInterval;
            _logForMinExecutionDuration = configuration.LogForMinExecutionDuration;
        }

        private TimeSpan GetRertyInterval(int numberOfRetries)
        {
            //下次重试的时间间隔 3的N+1次方，重试3次的话时间间隔依次为：3,9,27
            return TimeSpan.FromMinutes(Math.Pow(3, numberOfRetries + 1)); 
        }

        public async Task<Acknowledgement> DispatchAsync<TMessage, TAsyncConsumer>(TMessage message, RetryMessageContext retryMessageContext)
            where TMessage : class
            where TAsyncConsumer : class, IConsumeAsync<TMessage>
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Dictionary<string, long> stepDic = new Dictionary<string, long>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var consumer = scope.ServiceProvider.GetService<IConsumeAsync<TMessage>>();
                if (consumer == null)
                {
                    _logger.LogWarning($"类型{typeof(TAsyncConsumer).Name} 找不到实现");
                    return new Ack();
                }
                try
                {
                    stepDic.Add("创建handler", stopwatch.ElapsedMilliseconds);
                    var unitOfWorkAttr = consumer.GetType().GetMethod(nameof(IConsumeAsync<TMessage>.ConsumeAsync)).GetCustomAttribute<UnitOfWorkAttribute>();
                    stepDic.Add("获取UnitOfWorkAttribute", stopwatch.ElapsedMilliseconds);
                    IUnitOfWork unifOfWork = null;
                    if (unitOfWorkAttr!= null)
                    {
                        unifOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                        if(unitOfWorkAttr.IsTransactional)
                        {
                            unifOfWork.Begin(unitOfWorkAttr.IsolationLevel);
                        }
                    }
                    stepDic.Add("开启工作单元", stopwatch.ElapsedMilliseconds);
                    await consumer.ConsumeAsync(message).ConfigureAwait(false);
                    stepDic.Add("执行handler", stopwatch.ElapsedMilliseconds);
                    if(unifOfWork != null)
                    {
                        await unifOfWork.CompleteAsync();
                    }
                    stepDic.Add("提交工作单元", stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    if (_needAckExceptionTypes.Contains(ex.GetType()))
                    {
                        return new Ack();
                    }
                    if (retryMessageContext.RetryInfo.NumberOfRetries < _retryCount - 1)
                    {
                        var timespan = _rertyIntervalFunc.Invoke(retryMessageContext.RetryInfo.NumberOfRetries);
                        _logger.LogError(ex, $"message:{Newtonsoft.Json.JsonConvert.SerializeObject(message)},类型{typeof(TAsyncConsumer).Name}消息处理发生了异常,当前重试次数{retryMessageContext.RetryInfo.NumberOfRetries},将在{timespan.TotalSeconds}秒后重试");
                        return Retry.In(timespan);
                    }
                    else
                    {
                        _logger.LogError(ex,$"message:{Newtonsoft.Json.JsonConvert.SerializeObject(message)},类型{typeof(TAsyncConsumer).Name}消息处理失败,重试次数{retryMessageContext.RetryInfo.NumberOfRetries},消息不再重试");
                        return new Ack();
                    }
                }
            }

            if (stopwatch.ElapsedMilliseconds > _logForMinExecutionDuration && _logger.IsEnabled(LogLevel.Information))
            {
                var log = new { MessageType = typeof(TMessage).Name, RetryTimes = retryMessageContext.RetryInfo.NumberOfRetries, Context = message ,Step = stepDic};
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(log);
                stopwatch.Stop();
                _logger.LogInformation($"{json}  LastStep:{stopwatch.ElapsedMilliseconds}");
            }
            return new Ack();
        }

    }
}
