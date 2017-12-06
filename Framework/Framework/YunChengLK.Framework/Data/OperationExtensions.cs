using System;

namespace YunChengLK.Framework.Data
{
    public static class OperationExtensions
    {
        public static bool Ascending(this IComparable obj)
        {
            return true;
        }

        public static bool Descending(this IComparable obj)
        {
            return true;
        }
    }
}
