using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YunChengLK.Framework.Utility
{
    public static class Guider
    {
        /// <summary>GUID长度
        /// </summary>
        static int guidLength = 36;

        /// <summary>获取新的GUID
        /// </summary>
        /// <returns></returns>
        public static Guid GetNewGuid
        {
            get { return Guid.NewGuid(); }
        }

        /// <summary>得到空的Guid
        /// </summary>
        /// <returns></returns>
        public static Guid EmptyGuid
        {
            get { return Guid.Empty; }
        }

        /// <summary>GUID是否为空
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsEmpty(this Guid guid) 
        {
            return guid == Guid.Empty;
        }

        /// <summary>字符串转换GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Guid? ToGuidOrNull(this string guid) 
        {
            Guid convertGuid;
            if (!string.IsNullOrEmpty(guid) 
                && guid.Length == guidLength 
                && Guid.TryParse(guid, out convertGuid))
            {
                return convertGuid;
            }
            return null;
        }
    }
}
