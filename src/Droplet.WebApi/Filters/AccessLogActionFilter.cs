using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace LG.Main.Host.Web.Filters
{
    /// <summary>
    /// 访问日志
    /// </summary>
    public class AccessLogActionFilter : IAsyncActionFilter
    {
        private readonly ILogger _logger;
        public AccessLogActionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("accessLog");
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var watch = new Stopwatch();
            watch.Start();
            ActionExecutedContext result = null;
            try
            {
                result = await next();
                Log(context, watch, result);
            }
            catch (Exception ex)
            {
                Log(context, watch, ex);
                throw;
            }
        }

        private void Log(ActionExecutingContext context, Stopwatch watch, ActionExecutedContext result)
        {
            watch.Stop();
            object response_content = null;
            if (result.Result is ObjectResult objectResult)
            {
                response_content = objectResult.Value;
            }
            var log = new AccessLogModel()
            {
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ms"),
                elapsed_time = watch.Elapsed.TotalMilliseconds,
                interface_name = context.ActionDescriptor.DisplayName,
                request_content = context.ActionArguments,
                response_content = response_content,
                source_ip = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                status = 200
            };
            _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(log));
        }

        private void Log(ActionExecutingContext context, Stopwatch watch, Exception exception)
        {
            watch.Stop();
            object response_content = null;
            var log = new AccessLogModel()
            {
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ms"),
                elapsed_time = watch.Elapsed.TotalMilliseconds,
                interface_name = context.ActionDescriptor.DisplayName,
                request_content = context.ActionArguments,
                response_content = response_content,
                source_ip = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                status = 500,
                msg = exception.ToString(),
            };
            _logger.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(log));
        }
    }

    internal class AccessLogModel
    {
        /// <summary>
        /// 日志产生时间
        /// </summary>
        public string time { get; set; }

        /// <summary>
        /// 接口调用返回码，返回码意义参照全局错误代码
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 接口调用耗时，ms为单位
        /// </summary>
        public double elapsed_time { get; set; }

        /// <summary>
        /// 调用方服务名称
        /// </summary>
        public string source_srv { get; set; }

        /// <summary>
        /// 调用方来源ip
        /// </summary>
        public string source_ip { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        public string interface_name { get; set; }

        /// <summary>
        /// 请求体内容，可以细化为更详细的字段
        /// </summary>
        public object request_content { get; set; }

        /// <summary>
        /// 响应体内容，可以细化为更详细的字段
        /// </summary>
        public object response_content { get; set; }

        /// <summary>
        ///  消息体，此字段作为扩展字段，可以为空，也可以存放发生异常时一些错误堆栈信息之类的
        /// </summary>
        public string msg { get; set; }
    }
}
