using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace DataReducer
{
    static class Env
    {
        static Env()
        {
            dbServerAddress = "localhost";
            dbUid = "root";
            dbPassword = "qwer";
            dbPort = "3306";
            dbDatabase = "longvis";
            dbBaseName = "longvis_list";
            /*
            CREATE TABLE posts ( 
                  id bigint(20) unsigned NOT NULL AUTO_INCREMENT,
                  subject varchar(255) NOT NULL,
                  content mediumtext,
                  created datetime,
                  user_id int(10) unsigned NOT NULL,
                  user_name varchar(32) NOT NULL,
                  hit int(10) unsigned NOT NULL default '0',  
                  PRIMARY KEY (id)
            );
             */
            dbBaseScheme = "id BIGINT(20) unsigned NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                           "name VARCHAR(50) NOT NULL UNIQUE, " +
                           "summary TINYTEXT, " +
                           "algo TINYTEXT, " +
                           "created DATETIME, " +
                           "accessed DATETIME, " +
                           "sensor TINYINT NOT NULL, " +
                           "sensornames VARCHAR(255)";
            dbTableScheme = "id BIGINT(20) unsigned NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                            "timestamp DOUBLE, ";
            dbTableAppend = "S{0} DOUBLE, Q{0} BIGINT";
        }

        public static string dbServerAddress { get; set; }
        public static string dbUid { get; set; }
        public static string dbPassword { get; set; }
        public static string dbPort { get; set; }
        public static string dbDatabase { get; set; }
        public static string dbBaseName { get; set; }
        public static string dbBaseScheme { get; set; }
        public static string dbTableScheme { get; set; }
        public static string dbTableAppend { get; set; }

        private static string _app = "";
        public static string app
        {
            get { return _app; }
            set
            {
                _app = value;
                root = value + @"\LongVis";
                dbpath = root + @"\data.db";
            }
        }

        private static string _root = "";
        public static string root
        {
            get { return _root; }
            set
            {
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }
                _root = value;
            }
        }

        private static string _dbpath = "";
        public static string dbpath
        {
            get { return _dbpath; }
            set
            {
                if (!File.Exists(value))
                {
                    SQLiteConnection.CreateFile(value);
                }
                _dbpath = value;
            }
        }
    }
}
