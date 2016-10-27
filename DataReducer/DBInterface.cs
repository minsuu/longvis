using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace DataReducer
{
    public class DBInterface
    {
        private SQLiteConnection conn;
        public int ins_max { get; set; }
        public int ins_now { get; set; }

        public void insert(string name, CSVParser csv)
        {
            using (var cmd = new SQLiteCommand(conn))
            {
                using (var trans = conn.BeginTransaction())
                {
                    string q = "INSERT INTO [" + name + "] VALUES (NULL, ";
                    ins_max = csv.raw_len;
                    for (ins_now = 0; ins_now < ins_max; ins_now++)
                    {
                        StringBuilder qq = new StringBuilder(q);
                        foreach(string h in csv.headers)
                        {
                            qq.Append(csv.raw[h][ins_now] + ", ");
                        }
                        qq.Remove(qq.Length - 2, 2);
                        qq.Append(")");
                        cmd.CommandText = qq.ToString();
                        cmd.ExecuteNonQuery();
                        if(ins_now % 100 == 0)
                        {
                            Console.WriteLine(ins_now);
                        }
                    }
                    trans.Commit();
                }
            }
        }

        public DBInterface()
        {
            conn = new SQLiteConnection("Data Source=" + Env.dbpath + ";Version=3;");
            conn.Open();
            ins_max = 1; ins_now = 0;
        }

        public List<string> tables()
        {
            string q = @"SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
            DataTable t = GetDataTable(q);
            List<string> r = new List<string>();
            foreach(DataRow row in t.Rows)
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
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string CreateTable(string name, CSVParser csv)
        {
            StringBuilder q = new StringBuilder("CREATE TABLE " + name + "(id INTEGER PRIMARY KEY AUTOINCREMENT, ");
            foreach(string h in csv.headers)
            {
                q.Append(h + " double, ");
            }
            q.Remove(q.Length - 2, 2);
            q.Append(")");
            execute(q.ToString());
            return q.ToString();
        }

        public void execute(string q)
        {
            SQLiteCommand comm = new SQLiteCommand(q, conn);
            try
            {
                comm.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
