using System;
using System.Configuration;

namespace SsrsMongo
{
    class Program
    {
        const int MaxDaysBack = -8;
        static readonly string[] ParametersToIgnore = new[] 
        { 
            "RESOURCE_ID", 
            "RESOURCES", 
            "Profile" 
        };

        static void Main(string[] args)
        {
            var importer = 
                new MongoImporter(
                    connectionString:   ConfigurationManager.ConnectionStrings["mongoDb"].ConnectionString,
                    databaseName:       ConfigurationManager.AppSettings["mongo_db"],
                    collectionName:     ConfigurationManager.AppSettings["mongo_collection"],
                    ignoreKeys:         ParametersToIgnore
                );

            var lastDate = importer.GetLastExecutionTime() ?? DateTime.Now.AddDays(MaxDaysBack);

            var items = 
                new ReportServer(
                    connectionString:   ConfigurationManager.ConnectionStrings["reportsServer"].ConnectionString
                ).GetItems(lastDate);            

            var documentCount = importer.ImportItems(items);

            Console.WriteLine("Processed {0} documents starting from {1}", documentCount, lastDate);
        }
    }
}
