using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using YunChengLK.Framework.Data.Core;
using MongoDB.Driver;
using Newtonsoft.Json;
using YunChengLK.Framework.Logging;

namespace YunChengLK.Framework.Data
{
    [Serializable]
    public class MGServer<T>
    {
        private MongoClient mongoClient = null;
        private IMongoDatabase database = null;

        public MGServer(string mongoConnctionStr, string dbname)
        {
            MongoUrl mongoUrl = new MongoUrl(mongoConnctionStr);
            mongoClient = new MongoClient(mongoUrl);
            database = mongoClient.GetDatabase(dbname);
        }
        public void insert(T t)
        {
            var collection = database.GetCollection<BsonDocument>(typeof(T).Name);
            string json = JsonConvert.SerializeObject(t);
            var document = BsonDocument.Parse(json);
            string ID = XY.DataAccess.ReflectHelper.GetFieldValue(t, "ID").ToString();
            var id = new ObjectId(ID.ToString().Replace("-", "").Substring(0, 24));
            document.Add("_id", id);
            collection.InsertOne(document);
            Logger.Info("MongoInsert:" + json);
        }
        public void insertList(List<T> list)
        {
            var collection = database.GetCollection<BsonDocument>(typeof(T).Name);
            List<BsonDocument> listV = new List<BsonDocument>();
            foreach (var item in list)
            {
                string json = JsonConvert.SerializeObject(item);
                var document = BsonDocument.Parse(json);
                string ID = XY.DataAccess.ReflectHelper.GetFieldValue(item, "ID").ToString();
                document.Add("_id", new ObjectId(ID.ToString().Replace("-", "").Substring(0, 24)));
                listV.Add(document);
                Logger.Info("MongoInsertList:" + json);
            };
            collection.InsertMany(listV);
        }
        public void Save(T t)
        {
            var collection = database.GetCollection<BsonDocument>(typeof(T).Name);
            string json = JsonConvert.SerializeObject(t);
            var document = BsonDocument.Parse(json);
            string ID = XY.DataAccess.ReflectHelper.GetFieldValue(t, "ID").ToString();
            var id = new ObjectId(ID.ToString().Replace("-", "").Substring(0, 24));
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            document.Add("_id", id);
            var resut = collection.DeleteOne(filter);
            if (resut.DeletedCount == 1)
            {
                collection.InsertOne(document);
                Logger.Info("MongoUpdate:" + json);
            }
            else
                Logger.Error("MongoUpdate:" + json);
        }
        public void Delete(Guid ID)
        {
            var collection = database.GetCollection<BsonDocument>(typeof(T).Name);
            var id = new ObjectId(ID.ToString().Replace("-", "").Substring(0, 24));
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var resut = collection.DeleteOne(filter);
            Logger.Info("MongoDelete:" + resut);
        }
    }
}