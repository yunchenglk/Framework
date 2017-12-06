using System;

namespace YunChengLK.Framework.Data
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// 数据库表名
        /// </summary>
        public string Name { get; set; }

        public TableAttribute(string name)
        {
            this.Name = name;
        }
    }
}
