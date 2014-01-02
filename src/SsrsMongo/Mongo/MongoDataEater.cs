using System;
using System.Collections.Generic;
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
        public void ConsumeData(IEnumerable<ReportUsageItem> items)
        {
            foreach (var item in items)
            {
                BsonDocument doc = new BsonDocument
                {
                    { "name", item.NAME},
                    { "timestart", item.TIMESTART }
                };

                
            }
        }
    }
}
