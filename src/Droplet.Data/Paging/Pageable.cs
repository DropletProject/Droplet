using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Data.Paging
{
    public abstract class Orderable
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 是否倒叙
        /// </summary>
        public bool IsDesc { get; set; }
    }
    public class Pageable: Orderable
    {
        public Pageable()
        {

        }
        public Pageable(int pageIndex,int pageSize,string orderby = null,bool isDesc = false)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            OrderBy = orderby;
            IsDesc = isDesc;
        }
        /// <summary>
        /// 第几页 从1开始
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页多少条数据
        /// </summary>
        public int PageSize { get; set; }
    }

   

    public abstract class PageableResult : Pageable
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
    }

    public class PageableResult<T> : PageableResult
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> Items { get; set; }

        public PageableResult(Pageable query)
        {
            this.PageIndex = query.PageIndex;
            this.PageSize = query.PageSize;
        }

        public PageableResult(Pageable query, int totalCount, List<T> items)
        {
            this.PageIndex = query.PageIndex;
            this.PageSize = query.PageSize;
            this.TotalCount = totalCount;
            this.Items = items;
        }

        public PageableResult(int pageIndex, int pageSize, int totalCount, List<T> items)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
            this.Items = items;
        }
    }
}
