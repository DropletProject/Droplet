using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.EventBus.RabbitMQ
{
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
