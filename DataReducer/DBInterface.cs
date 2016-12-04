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
        // mysqlConnection!!
        private MySqlConnection conn;
        // woking progress. 0~100.
        public double workProgress { get; set; }

        // constructor
        public DBInterface()
        {
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
                    using (var conn = new MySqlConnection(string.Format(createConn, Env.dbServerAddress, Env.dbUid, Env.dbPassword)))
                        using (var com = conn.CreateCommand())
                    {
                        conn.Open();
                        com.CommandText = string.Format(@"CREATE DATABASE IF NOT EXISTS `{0}`;", Env.dbDatabase);
                        com.ExecuteNonQuery();
                    }
                    goto retry;                                        
                }else
                {
                    MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Stop);
                    Application.Current.Shutdown();
                }
            }

            // check info table exists..
            using (var res = getDataTable(string.Format(@"SHOW TABLES LIKE '{0}'", Env.dbBaseName)))
            {
                if(res?.Rows.Count == 0)
                {
                    execute(string.Format("CREATE TABLE {0} ({1})", Env.dbBaseName, Env.dbBaseScheme));
                }
            }
            workProgress = 0;
        }

        // insertion
        public void insert(string name, CSVParser csv)
        {
            /*
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                using (var trans = conn.BeginTransaction())
                {
                    string q = "INSERT INTO [" + name + "] VALUES (NULL, ";
                    ins_max = csv.raw_len;
                    for (ins_now = 0; ins_now < ins_max; ins_now++)
                    {
                        StringBuilder qq = new StringBuilder(q);
                        foreach (string h in csv.headers)
                        {
                            qq.Append(csv.raw[h][ins_now] + ", ");
                        }
                        qq.Remove(qq.Length - 2, 2);
                        qq.Append(")");
                        cmd.CommandText = qq.ToString();
                        execute(cmd);
                        if (ins_now % 100 == 0)
                        {
                            Console.WriteLine(ins_now);
                        }
                    }
                    trans.Commit();
                }
            }*/
        }

        // tables list..
        public List<string> tables()
        {
            string q = @"SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = `{0}`";
            DataTable t = getDataTable(string.Format(q, Env.dbDatabase));
            List<string> r = new List<string>();
            if (t?.Rows == null) return r;
            foreach(DataRow row in t?.Rows)
            {
                r.Add(row.ItemArray[0].ToString());
            }
            return r;
        }

        // retrieving data
        public DataTable getDataTable(string q)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlCommand c = new MySqlCommand(q, conn))
                {
                    using (MySqlDataReader r = c.ExecuteReader())
                    {
                        dt.Load(r);
                        return dt;
                    }
                }
            }
            catch(MySqlException ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Stop);
                Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Background);
                return null;
            }
        }

        // general execution function
        public void execute(MySqlCommand comm)
        {
            try
            {
                comm.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Stop);
                Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        public void execute(string q)
        {
            MySqlCommand comm = new MySqlCommand(q, conn);
            execute(comm);
        }
    }
}
