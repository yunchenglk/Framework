using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YunChengLK.Framework.Data.Core;

namespace YunChengLK.Framework.Data
{
    [Serializable]
    public class MongoServer : BaseDatabase
    {
        private string _connectionString = null;
        public MongoServer(string connectionString)
        {
            this._connectionString = connectionString;
            //this.Connection = new SqlConnection(connectionString);
        }

    }
}