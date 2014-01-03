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
        static void Main(string[] args)
        {
            var repo = new ReportUsageRepo();

            var items = repo.GetItems(DateTime.Now.AddDays(-8));

            var eater = new MongoDataEater();
            var documentCount = eater.ConsumeData(items);

            Console.WriteLine("Processed {0} documents.", documentCount);
        }
    }
}
