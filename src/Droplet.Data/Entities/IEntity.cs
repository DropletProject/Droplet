using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Data.Entities
{
    /// <summary>
    /// 实体
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// 带主键的实体
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IEntity<TPrimaryKey> :IEntity
    {

        TPrimaryKey Id { get; set; }

        bool IsTransient();

    }

   
}
