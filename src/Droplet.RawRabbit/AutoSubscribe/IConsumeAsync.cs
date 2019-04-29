using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Droplet.RawRabbit.AutoSubscribe
{
    public interface IConsumeAsync<in TMessage>
      where TMessage : class
    {
        Task ConsumeAsync(TMessage message);
    }
}
