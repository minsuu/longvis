using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
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
    /*
        dbBaseScheme = "id BIGINT(20) unsigned NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                           "name VARCHAR(50) NOT NULL UNIQUE, " +
                           "summary TINYTEXT, " +
                           "algo TINYTEXT, " +
                           "created DATETIME, " +
                           "accessed DATETIME, " +
                           "sensor TINYINT NOT NULL, " +
                           "sensornames VARCHAR(255)";
    */

    public static class ConvertTime
    {
        public static string RelDateTime(DateTime dt)
        {
            var ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);
            if (delta < 60)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 120)
            {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400) // 24 * 60 * 60
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 172800) // 48 * 60 * 60
            {
                return "yesterday";
            }
            if (delta < 2592000) // 30 * 24 * 60 * 60
            {
                return ts.Days + " days ago";
            }
            if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
    public class tableInfo
    {
        public long id { get; set; }
        public int sensor { get; set; }
        public string name { get; set; }
        public string summary { get; set; }
        public string algo { get; set; }
        public string sensornames { get; set; }
        public DateTime created { get; set; }
        public DateTime accessed { get; set; }
        public string accessed_rel
        {
            get
            {
                return ConvertTime.RelDateTime(accessed);
            }
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private static MainViewModel instance;
        public static MainViewModel Instance
        {
          get 
          {
             if (instance == null)
             {
                instance = new MainViewModel();
             }
             return instance;
          }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /* Bound Properties! ! ! */
        private string _Logger;
        public string Logger
        {
            get { return _Logger; }
            set
            {
                _Logger = value;
                OnPropertyChanged("Logger");
            }
        }

        private ObservableCollection<tableInfo> _tableInfoList = new ObservableCollection<tableInfo>();
        public ObservableCollection<tableInfo> tableInfoList
        {
            get { return _tableInfoList; }
            set
            {
                _tableInfoList = value;
                OnPropertyChanged("tableInfoList");
            }
        }
        /* ENDED */
        
        private DBInterface db = new DBInterface();
        public PlotModel Plot { get; set; } = new PlotModel();
        public int Plot_width { get; set; } = 0;

        public MainViewModel()
        {
            Plot.LegendTitle = "Legend";
            try
            {
                getTableInfo();
            }catch(Exception e)
            {
                Debug.Print(e.ToString());
            }
        }

    /*
        dbBaseScheme = "id BIGINT(20) unsigned NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                           "name VARCHAR(50) NOT NULL UNIQUE, " +
                           "summary TINYTEXT, " +
                           "algo TINYTEXT, " +
                           "created DATETIME, " +
                           "accessed DATETIME, " +
                           "sensor TINYINT NOT NULL, " +
                           "sensornames VARCHAR(255)";
    */
        public void getTableInfo()
        {
            tableInfoList.Clear();
            DataTable r = db.getDataTable(string.Format("SELECT * FROM {0}", Env.dbBaseName));
            tableInfoList = new ObservableCollection<tableInfo>(
                r.AsEnumerable().Select(row => new tableInfo {
                    id = Convert.ToInt64(row["id"]),
                    name = row.Field<string>("name"),
                    summary = row.Field<string>("summary"),
                    algo = row.Field<string>("algo"),
                    created = row.Field<DateTime>("created"),
                    accessed = row.Field<DateTime>("accessed"),
                    sensor = Convert.ToByte(row["sensor"]),
                    sensornames = row.Field<string>("sensornames")
                }));
        }
        
        public void fillPlot(string table, string[] headers)
        {
            Plot.Series.Clear();
            Plot.Axes.Clear();

            Plot.Axes.Add(new DateTimeAxis
            {
                StringFormat = "mm:ss.fff",
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = DateTimeAxis.ToDouble(DateTime.FromBinary(0))
            });

            OxyPalette myPallete = OxyPalettes.Hot64;
            int i = 0;
            foreach(string header in headers)
            {
                LineSeries a = new LineSeries();
                a.Color = myPallete.Colors[i];
                if (i % 2 == 0)
                    a.StrokeThickness = 5;
                Plot.Axes.Add(new LinearAxis
                {
                    Key = header,
                    Position = ((i % 2==0)? AxisPosition.Left : AxisPosition.Right),
                    AxisDistance = (i/2)*30,                    
                    Angle = 90,
                    TicklineColor = myPallete.Colors[i],
                });
                a.YAxisKey = header;
                a.Title = header;
                string s = string.Format("SELECT TS, S{1} FROM {0} WHERE Q{1} <= 2048 ORDER BY TS", table, i);
                DataTable r = db.getDataTable(s);
                foreach(var row in r.AsEnumerable())
                {
                    DateTime ts = DateTime.FromBinary(Convert.ToInt64(row["TS"]));
                    a.Points.Add(new DataPoint(DateTimeAxis.ToDouble(ts),
                                               Convert.ToDouble(row[string.Format("S{0}", i)])));
                }
                Debug.Print(a.Points.Count().ToString());
                Plot.Series.Add(a);
                i++;
            }
        }

        /*
        public delegate void d_cache_update();
        public CacheController cc;
        public void cache_update()
        {
            cc.fill_data(cc.data_l, cc.data_r, Plot_width);
            Plot.Series.Clear();
            foreach (var a in cc.line)
                Plot.Series.Add(a);
            Plot.InvalidatePlot(true);
        }
        */

        /*
        public void Open_clicked(string name)
        {
            Plot.Title = name;
            cc = new CacheController(db, name);
            d_cache_update work = cache_update;
            work.BeginInvoke(null, null);
        }
        
        Stopwatch sw = new Stopwatch();
        Timer timer = new Timer();
        private string _log_past;

        public void Log_begin(string s, bool t = true)
        {
            sw.Restart();
            if (t)
                timer.Start();
            Logger = _log_past = s;
        }

        public void Log_update(int per)
        {
            Logger = _log_past + " (" + per + "%)";
        }

        public void Log_update()
        {
            Logger += ".";
        }

        public void Log_end()
        {
            timer.Stop();
            sw.Stop();
            Logger = "Completed! Time elapsed : " + sw.ElapsedMilliseconds / 1000.0 + "s";
        }
        */

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
