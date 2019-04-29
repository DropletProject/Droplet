using RawRabbit;
using RawRabbit.Pipe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Droplet.RawRabbit.Test.Fake
{
    public class FakeBusClient : IBusClient
    {
        public Task<IPipeContext> InvokeAsync(Action<IPipeBuilder> pipeCfg, Action<IPipeContext> contextCfg = null, CancellationToken token = default(CancellationToken))
        {
            return Task.Run<IPipeContext>(()=> {
                return new FakePipeContext(); 
            });
        }
    }

    public class FakePipeContext : IPipeContext
    {
        public IDictionary<string, object> Properties => new Dictionary<string,object>();
    }
}
