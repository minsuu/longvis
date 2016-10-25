using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace DataVisualizer
{
    public class DBInterface
    {
        private SQLiteConnection conn;

        public DBInterface()
        {
            conn = new SQLiteConnection("Data Source=" + Env.dbpath + ";Version=3;");
            conn.Open();
        }

        public List<string> tables()
        {
            string q = @"SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
            DataTable t = GetDataTable(q);
            List<string> r = new List<string>();
            foreach (DataRow row in t.Rows)
            {
                r.Add(row.ItemArray[0].ToString());
            }
            return r;
        }

        public DataTable GetDataTable(string q)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SQLiteCommand c = new SQLiteCommand(q, conn))
                {
                    using (SQLiteDataReader r = c.ExecuteReader())
                    {
                        dt.Load(r);
                        return dt;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void execute(string q)
        {
            SQLiteCommand comm = new SQLiteCommand(q, conn);
            try
            {
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
