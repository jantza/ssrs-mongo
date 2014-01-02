using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsrsMongo.Models;

namespace SsrsMongo
{
    public class Eater
    {
        public List<Chunk> Chunks { get; private set; }
        public List<List<KeyValuePair<string, string>>> Pieces { get; private set; }


        public Eater()
        {
            Chunks = new List<Chunk>();
            Pieces = new List<List<KeyValuePair<string, string>>>();
        }

        public void ConsumeReportItem(ReportUsageItem item)
        {
            // TODO: Do the real logic to make this happen....
            StringBuilder sb = new StringBuilder();

            sb.Append("NAME: " + item.NAME + "; ");
            sb.Append("USERNAME: " + item.USERNAME + "; ");
            sb.Append("TIMESTART: " + item.TIMESTART + "; ");
            sb.Append("TIMEEND: " + item.TIMEEND + "; ");

            var parmeterString = item.PARAMETERS.Replace("=", ": ").Replace("&", "; ");
            sb.Append(parmeterString);

            this.ConsumeLine(sb.ToString());
        }

        public void ConsumeLine(string line)
        {
            var currentList = new List<KeyValuePair<string, string>>();

            var pieces = line.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var piece in pieces)
            {
                var keyValue = piece.Split(new[] { ": " }, StringSplitOptions.None);

                if (2 != keyValue.Length)
                {
                    Console.WriteLine("Argument Count Error While Parsing: '" + piece + "'");
                    continue;
                }

                currentList.Add(new KeyValuePair<string, string>(keyValue[0], keyValue[1]));
            }

            Pieces.Add(currentList);
        }

        public string BlowChunks()
        {
            StringBuilder sb = new StringBuilder();

            // Use for proper JS object
            //"{ \"chunks\": [\r\n");

            foreach (var pieces in Pieces)
            {
                var chunk = DigestPiece(pieces);

                sb.AppendLine(chunk.Blow());
            }

            // Use for proper JS object
            //sb.Remove(sb.Length - 5, 5);
            //sb.Append("}");
            //sb.AppendLine("]}");

            var result = sb.ToString();

            return result;
        }


        private Chunk DigestPiece(List<KeyValuePair<string, string>> piece)
        {
            var chunk = new Chunk();

            foreach (var item in piece)
            {
                if (!chunk.ContainsKey(item.Key))
                {
                    chunk.Add(item.Key, new List<string> { item.Value });
                }
                else
                {
                    chunk[item.Key].Add(item.Value);
                }
            }

            return chunk;
        }
    }

    public class Chunk : Dictionary<string, List<string>>
    {
        public string Blow()
        {
            var sb = new StringBuilder("{ ");

            foreach (var entry in this)
            {
                if (entry.Value.Count > 1)
                {
                    sb.AppendFormat("\"{0}\": [\"{1}\"], ", entry.Key, string.Join("\", \"", entry.Value));
                }
                else if (entry.Value.Count == 1)
                {
                    int intValue;
                    if (int.TryParse(entry.Value[0], out intValue))
                    {
                        sb.AppendFormat("\"{0}\": {1}, ", entry.Key, intValue);
                    }
                    else
                    {
                        DateTime dt;
                        if (DateTime.TryParse(entry.Value[0], out dt))
                        {
                            sb.AppendFormat("\"{0}\": {{\"$date\": {1}}}, ", entry.Key, dt.ToUnixTime());
                        }
                        else
                        {
                            sb.AppendFormat("\"{0}\": \"{1}\", ", entry.Key, entry.Value[0]);
                        }
                    }
                }
                else
                {
                    throw new Exception("Problem blowing a chunk...");
                }
            }

            // Remove last comma...
            sb.Remove(sb.Length - 2, 2);
            sb.Append("}");

            var result = sb.ToString();

            return result;
        }
    }

    public static class FormattingExtensions
    {
        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalMilliseconds);
        }
    }
}
