using Droplet.Data.Uow;
using Droplet.GrpcHost;
using Droplet.GrpcHost.Interceptors;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.GrpcHost.Interceptors
{
    public class UnitOfWorkInterceptor: ServerInterceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWorkInterceptor(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        private IUnitOfWork CreateUnitOfWork(MethodInfo methodInfo)
        {
            var unitOfWork = ScopeServiceProvider.GetService<IUnitOfWork>();
            if (unitOfWork != null)
            {
                var unitOfWorkAttr = methodInfo.GetCustomAttribute<UnitOfWorkAttribute>();
                if (unitOfWorkAttr != null && unitOfWorkAttr.IsTransactional)
                {
                    unitOfWork.Begin(unitOfWorkAttr.IsolationLevel);
                }
            }
            return unitOfWork;
        }

        #region Handler

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ScopeServiceProvider.Current = scope;

                var unitOfWork = CreateUnitOfWork(continuation.GetMethodInfo());
                var response = await continuation(request, context);
                if (unitOfWork != null)
                    await unitOfWork.CompleteAsync();
                return response;
            }

        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ScopeServiceProvider.Current = scope;
                var unitOfWork = CreateUnitOfWork(continuation.GetMethodInfo());
                var response = await continuation(requestStream, context);
                if (unitOfWork != null)
                    await unitOfWork.CompleteAsync();
                return response;
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ScopeServiceProvider.Current = scope;
                var unitOfWork = CreateUnitOfWork(continuation.GetMethodInfo());
                await continuation(request, responseStream, context);
                if (unitOfWork != null)
                    await unitOfWork.CompleteAsync();
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ScopeServiceProvider.Current = scope;
                var unitOfWork = CreateUnitOfWork(continuation.GetMethodInfo());
                await continuation(requestStream, responseStream, context);
                if (unitOfWork != null)
                    await unitOfWork.CompleteAsync();
            }
        }

        #endregion

    }
}
