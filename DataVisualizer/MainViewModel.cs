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
using System.Windows.Media;

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

    public class axesInfo
    {
        public string name { get; set; }
        public Brush col { get; set; }
        public bool IsSelected { get; set; }
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
        private string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

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

        private ObservableCollection<axesInfo> _axesInfoList = new ObservableCollection<axesInfo>();
        public ObservableCollection<axesInfo> axesInfoList
        {
            get { return _axesInfoList; }
            set
            {
                _axesInfoList = value;
                OnPropertyChanged("axesInfoList");
            }
        }

        /* ENDED */

        private DBInterface db = new DBInterface();
        public PlotModel Plot { get; set; } = new PlotModel();

        public MainViewModel()
        {
            Plot.LegendTitle = "범례";
            Plot.LegendBackground = OxyColors.White;
            Plot.LegendBorder = OxyColors.Black;
            Plot.LegendBorderThickness = 2;
            Plot.Updating += delegate { Debug.Print("on Updating"); };
            Plot.Updated += delegate { Debug.Print("updated!"); };
            try
            {
                getTableInfo();
            } catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
            string[] pal = { "#4D4D4D", "#5DA5DA", "#FAA43A", "#60BD68", "#F17CB0", "#B2912F", "#B276B2", "#DECF3F", "#F15854" };
            List<OxyColor> poxy = new List<OxyColor>();
            for(int i=0;i<10;i++)
                foreach (var p in pal)
                    poxy.Add(OxyColor.Parse(p));
            myPallete = new OxyPalette(poxy);
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

        public void seriesChanged()
        {
            int i = 0;
            foreach(var a in axesInfoList)
            {
                Plot.Series[i].IsVisible = a.IsSelected;
                i++; 
            }
            Plot.InvalidatePlot(false);
        }

        public string table { get; set; }
        public string[] headers { get; set; }

        public bool debugMode { get; set; } = false;

        public void toDebugPlot()
        {
            st.Restart();
            int sumPoints = 0, i=0;
            foreach(LineSeries a in CacheController.fillPlotDebug(db, table, headers))
            {
                a.IsVisible = Plot.Series[0].IsVisible;  
                Plot.Series.RemoveAt(0);
                a.Title = headers[i];
                a.Color = myPallete.Colors[i];
                Plot.Series.Add(a);
                sumPoints += a.Points.Count();
                i++;
            }
            sumPoint = sumPoints;
            Plot.InvalidatePlot(true);
            st.Stop();
            Debug.Print("{0}", st.ElapsedMilliseconds / 1000.0);
        }

        Stopwatch st = new Stopwatch();

        public void createPlot()
        {
            st.Restart();

            updTimer?.Stop();

            Plot.Series.Clear();
            Plot.Axes.Clear();

            Plot.Axes.Add(new DateTimeAxis
            {
                StringFormat = "mm:ss.fff",
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = DateTimeAxis.ToDouble(DateTime.FromBinary(0))
            });
            Plot.Axes.First().AxisChanged += delegate
            {
                need_update = true;
            };
            Plot.Axes.Add(new LinearAxis
            {
                Key = "default",
                Position = AxisPosition.Left,
                Angle = 90
            });

            int i = 0, sumPoints = 0;

            dataMin = double.MaxValue;
            dataMax = double.MinValue;

            axesInfoList.Clear();
            foreach (string header in headers)
            {
                Debug.Print("{0}", plotWidth);
                LineSeries a = CacheController.fillPlotFrist(db, table, header, i, plotWidth);
                axesInfoList.Add(new axesInfo { name = header, col = new SolidColorBrush(fromOxyColor(myPallete.Colors[i])), IsSelected=true});

                a.Color = myPallete.Colors[i];
                a.Title = header;
                a.YAxisKey = "default";
                dataMin = Math.Min(dataMin, a.Points.First().X);
                dataMax = Math.Max(dataMax, a.Points.Last().X);
                sumPoints += a.Points.Count();
                Plot.Series.Add(a);
                i++;
            }
            Plot.Axes.First().AbsoluteMaximum = dataMax;
            Plot.Axes.First().AbsoluteMinimum = dataMin;

            sumPoint = sumPoints;

            updTimer = new Timer(500);
            updTimer.Elapsed += delegate { updatePlot(); };
            updTimer.Start();

            st.Stop();
            Debug.Print("{0}", st.ElapsedMilliseconds / 1000.0);
        }

        Color fromOxyColor(OxyColor c)
        {
            return Color.FromRgb(c.R, c.G, c.B);
        }

        OxyPalette myPallete;
        double dataMin = 0;
        double dataMax = 1;
        Timer updTimer;
        public bool need_update { get; set; } = false;

        public void updatePlot()
        {
            if (!need_update) return;
            updTimer.Stop();
            need_update = false;
            if (debugMode) return;
            try
            {
                int i = 0, sumPoints = 0;
                // dataMin -- dataMax :: ???
                // actualMin -- actualMax :: plotWidth
                double plotMax = Plot.Axes.First().ActualMaximum;
                double plotMin = Plot.Axes.First().ActualMinimum;
                double scale = (dataMax - dataMin) / (plotMax - plotMin);

                foreach (string header in headers)
                {
                    LineSeries a = Plot.Series[i] as LineSeries;
                    a.Points.Clear();
                    CacheController.fillPlot(db, a, table, header, i, plotWidth, plotMin, plotMax, scale);
                    dataMin = a.Points.First().X;
                    dataMax = a.Points.Last().X;
                    sumPoints += a.Points.Count();
                    i++;
                }
                sumPoint = sumPoints;
                Plot.InvalidatePlot(true);
            }catch(Exception e)
            {
                Debug.Print(e.Message);
                need_update = true;
            }
            updTimer.Start();
        }

        public void toOneAxis()
        {
            Plot.Axes.Add(new LinearAxis
            {
                Key = "default",
                Position = AxisPosition.Left,
                Angle = 90
            });
            int i = 1;
            foreach(var series in Plot.Series)
            {
                LineSeries a = series as LineSeries;
                a.YAxisKey = "default";
                Plot.Axes.RemoveAt(1);
                i++;
            }
            Plot.InvalidatePlot(false);
        }

        public void toEachAxis()
        {
            int i = 0;
            foreach(var series in Plot.Series)
            {
                LineSeries a = series as LineSeries;
                Plot.Axes.Add(new LinearAxis
                {
                    Key = a.Title + "Y",
                    Position = ((i % 2==0)? AxisPosition.Left : AxisPosition.Right),
                    AxisDistance = (i/2)*30,                    
                    Angle = 90,
                    TicklineColor = a.Color,
                });
                a.YAxisKey = a.Title + "Y";
                i++;
            }
            Plot.Axes.RemoveAt(1);
            Plot.InvalidatePlot(false);
        }

        int _sumPoint;
        public int sumPoint {
            get { return _sumPoint; }
            set { _sumPoint = value; StatusUpdate(); }
        }
        double _plotWidth;
        public double plotWidth {
            get { return _plotWidth; }
            set { _plotWidth = value; StatusUpdate(); }
        }
        double _plotHeight;
        public double plotHeight {
            get { return _plotHeight; }
            set { _plotHeight = value; StatusUpdate(); }
        }
        void StatusUpdate()
        {
            Status = string.Format("Points : {0}, Size : {1} * {2}", sumPoint, plotWidth, plotHeight);
        } 

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
