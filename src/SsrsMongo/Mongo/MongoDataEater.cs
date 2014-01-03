using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SsrsMongo.Models;

namespace SsrsMongo.Mongo
{
    public class MongoDataEater
    {
        private List<string> IgnoreKeys { get; set; }


        public MongoDataEater()
        {
            this.IgnoreKeys = new List<string>();
        }

        public MongoDataEater(IEnumerable<string> ignoreKeys)
        {
            this.IgnoreKeys = ignoreKeys.ToList();
        }


        public int ConsumeData(IEnumerable<ReportUsageItem> items)
        {
            List<BsonDocument> documents = new List<BsonDocument>();
            foreach (var item in items)
            {
                try
                {
                    var doc = this.ParseDocument(item);
                    documents.Add(doc);
                }
                catch (Exception ex)
                {
                    // TODO: Log to console?
                    throw;
                }
            }

            var connectionString = ConfigurationManager.ConnectionStrings["mongoDb"].ConnectionString;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("ssrs");
            var collection = database.GetCollection("usage");
            collection.InsertBatch(documents);

            return documents.Count;
        }


        private BsonDocument ParseDocument(ReportUsageItem item)
        {
            BsonDocument doc = new BsonDocument
            {
                { "name", item.NAME},
                { "username", item.USERNAME },
                { "timestart", item.TIMESTART },
                { "timeend", item.TIMEEND },
            };

            var parameters = item.PARAMETERS
                .Replace("=", ": ")
                .Replace("&", "; ")
                .Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

            // Break everything into key/value pairs
            var keyValues = new List<KeyValuePair<string, string>>();
            foreach (var parameter in parameters)
            {
                var keyValue = parameter.Split(new[] { ": " }, StringSplitOptions.None);

                if (2 != keyValue.Length)
                {
                    Console.WriteLine("Argument Count Error While Parsing: '" + parameter + "'");
                    continue;
                }

                keyValues.Add(new KeyValuePair<string, string>(keyValue[0], keyValue[1]));
            }

            // Distill key/value pairs down to unqiue single demensional arrays or single elements
            var hash = new Dictionary<string, List<string>>();            
            foreach (var keyValue in keyValues)
            {
                if (!hash.ContainsKey(keyValue.Key))
                {
                    hash.Add(keyValue.Key, new List<string> { keyValue.Value });
                }
                else
                {
                    hash[keyValue.Key].Add(keyValue.Value);
                }
            }

            foreach (var entry in hash)
            {
                if (entry.Value.Count > 1)
                {
                    var array = new BsonArray();
                    array.AddRange(entry.Value);

                    doc.Add(new BsonElement(entry.Key, array));
                }
                else if (entry.Value.Count == 1)
                {
                    int intValue;
                    if (int.TryParse(entry.Value[0], out intValue))
                    {
                        doc.Add(entry.Key, intValue);
                    }
                    else
                    {
                        DateTime dt;
                        if (DateTime.TryParse(entry.Value[0], out dt))
                        {
                            doc.Add(entry.Key, dt);
                        }
                        else
                        {
                            doc.Add(entry.Key, entry.Value[0]);
                        }
                    }
                }
                else
                {
                    throw new Exception("Please help...");
                }
            }

            return doc;
        }
    }
}
