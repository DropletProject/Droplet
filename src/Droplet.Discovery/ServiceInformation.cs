using System;
using System.Collections.Generic;

namespace Droplet.Discovery
{
   
    public class ServiceInformation
    {
        /// <summary>
        /// Service name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Service id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Service host
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Service port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Service version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
