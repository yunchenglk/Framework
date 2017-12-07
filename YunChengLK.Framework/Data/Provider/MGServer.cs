using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using YunChengLK.Framework.Data.Core;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace YunChengLK.Framework.Data
{
    [Serializable]
    public class MGServer<T>
    {
        private string _mongoConnctionStr = null;
        private MongoServer mongodb = null;
        private MongoCollection mongoCollection = null;
        private MongoDatabase mongoDataBase = null;
        public MGServer(string mongoConnctionStr, string dbname)
        {
            this._mongoConnctionStr = mongoConnctionStr;
            this.mongodb = MongoServer.Create(this._mongoConnctionStr);
            this.mongoDataBase = mongodb.GetDatabase(dbname); // 选择数据库名
        }
        public void Save(T t)
        {
            mongoCollection = mongoDataBase.GetCollection(typeof(T).Name);
            string json = JsonConvert.SerializeObject(t);
            var document = BsonDocument.Parse(json);
            string ID = XY.DataAccess.ReflectHelper.GetFieldValue(t, "ID").ToString();
            document.Add("_id", new ObjectId(ID.ToString().Replace("-", "").Substring(0, 24)));
            mongoCollection.Save(document);
        }


    }
}