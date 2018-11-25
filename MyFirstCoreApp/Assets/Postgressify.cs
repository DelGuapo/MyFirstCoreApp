using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Data.SqlClient;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using Npgsql;

namespace MyFirstCoreApp
{
    public class Postgressify
    {
        // this is a shortcut for your connection string
        public static string DatabaseConnectionString;
        public static int SQLTimeout;
        public Boolean KeepConnectionOpen;
        private NpgsqlConnection Connection;
        public Postgressify(string cnString, Boolean keepOpen = false, int timeout = 5400 )
        {
            DatabaseConnectionString = cnString;
            KeepConnectionOpen = keepOpen;
            SQLTimeout = timeout;
            Connection = new NpgsqlConnection(DatabaseConnectionString);

            if (KeepConnectionOpen)
            {
                Connection.Open();
            }

        }

        public void Disconnect()
        {
            Connection.Close();
        }

        // this is for just executing sql command with no value to return
        public Boolean SqlExecute(string sql, List<NpgsqlParameter> sqlParams = null)
        {
            try
            {
                /* USE LOCAL CONNECTION */
                using (Connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, Connection);

                    if (sqlParams != null)
                    {
                        foreach (NpgsqlParameter param in sqlParams)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    if (!KeepConnectionOpen) { cmd.Connection.Open(); }
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception err)
            {
                // log error
            }
            return false;
        }

        // with this you will be able to return a value
        public object SqlReturn(string sql, List<NpgsqlParameter> sqlParams = null)
        {
            try
            {
                using (Connection)
                {

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, Connection);
                    if (sqlParams != null)
                    {
                        foreach (NpgsqlParameter param in sqlParams)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    if (!KeepConnectionOpen) { cmd.Connection.Open(); }
                    object result = (object)cmd.ExecuteScalar();
                    return result;
                }
            }
            catch(Exception err)
            {
                // LOG ERROR
            }
            return new object();

        }

        // with this you can retrieve an entire table or part of it
        public DataSet SqlDataSet(string sql, List<NpgsqlParameter> sqlParams = null)
        {
            DataSet dataset = new DataSet();
            try
            {
                using (Connection)
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
                    adapter.SelectCommand = new NpgsqlCommand(sql, Connection);
                    if (sqlParams != null)
                    {
                        foreach (NpgsqlParameter param in sqlParams)
                        {
                            adapter.SelectCommand.Parameters.Add(param);
                        }
                    }
                    adapter.SelectCommand.CommandTimeout = SQLTimeout;
                    if (!KeepConnectionOpen) { Connection.Open(); }
                    adapter.Fill(dataset);
                    if (!KeepConnectionOpen) { Connection.Close(); }
                }
            }
            catch(Exception err)
            {
                // log error
            }
            return dataset;
        }


        public DataTable SqlDataTable(string sql, List<NpgsqlParameter> sqlParams = null)
        {
            try
            {
                using (Connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, Connection);
                    if (!KeepConnectionOpen) { cmd.Connection.Open(); }
                    if (sqlParams != null)
                    {
                        foreach (NpgsqlParameter param in sqlParams)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    cmd.CommandTimeout = SQLTimeout;
                    DataTable TempTable = new DataTable();
                    TempTable.Load(cmd.ExecuteReader());
                    return TempTable;
                }
            } catch(Exception err)
            {
                // log error
            }
            return new DataTable();
        }

        public static Int64 bulkUploadText(string filePath, string tableName, Int64 skipRows = 0)
        {
            //https://stackoverflow.com/questions/38652832/bulk-c-sharp-datatable-to-postgresql-table
            DataTable dt = new DataTable();
            DataTable rtn = new DataTable();
            string delim = "";
            Int64 nRows = 0;
            int tmpCount = 0;

            StreamReader sr = File.OpenText(filePath);
            while (sr.Peek() >= 0)
            {
                if (nRows < skipRows) continue;
                tmpCount++;
                string sTemp = sr.ReadLine();
                if (delim == "")
                {
                    if (sTemp.Split(',').Length > 55) delim = ",";
                    if (sTemp.Split(';').Length > 55) delim = ";";
                    if (sTemp.Split('|').Length > 55) delim = "|";
                    if (sTemp.Split('\t').Length > 55) delim = "\t";
                }
                string[] sRecord = sTemp.Split(delim.ToCharArray());

                DataRow rw = rtn.NewRow();
                for (int i = 0; i < rtn.Columns.Count; i++)
                {
                    if (i >= sRecord.Length)
                    {
                        rw[i] = DBNull.Value;
                    }
                    else if (sRecord[i].Replace("\"", "") == "")
                    {
                        rw[i] = DBNull.Value;
                    }
                    else
                    {
                        rw[i] = sRecord[i].ToString().Replace("\"", "");
                    }
                }
                if (tmpCount >= 100)
                {
                    Console.WriteLine("Row " + rtn.Rows.Count + " inserted into staging table");
                    tmpCount = 0;
                }
                nRows++;
                rtn.Rows.Add(rw);

            }
            if (rtn.Rows.Count > 0)
            {
                if (bulkInsert(rtn, tableName))
                {
                    Console.WriteLine("Data inserted successfully");
                }
                else
                {
                    Console.WriteLine("Error inserting Data");
                }

            }
            else
            {
                Console.WriteLine("No Data Found");
            }
            return nRows;
        }
        public static Boolean bulkInsert(DataTable tbl, string tableName)
        {
            SqlBulkCopy bulkcopy = new SqlBulkCopy(DatabaseConnectionString);
            bulkcopy.DestinationTableName = tableName;
            try
            {
                bulkcopy.BulkCopyTimeout = 1800;
                bulkcopy.WriteToServer(tbl);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("**** [ERROR BULK INSERT] ****");
                Console.WriteLine(ex.Message.ToString());
                return false;
            }

        }
    }
}