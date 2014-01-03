using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace SsrsMongo
{
    public class ReportUsageItem
    {
        public string USERNAME { get; set; }
        public string NAME { get; set; }
        public DateTime TIMESTART { get; set; }
        public DateTime TIMEEND { get; set; }
        public string PARAMETERS { get; set; }


        public BsonDocument ToBson()
        {
            // Standard values
            BsonDocument doc = new BsonDocument
            {
                { "name", this.NAME },
                { "username", this.USERNAME },
                { "time_start", this.TIMESTART },
                { "time_end", this.TIMEEND },
            };

            var hash = HashParameters();

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
            }

            return doc;
        }


        private Dictionary<string, List<string>> HashParameters()
        {
            var parameters = this.PARAMETERS
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
            return hash;
        }
    }
}
