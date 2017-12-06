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
        public MGServer(string mongoConnctionStr)
        {
            this._mongoConnctionStr = mongoConnctionStr;
            this.mongodb = MongoServer.Create(mongoConnctionStr);
            this.mongoDataBase = mongodb.GetDatabase("0359idatabase"); // 选择数据库名
            this.mongoCollection = mongoDataBase.GetCollection("User");
        }
        public void Save(T t)
        {
            string json = JsonConvert.SerializeObject(t);
            var document = BsonDocument.Parse(json);
            string ID = XY.DataAccess.ReflectHelper.GetFieldValue(t, "ID").ToString();
            document.Add("_id", new ObjectId(ID.ToString().Replace("-", "").Substring(0, 24)));
            mongoCollection.Save(document);
        }


    }
}