using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.GrpcHost.Interceptors
{
    public class ExceptionHandlerInterceptor : ServerInterceptor
    {
        private readonly ILogger _logger;
        private readonly int _defaultCode;


        public ExceptionHandlerInterceptor(ILoggerFactory loggerFactory, int defaultCode)
        {
            _defaultCode = defaultCode;
            _logger = loggerFactory.CreateLogger<ExceptionHandlerInterceptor>();
        }

        #region Handler

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                throw ExceptionHandlerHelper.BuildRpcException(ex, _defaultCode);
            }

        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(requestStream, context);
            }
            catch (Exception ex)
            {
                throw ExceptionHandlerHelper.BuildRpcException(ex, _defaultCode);
            }
           
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                await continuation(request, responseStream, context);
            }
            catch (Exception ex)
            {
                throw ExceptionHandlerHelper.BuildRpcException(ex, _defaultCode);
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                await continuation(requestStream, responseStream, context);
            }
            catch (Exception ex)
            {
                throw ExceptionHandlerHelper.BuildRpcException(ex, _defaultCode);
            }
        }

        #endregion



    }

    class ExceptionHandlerHelper
    {
        public static RpcException BuildRpcException(Exception ex,int defaultCode = default)
        {
            if (ex is RpcException rpcEx)
            {
                return rpcEx;
            }

            int code = ParseCode(ex, defaultCode);
            var err = new ErrorModel() {
                Code = code,
                Detail = ex.Message,
                Internal = ex.ToString(),
                Status = (int)StatusCode.Internal,
            };
            return new RpcException(new Status(StatusCode.Internal,Newtonsoft.Json.JsonConvert.SerializeObject(err)), ex.Message);
        }

        private static int ParseCode(Exception ex,int defaultCode)
        {
            var codeProperty = ex.GetType().GetProperty("Code");
            if(codeProperty != null && codeProperty.PropertyType == typeof(int))
            {
                return (int)codeProperty.GetValue(ex);
            }

            return defaultCode;
        }

        class ErrorModel
        {
            public int Code { get; set; }
            public int Status { get; set; }
            public string Detail { get; set; }
            public string Internal { get; set; }
            public string Content { get; set; }
        }

    }
}
