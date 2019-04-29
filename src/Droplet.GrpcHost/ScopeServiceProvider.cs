using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Droplet.GrpcHost
{

    public interface IScopeServiceProvider
    {

        T GetService<T>();

        IEnumerable<T> GetServices<T>();
    }

    public class DefaultScopeServiceProvider : IScopeServiceProvider
    {

        public T GetService<T>()
        {
            return ScopeServiceProvider.GetService<T>();
        }

        public IEnumerable<T> GetServices<T>()
        {
            return ScopeServiceProvider.GetServices<T>();
        }
    }

    public static class ScopeServiceProvider
    {
        
        static AsyncLocal<IServiceScope> AsyncScope = new AsyncLocal<IServiceScope>();

        public static IServiceScope Current {
            get {
                return AsyncScope.Value;
            }
            set
            {
                AsyncScope.Value = value;
            }
        }

        public static IServiceProvider CurrentServiceProvider
        {
            get
            {
                return Current.ServiceProvider;
            }
        }


        public static T GetService<T>()
        {
            return CurrentServiceProvider.GetService<T>();
        }

        public static IEnumerable<T> GetServices<T>()
        {
            return CurrentServiceProvider.GetServices<T>();
        }

    }


}
