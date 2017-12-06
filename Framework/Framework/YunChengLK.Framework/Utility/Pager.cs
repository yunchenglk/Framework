using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YunChengLK.Framework.Utility
{
    public static class Pager
    {
        /// <summary>
        /// 计算分页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        public static void Compute(ref int pageIndex, ref int pageSize)
        {
            int page = pageIndex;
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 1;
            pageIndex = pageSize * (pageIndex - 1);
            pageSize = pageSize * page;
        }
    }
}
