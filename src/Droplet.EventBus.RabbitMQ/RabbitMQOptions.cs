using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.EventBus.RabbitMQ
{

    public class RabbitMQOptions
    {
        public List<RabbitServerSetting> Servers { get; set; }

        public int RetryCount { get; set; } = 5;
    }


    public class RabbitServerSetting
    {

        public string TypeName { get; set; }
        public string Host { get; set; }

        public int Port { get; set; }

        public string VirtualHost { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public ushort RequestedHeartbeat { get; set; }

    }
}
