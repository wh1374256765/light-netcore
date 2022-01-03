using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Server
{
    /// <summary>
    /// 通用分页信息类
    /// </summary>
    public class PageModel<T>
    {
        /// <summary>
        /// 当前页标
        /// </summary>
        public int pageIndex { get; set; } = 1;
        /// <summary>
        /// 总页数
        /// </summary>
        public int pageCount { get; set; } = 6;
        /// <summary>
        /// 数据总数
        /// </summary>
        public int dataCount { get; set; } = 0;
        /// <summary>
        /// 每页大小
        /// </summary>
        public int pageSize { set; get; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> data { get; set; }
    }
}
