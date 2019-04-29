using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.RawRabbit.MultipleConnection
{

    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionAttribute : Attribute
    {
        public ConnectionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        
    }
}
