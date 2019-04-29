using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.RawRabbit.AutoSubscribe
{
    public class AutoSubscribeConfiguration
    {
        /// <summary>
        /// 需要ack的异常类型
        /// </summary>
        public Type[] NeedAckExceptionTypes { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// 重试间隔
        /// </summary>
        public Func<int, TimeSpan> RertyIntervalFunc { get; set; }

        /// <summary>
        /// 执行时长大于多少输出日志
        /// </summary>
        public int LogForMinExecutionDuration { get; set; } = 5;
    }
}
