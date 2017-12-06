using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YunChengLK.Framework.Utility
{
    public static class Guard
    {
        public static void ArgumentNotNullOrEmpty(object value, string name)
        {
            if (null == value)
            {
                throw new ArgumentNullException(name);
            }
            if (value is string && string.Empty.Equals(value))
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
