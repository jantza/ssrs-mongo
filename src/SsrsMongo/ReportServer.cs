using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SsrsMongo
{
    public class ReportServer
    {
        public string ConnectionString { get; private set; }


        public ReportServer(string connectionString)
        {
            this.ConnectionString = connectionString;
        }


        public IEnumerable<ReportUsageItem> GetItems(DateTime? lastExecutionTime)
        {
            var queryText =
@"SELECT EL.USERNAME, C.NAME, EL.TIMESTART, EL.TIMEEND, EL.PARAMETERS
FROM REPORTSERVER_ESP.DBO.EXECUTIONLOG (NOLOCK) EL INNER JOIN REPORTSERVER_ESP.DBO.CATALOG (NOLOCK) C ON EL.REPORTID=C.ITEMID
WHERE(@LAST_EXECUTION_TIME IS NULL OR EL.TIMESTART > @LAST_EXECUTION_TIME)";

            using (var con = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand { CommandText = queryText, Connection = con };
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@LAST_EXECUTION_TIME",
                    Value = lastExecutionTime.HasValue ? (object)lastExecutionTime.Value : DBNull.Value
                });

                con.Open();

                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    int idx = 0;

                    yield return new ReportUsageItem
                    {
                        USERNAME = rdr.GetString(idx++),
                        NAME = rdr.GetString(idx++),
                        TIMESTART = rdr.GetDateTime(idx++),
                        TIMEEND = rdr.GetDateTime(idx++),
                        PARAMETERS = rdr.GetString(idx++)
                    };
                }
            }

        }
    }
}
