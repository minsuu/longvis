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

namespace DataVisualizer
{
    public class DBInterface
    {
        // constructor
        public DBInterface()
        {
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
            Debug.Print("table_exists!!");
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

        /// <summary>
        /// 쿼리 q에 맞는 DataTable을 가져온다
        /// </summary>
        /// <param name="q">쿼리문</param>
        /// <returns></returns>
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

        /// <summary>
        /// 쿼리 q를 수행한다.
        /// </summary>
        /// <param name="q"></param>
        public void execute(string q)
        {
            using(var conn = create_conn()) {
                if (conn == null) return;
                MySqlCommand comm = new MySqlCommand(q, conn);
                comm.ExecuteNonQuery();
            }
        }
    }
}
