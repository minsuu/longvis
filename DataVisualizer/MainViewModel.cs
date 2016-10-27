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
    public class BindableMenuItem
    {
        public BindableMenuItem(string _name, BindableMenuItem[] _child, ICommand _command)
        {
            Name = _name;
            Children = _child;
            Command = _command;
        }
        public string Name { get; set; }
        public BindableMenuItem[] Children { get; set; }
        public ICommand Command { get; set; }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private const int BLOCK = 50;
        private const string name = @"..\..\..\Dataset\dat.csv";
        private const int A = 0;
        private const int B = 500000;

        public ObservableCollection<BindableMenuItem> MenuCollection { get; set; } = new ObservableCollection<BindableMenuItem>();
        public RelayCommand opentable { get; set; }
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
        private DBInterface db = new DBInterface();
        public delegate void d_open_table(string name);
        public PlotModel Plot { get; set; } = new PlotModel();
        private LineSeries line;

        public MainViewModel()
        {
            opentable = new RelayCommand(o => Open_table(o.ToString()));
            timer.Interval = 500;
            timer.Elapsed += new ElapsedEventHandler((a, b) => Log_update());
            foreach (string t in db.tables())
            {
                MenuCollection.Add(new BindableMenuItem(t, null, opentable));
            }
            line = new LineSeries();
            line.Title = "line";
            Plot.Series.Add(line);
            Plot.LegendTitle = "Legend";
        }

        public void Open_table(string name)
        {
            d_open_table work = open_table;
            work.BeginInvoke(name, null, null);
        }
        
        private int load_max { get; set; } = 1;
        private int load_now { get; set; } = 0;
        public void open_table(string name)
        {
            Log_begin("loading " + name, false);
            Plot.Title = name;
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
            Log_end();
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

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
