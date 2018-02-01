using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
/*
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
 * */


namespace eCertificateRest.Utils
{
    public class Mongo
    {
        /*
        public static IMongoDatabase Connect()
        {
            try
            {
                IMongoClient _client;
                IMongoDatabase _database;
                _client = new MongoClient();
                _database = _client.GetDatabase("SGRANH");
                return _database;
            }
            catch
            {
                return null;
            }
        }
        public static void SaveRequest(string operation, string user, string method,string ip)
        {
            IMongoDatabase db = Connect();
            if (db != null)
            {

                var request = new BsonDocument{
                    {"date",DateTime.Now.ToString()},
                    {"user",user},
                    {"operation",operation},
                    {"method",method},
                    {"ip",ip}
                };
                var collection = db.GetCollection<BsonDocument>("Request");
                collection.InsertOneAsync(request);
            }
        }
        public static void SaveError(string componente, string message, string stacktrace,string user,string tipo)
        {
            IMongoDatabase db = Connect();
            if (db != null)
            {
                var error = new BsonDocument{
                    {"date",DateTime.Now.ToString()},
                    {"componente",componente},
                    {"message",message},
                    {"stacktrace",stacktrace},
                    {"user",user},             
                    {"tipo",tipo}               
                };

                var collection = db.GetCollection<BsonDocument>("Error");
                collection.InsertOneAsync(error);
            }
        }
         * */
    }
}