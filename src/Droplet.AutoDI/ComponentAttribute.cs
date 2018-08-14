using System;

namespace Droplet.AutoDI
{
    public enum LifetimeType
    {
        Transient,
        Singleton,
        Scopped
    }

    [Flags]
    public enum RegisterServiceType 
    {
        First = 1,
        Self = 2,
        Partition = 4,
        All = 8
    }

    public class ComponentAttribute :Attribute
    {
        public LifetimeType LiftTime { get; set; }
        public RegisterServiceType RegisterService { get; set; }

        public ComponentAttribute()
        {
            LiftTime = LifetimeType.Transient;
            RegisterService = RegisterServiceType.First;
        }
    }
}
