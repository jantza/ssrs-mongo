using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsrsMongo.SQL;
using SsrsMongo.Mongo;

namespace SsrsMongo
{
    class Program
    {
        const int DaysBack = -8;
        static readonly string[] KeysToIgnore = new[] 
        { 
            "RESOURCE_ID", 
            "RESOURCES", 
            "Profile" 
        };

        static void Main(string[] args)
        {
            var repo = new ReportUsageRepo();

            var items = repo.GetItems(DateTime.Now.AddDays(DaysBack));

            var eater = new MongoDataEater(KeysToIgnore);

            var documentCount = eater.ConsumeData(items);

            Console.WriteLine("Processed {0} documents.", documentCount);
        }
    }
}
