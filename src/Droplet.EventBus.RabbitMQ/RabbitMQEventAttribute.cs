using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.EventBus.RabbitMQ
{
    public class RabbitMQEventAttribute : Attribute
    {
        public RabbitMQEventAttribute(string queue,string exchange, string routingKey = "#", string typeName = "Default")
        {
            Exchange = exchange;
            Queue = queue;
            RoutingKey = routingKey;
            TypeName = typeName;
        }
        public string TypeName { get; set; }

        public string Exchange { get; set; }

        public string Queue { get; set; }

        public string RoutingKey { get; set; }
    }
}
