using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;

namespace DataReducer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            Tables = new ObservableCollection<string>();
            timer.Interval = 500;
            timer.Elapsed += new ElapsedEventHandler((a,b) => Log_update());
        }

        private string _Logger;
        public string Logger {
            get { return _Logger; }
            set
            {
                _Logger = value;
                OnPropertyChanged("Logger");
            }
        }

        Stopwatch sw = new Stopwatch();
        Timer timer = new Timer();
        private string _log_past;
        public void Log_begin(string s)
        {
            sw.Restart();
            timer.Start();
            _log_past += s;
            Logger = _log_past;
        }

        public void Log_update()
        {
            _log_past += ".";
            Logger = _log_past;
        }

        public void Log_end()
        {
            timer.Stop();
            sw.Stop();
            _log_past += "\nCompleted! Time elapsed : " + sw.ElapsedMilliseconds/1000.0 + "s\n";
            Logger = _log_past;
        }

        public ObservableCollection<string> Tables { get; set;}
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
