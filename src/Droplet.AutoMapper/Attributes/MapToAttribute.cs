using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.AutoMapper.Attributes
{
    public class MapToAttribute : MapAttribute
    {
        public override MapFlag Direction => MapFlag.MapTo;

        public MapToAttribute(params Type[] types) : base(types)
        {
        }
    }
}
