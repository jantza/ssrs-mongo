using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace SsrsMongo
{
    public class MongoImporter
    {
        public List<string> IgnoreKeys { get; set; }
        public string ConnectionString { get; private set; }
        public string DbName { get; private set; }
        public string CollectionName { get; private set; }   


        public MongoImporter(string connectionString, string databaseName, string collectionName)
        {
            this.IgnoreKeys = new List<string>();
            this.ConnectionString = connectionString;
            this.DbName = databaseName;
            this.CollectionName = collectionName;
        }

        public MongoImporter(string connectionString, string databaseName, string collectionName, IEnumerable<string> ignoreKeys)
            : this(connectionString, databaseName, collectionName)
        {
            this.IgnoreKeys = ignoreKeys.ToList();
        }


        public int ImportItems(IEnumerable<ReportUsageItem> items)
        {
            List<BsonDocument> documents = new List<BsonDocument>();
            foreach (var item in items)
            {
                try
                {
                    var doc = item.ToBson();

                    if (this.IgnoreKeys.Count > 0)
                    {
                        var elementsToRemove = doc.Elements.Where(x => IgnoreKeys.Contains(x.Name)).ToList();
                        elementsToRemove.ForEach(x => doc.RemoveElement(x)); 
                    }

                    documents.Add(doc);
                }
                catch (Exception ex)
                {
                    // TODO: Log to console?
                    throw;
                }
            }

            if (documents.Count > 0)
            {
                var collection = GetUsageCollection();
                collection.InsertBatch(documents); 
            }

            return documents.Count;
        }

        public DateTime? GetLastExecutionTime()
        {
            var collection = GetUsageCollection();

            var doc = collection
                .Find(null)
                .SetSortOrder(SortBy.Descending("time_start"))
                .FirstOrDefault();

            if (null != doc)
            {
                return doc["time_start"].ToLocalTime();
            }
            else
            {
                return null;
            }
        }


        private MongoCollection<BsonDocument> GetUsageCollection()
        {
            var server = new MongoClient(this.ConnectionString).GetServer();
            var database = server.GetDatabase(this.DbName);
            var collection = database.GetCollection(this.CollectionName);

            return collection;
        }
    }
}
