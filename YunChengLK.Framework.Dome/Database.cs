using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YunChengLK.Framework.Data;
using YunChengLK.Framework.Data.Configuration;
using YunChengLK.Framework.Data.Core;
using System.Configuration;

namespace YunChengLK.Framework.Dome
{
    public static class Database
    {
        static Database() { }

        public static SqlServer Test
        {
            get
            {
                var mongourl = ConfigurationManager.AppSettings["mongodb"];
                if (string.IsNullOrEmpty(mongourl))
                    return new SqlServer(ConnectionConfig.Connections["Test"]);
                else
                    return new SqlServer(ConnectionConfig.Connections["Test"], "mongodb://39.106.117.151:27017");
            }
        }

    }
}
