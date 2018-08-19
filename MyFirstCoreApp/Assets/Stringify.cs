using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;

namespace MyFirstCoreApp
{
    public class Stringify
    {
        public string datatable(DataTable dt)
        {
            string JSONresult = JsonConvert.SerializeObject(dt);
            return JSONresult;
        }
        public IEnumerable<Dictionary<string, object>> ToDictionary(DataTable table)
        {
            string[] columns = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            IEnumerable<Dictionary<string, object>> result = table.Rows.Cast<DataRow>()
                    .Select(dr => columns.ToDictionary(c => c, c => dr[c]));
            return result;
        }
    }
}
