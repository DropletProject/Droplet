using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.AutoMapper.Attributes
{
    [Flags]
    public enum MapFlag
    {
        MapTo = 1,
        MapFrom = 2
    }

    /// <summary>
    /// Map
    /// Create a bidirectional map
    /// </summary>
    public class MapAttribute :Attribute
    {
        public Type[] Target { get; set; }

        public virtual MapFlag Direction => MapFlag.MapTo | MapFlag.MapFrom;

        public MapAttribute(params Type[] target)
        {
            Target = target;
        }
    }
}
