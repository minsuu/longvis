using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using CsvHelper;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;
using System.Data;

namespace DataVisualizer
{
    public class CacheController
    {
        public int load_max { get; set; } = 1;
        public int load_now { get; set; } = 0;
        public List<LineSeries> line { get; set; }
        public List<string> col;
        private DBInterface db;
        public string name;

        public CacheController(DBInterface _db, string _name)
        {
            db = _db; name = _name;
            line = new List<LineSeries>();
            col = new List<string>();
            var cl = db.GetDataTable("PRAGMA table_info(" + name + ")");
            for (int i = 0; i < cl.Rows.Count; i++)
            {
                string s = cl.Rows[i].ItemArray[1].ToString();
                if (s != "id" && s != "date_time")
                    col.Add(s);
            }
            Debug.Print("aa");
            // 
        }

        /*
        public void open_table(string name)
        {
            LineSeries a;
            
            using (Timer upd = new Timer())
            {
                var cnt = db.GetDataTable("SELECT count(id) FROM " + name);
                load_max = Convert.ToInt32(cnt.Rows[0].ItemArray[0]);
                var data = db.GetDataTable("SELECT * FROM " + name);
                line.Points.Clear();
                for(load_now = 0; load_now < load_max; load_now++)
                {
                    line.Points.Add(new DataPoint(load_now, (double)data.Rows[load_now].ItemArray[3]));
                }
            }
            Plot.InvalidatePlot(true);
        }

        public void fill_data(int l, int r, int width)
        {
        }*/
    }
}