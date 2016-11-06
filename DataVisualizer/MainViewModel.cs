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
        public PlotModel Plot { get; set; } = new PlotModel();

        public MainViewModel()
        {
            opentable = new RelayCommand(o => Open_clicked(o.ToString()));
            timer.Interval = 500;
            timer.Elapsed += new ElapsedEventHandler((a, b) => Log_update());
            foreach (string t in db.tables())
            {
                MenuCollection.Add(new BindableMenuItem(t, null, opentable));
            }
            Plot.LegendTitle = "Legend";
        }

        public delegate void d_cache_update();
        public CacheController cc;
        public void cache_update()
        {
            Log_begin("loading " + cc.name, false);
            
            Log_end();
        }

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

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
