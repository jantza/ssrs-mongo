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

            var repo = new ReportUsageRepo();
            foreach (var item in repo.GetItems(DateTime.Now.AddDays(-8)))
            {
                eater.ConsumeReportItem(item);
            }

            var json = eater.BlowChunks();

            var desktop = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var outputfile = Path.Combine(desktop, "output.json");

            File.WriteAllText(outputfile, json);
        }
    }
}
