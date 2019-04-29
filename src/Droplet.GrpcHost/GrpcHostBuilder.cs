using Grpc.Core;
using System;
using Grpc.Core.Interceptors;
using System.Collections.Generic;
using Droplet.GrpcHost.Interceptors;

namespace Droplet.GrpcHost
{
    public static class GrpcHost
    {
        public static GrpcHostBuilder CreateBuilder()
        {
            return new GrpcHostBuilder();
        }
    }

    public class GrpcHostBuilder
    {
        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        public string ConsulAddrSection { get; private set; } = "consulAddr";
        public string ServiceName { get; private set; }
        public string ServiceVersion { get; private set; }

        public string ConfigName { get; private set; } = "appsettings";
        public string ConfigDir { get; private set; } = "./configs";
        public int DefaultCode { get; private set; }


        public List<Func<IServiceProvider, ServerServiceDefinition>> GrpcServers { get; }

        public List<Type> InterceptorTypes { get; private set; } = new List<Type>();


        internal GrpcHostOption Option { get; private set; }

        public GrpcHostBuilder()
        {
            GrpcServers = new List<Func<IServiceProvider, ServerServiceDefinition>>();
        }

        public GrpcHostBuilder UseHost(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
            return this;
        }

        public GrpcHostBuilder UseConsul(string serviceName,string serviceVersion = "", string consulAddrSection = "")
        {
            ServiceName = serviceName;
            ServiceVersion = serviceVersion;
            if(!string.IsNullOrEmpty(consulAddrSection))
            {
                ConsulAddrSection = consulAddrSection;
            }
            return this;
        }

        public GrpcHostBuilder UseConfig(string configDir = "", string configName = "")
        {
            if (!string.IsNullOrEmpty(configDir))
                this.ConfigDir = configDir;

            if (!string.IsNullOrEmpty(configName))
                this.ConfigName = configName;

            return this;
        }

        public GrpcHostBuilder UseInterceptor<TInterceptor>() where TInterceptor : ServerInterceptor
        {
            InterceptorTypes.Add(typeof(TInterceptor));
            return this;
        }

        public GrpcHostBuilder UseGrpcServer(Func<IServiceProvider, ServerServiceDefinition> grpcServer)
        {
            GrpcServers.Add(grpcServer);
            return this;
        }

        public GrpcHostBuilder UseExceptionHandler(int defaultCode)
        {
            DefaultCode = defaultCode;
            return this;
        }
    }
}

