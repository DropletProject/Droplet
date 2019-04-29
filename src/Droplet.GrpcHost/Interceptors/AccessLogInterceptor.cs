using Droplet.GrpcHost;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.GrpcHost.Interceptors
{
    public class AccessLogInterceptor : ServerInterceptor
    {
        private readonly ILogger _accessLogger;
        private readonly int _defaultCode;

        public AccessLogInterceptor(ILoggerFactory loggerFactory)
        {
            _accessLogger = loggerFactory.CreateLogger("accessLog");
        }

        #region Handler

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            TResponse response = null;
            Exception exception = null;
            var logHelper = new AccessLogHelper(_accessLogger);
            try
            {
                response = await continuation(request, context);
                return response;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw ;
            }
            finally
            {
                logHelper.Log(context.Peer, context.Method, request, response, exception);
            }
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var response = await continuation(requestStream, context);
            return response;
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await continuation(request, responseStream, context);
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await continuation(requestStream, responseStream, context);
        }

        #endregion

       

    }

    class AccessLogHelper
    {
        private readonly Stopwatch _stopwatch;
        private readonly ILogger _logger;
        public AccessLogHelper(ILogger logger)
        {
            _logger = logger;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Log(string sourceIp,string methodName,object request,object response,Exception ex = null)
        {
            _stopwatch.Stop();
            var log = new AccessLogModel()
            {
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ms"),
                elapsed_time = _stopwatch.ElapsedMilliseconds,
                interface_name = methodName,
                request_content = request,
                response_content = response,
                source_ip = sourceIp,
                status = 200,
                level = ex == null ? "Info" : "Error"
            };
            if(ex != null)
            {
                log.msg = ex.ToString();
                log.status = 400;
            }

            _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(log));
        }

        class AccessLogModel
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
            public long elapsed_time { get; set; }

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

            /// <summary>
            /// 错误级别
            /// </summary>
            public string level { get; set; }
        }
    }

   
}
