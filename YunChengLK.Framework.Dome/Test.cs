using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YunChengLK.Framework.Data;

namespace YunChengLK.Framework.Dome
{
    [Serializable]
    [Table("Test")]
    public class Test : IEntity
    {
        [Column("ID", DbType.Guid)]
        public Guid ID { get; set; }

        [Column("Name", DbType.String)]
        public string Name { get; set; }
        [Column("pwd", DbType.String)]
        public string pwd { get; set; }
    }
}
