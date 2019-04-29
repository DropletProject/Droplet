using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.ServiceProxy.Grpc.Internal
{
    public interface IServiceNameSelector
    {
        string Get(Type type);
    }
}
