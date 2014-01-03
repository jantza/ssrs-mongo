using System;

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
            var importer = new MongoImporter(ParametersToIgnore);

            var lastDate = importer.GetLastExecutionTime() ?? DateTime.Now.AddDays(MaxDaysBack);

            var items = new ReportServer().GetItems(lastDate);            

            var documentCount = importer.ImportItems(items);

            Console.WriteLine("Processed {0} documents starting from {1}", documentCount, lastDate);
        }
    }
}
