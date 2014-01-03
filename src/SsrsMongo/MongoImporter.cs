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
        private List<string> IgnoreKeys { get; set; }


        public MongoImporter()
        {
            this.IgnoreKeys = new List<string>();
        }

        public MongoImporter(IEnumerable<string> ignoreKeys)
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


        private static MongoCollection<BsonDocument> GetUsageCollection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["mongoDb"].ConnectionString;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("ssrs");
            var collection = database.GetCollection("usage");

            return collection;
        }
    }
}
