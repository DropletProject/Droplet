using Grpc.Core;
using Grpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.ServiceProxy.Grpc
{
    public class ServiceProxyCallInvoker : CallInvoker
    {
        readonly Channel _channel;

       
        public ServiceProxyCallInvoker(Channel channel)
        {
            _channel = GrpcPreconditions.CheckNotNull(channel);
        }

       
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var call = CreateCall(method, host, options);
            return Calls.BlockingUnaryCall(call, request);
        }

       
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncUnaryCall(call, request);
        }

        
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncServerStreamingCall(call, request);
        }

       
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncClientStreamingCall(call);
        }

        
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncDuplexStreamingCall(call);
        }

        protected virtual CallInvocationDetails<TRequest, TResponse> CreateCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
                where TRequest : class
                where TResponse : class
        {
            return new CallInvocationDetails<TRequest, TResponse>(_channel, method, host, options);
        }
    }
}
