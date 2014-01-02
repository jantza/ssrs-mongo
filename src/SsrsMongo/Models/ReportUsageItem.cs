using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsrsMongo.Models
{
    public class ReportUsageItem
    {
        public string USERNAME { get; set; }
        public string NAME { get; set; }
        public DateTime TIMESTART { get; set; }
        public DateTime TIMEEND { get; set; }
        public string PARAMETERS { get; set; }
    }
}
