using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YunChengLK.Framework.Data;
using YunChengLK.Framework.Data.Configuration;
using YunChengLK.Framework.Data.Core;

namespace YunChengLK.Framework.Dome
{
    public static class Database
    {
        static Database() { }

        public static SqlServer Test
        {
            get
            {
                return new SqlServer(ConnectionConfig.Connections["Test"]);
            }
        }
        public static MongoServer mongo
        {
            get
            {
                return new MongoServer(ConnectionConfig.Connections["Test"]);
            }
        }
    }
}
