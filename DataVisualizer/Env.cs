using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Media;

namespace DataVisualizer
{
    static public class Env
    {
        static Env()
        {
            appPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            dateFormats = new List<string>();
            dateFormats.Add("yyyyMMdd HH:mm:ss.fff");
            dateFormats.Add("HH:mm:ss.fff");
            dateFormats.Add("mm:ss.fff");
            dateFormats.Add("mm:ss.ff");
            dateFormats.Add("mm:ss.f");

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
                            "TS BIGINT unsigned, ";
            dbTableAppend = "S{0} DOUBLE, Q{0} BIGINT, ";
        }

        public static string appPath { get; set; }
        public static string dbServerAddress { get; set; }
        public static string dbUid { get; set; }
        public static string dbPassword { get; set; }
        public static string dbPort { get; set; }
        public static string dbDatabase { get; set; }
        public static string dbBaseName { get; set; }
        public static string dbBaseScheme { get; set; }
        public static string dbTableScheme { get; set; }
        public static string dbTableAppend { get; set; }
        public static List<string> dateFormats { get; set; }

        public static SolidColorBrush colA { get; set; } = new SolidColorBrush(Color.FromRgb(0, 67, 88));
        public static SolidColorBrush colB { get; set; } = new SolidColorBrush(Color.FromRgb(31, 138, 112));

    }
}
