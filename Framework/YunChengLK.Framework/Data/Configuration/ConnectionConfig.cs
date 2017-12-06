using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YunChengLK.Framework.Utility;
using YunChengLK.Framework.Serialization;

namespace YunChengLK.Framework.Data.Configuration
{
    public static class ConnectionConfig
    {
        static ConnectionConfig()
        {
            Initialize();
        }
        public static void Initialize()
        {
            Databases db = XmlSerialize.DeserializeXmlFile<Databases>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs\\Database.config"));
            Singleton<Databases>.SetInstance(db);
        }

        public static Databases Connections
        {
            get
            {
                return Singleton<Databases>.GetInstance();
            }
        }
    }
}
