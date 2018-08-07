using System;

namespace Droplet.AutoDI
{
    public enum LifetimeType
    {
        Transient,
        Singleton,
        Scopped
    }

    public enum RegisterServiceType
    {
        FirstOne,
        All
    }

    public class ComponentAttribute :Attribute
    {
        public LifetimeType LiftTime { get; set; }
        public RegisterServiceType RegisterService { get; set; }
        public string Name { get; set; }

        public ComponentAttribute()
        {
            LiftTime = LifetimeType.Transient;
            RegisterService = RegisterServiceType.FirstOne;
        }
    }
}
