using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Discovery.Selectors
{
    /// <summary>
    /// 服务选取策略
    /// </summary>
    public interface IServiceSelector
    {
        ServiceInformation SelectAsync(IEnumerable<ServiceInformation> services);
    }
}
