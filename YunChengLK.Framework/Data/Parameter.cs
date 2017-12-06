using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YunChengLK.Framework.Data
{
    [Serializable]
    public class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public DbType Type { get; set; }

        public Parameter() { }
        public Parameter(string name, object value, DbType type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }
    }
}
