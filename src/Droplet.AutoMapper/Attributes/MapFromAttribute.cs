using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.AutoMapper.Attributes
{
    public class MapFromAttribute : MapAttribute
    {
        public override MapFlag Direction => MapFlag.MapFrom;
        public MapFromAttribute(params Type[] types) : base(types)
        {
        }
    }
}
