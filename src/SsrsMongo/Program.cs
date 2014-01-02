using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsrsMongo.SQL;

namespace SsrsMongo
{
    class Program
    {
        static void Main(string[] args)
        {
            var eater = new Eater();

            using (var repo =  new ReportUsageRepo())
            {
                foreach (var item in repo.GetItems(DateTime.Now.AddDays(-8)))
                {
                    eater.ConsumeReportItem(item);
                }                
            }

            var json = eater.BlowChunks();

            File.WriteAllText(@"C:\Users\sichj\Desktop\output.json", json);
        }
    }
}
