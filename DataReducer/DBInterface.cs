using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Diagnostics;

namespace DataReducer
{
    public class DBInterface
    {
        // woking progress. 0~100.
        public int workProgress {
            get
            {
                return ins_iter * 100 / ins_max;
            }
        }

        // constructor
        public DBInterface()
        {
            ins_iter = 0; ins_max = 1;
        }
        
        private MySqlConnection create_conn() {
            MySqlConnection conn = null;
            retry:
            string strConn = @"Server={0};Database={1};Uid={2};Pwd={3};";
            try
            {
                conn = new MySqlConnection(string.Format(strConn, Env.dbServerAddress, Env.dbDatabase, Env.dbUid, Env.dbPassword));
                conn.Open();
            }catch (MySqlException ex)
            {
                MySqlException inner = ex.InnerException as MySqlException;
                // no database!! create!!
                if(inner?.Number == 1049)
                {
                    string createConn = @"Server={0};Uid={1};Pwd={2};";
                    using (conn = new MySqlConnection(string.Format(createConn, Env.dbServerAddress, Env.dbUid, Env.dbPassword)))
                        using (var com = conn.CreateCommand())
                    {
                        conn.Open();
                        com.CommandText = string.Format(@"CREATE DATABASE IF NOT EXISTS `{0}`;", Env.dbDatabase);
                        com.ExecuteNonQuery();
                    }
                    goto retry;                                        
                }
                throw;
            }
            return conn;
        }

        public bool table_exists(string s)
        {
            using (var res = getDataTable(string.Format(@"SHOW TABLES LIKE '{0}'", s)))
                if (res?.Rows.Count == 0)
                    return false;
            return true;
        }

        public int table_count(string s)
        {
            try
            {
                using (var res = getDataTable(string.Format(@"SELECT count(*) FROM {0}", s)))
                    if (res?.Rows.Count == 1)
                        return res.Rows[0].Field<int>(0);
                return 0;
            }catch
            {
                return 0;
            }
        }

        // insertion
        private int ins_iter;
        private int ins_max;
        public void insert(string name, List<long> T, List<List<double>> S, List<long[]> Q)
        {
            try
            {
                using (var conn = create_conn())
                using (var cmd = new MySqlCommand())
                {
                    if (conn == null) return;
                    cmd.Connection = conn;
                    ins_max = T.Count;

                    cmd.CommandText = string.Format("LOCK TABLE `{0}` WRITE", name);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format("ALTER TABLE `{0}` DISABLE KEYS", name);
                    cmd.ExecuteNonQuery();
                    string qq = string.Format("INSERT INTO `{0}` VALUES ", name);
                    List<string> batch = new List<string>();
                    for (ins_iter = 0; ins_iter < ins_max; ins_iter++)
                    {
                        StringBuilder each = new StringBuilder(string.Format("(NULL, {0}, ", T[ins_iter]/TimeSpan.TicksPerMillisecond));
                        for (int i = 0; i < S.Count; i++)
                            each.AppendFormat("{0:0.#####}, {1}, ", S[i][ins_iter], Q[i][ins_iter]);
                        each.Remove(each.Length - 2, 2);
                        each.Append(")");
                        batch.Add(each.ToString());

                        if (ins_iter == ins_max - 1 || ins_iter % 10000 == 9999)
                        {
                            string s = qq + string.Join(",", batch);
                            cmd.CommandText = s;
                            cmd.ExecuteNonQuery();
                            batch.Clear();
                        }
                    }
                    cmd.CommandText = string.Format("ALTER TABLE `{0}` ENABLE KEYS", name);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format("UNLOCK TABLES");
                    cmd.ExecuteNonQuery();
                }
            }catch(Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        // retrieving data
        public DataTable getDataTable(string q)
        {
            DataTable dt = new DataTable();
            using (var conn = create_conn())
            using (MySqlCommand c = new MySqlCommand(q, conn))
            {
                using (MySqlDataReader r = c.ExecuteReader())
                {
                    dt.Load(r);
                    return dt;
                }
            }
        }

        public void insert(string tableName, params object[] list)
        {
            using (var conn = create_conn())
            using (var comm = conn?.CreateCommand())
            {
                if (conn == null) return;
                StringBuilder query = new StringBuilder(string.Format(@"INSERT INTO `{0}` VALUES (", tableName));
                for (int i = 0; i < list.Length; i++)
                {
                    query.AppendFormat(@"@V{0}, ", i);
                }
                query.Remove(query.Length - 2, 2);
                query.Append(")");
                comm.CommandText = query.ToString();

                for (int i = 0; i < list.Length; i++)
                {
                    comm.Parameters.AddWithValue(string.Format(@"@V{0}", i), list[i]);
                }
                Debug.Print(comm.CommandText);
                execute(comm);
            }
        }
        // general execution function
        public void execute(MySqlCommand comm)
        {
            comm.ExecuteNonQuery();
        }

        public void execute(string q)
        {
            using(var conn = create_conn()) {
                if (conn == null) return;
                MySqlCommand comm = new MySqlCommand(q, conn);
                execute(comm);
            }
        }
    }
}
