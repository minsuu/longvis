using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace DataVisualizer
{
    static class Env
    {
        static Env()
        {
            app = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

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
