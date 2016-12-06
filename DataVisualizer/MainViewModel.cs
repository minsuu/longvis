using System;
using System.IO;
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
            
    public class tableInfo
    {
        public int id, sensor;
        public string name, summary, algo, sensornames;
        public DateTime created, accessed;
    }

    public class MainViewModel : INotifyPropertyChanged
    {
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
        }

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
