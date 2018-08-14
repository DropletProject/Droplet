using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Droplet.Data.Services
{
    /// <summary>
    /// 查询服务
    /// </summary>
    public interface IQueryService
    {

        IEnumerable<TAny> Query<TAny>(string query,  object parameters = null) where TAny : class;

     
        Task<IEnumerable<TAny>> QueryAsync<TAny>(string query,object parameters = null) where TAny : class;
    }
}
