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
    public class nameSource
    {
        public string oldname { get; set; }
        public string newname { get; set; }
        public bool isTimestamp { get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            Tables = new ObservableCollection<string>();
            /*
                        timer.Interval = 500;
                        timer.Elapsed += new ElapsedEventHandler((a,b) => Log_update());
            */
            nameSourceCol.Add(new nameSource { oldname = "1", newname = "2", isTimestamp = true });
        }

        private string _tableName;
        public string tableName {
            get { return _tableName; }
            set
            {
                _tableName = value;
                OnPropertyChanged("tableName");
            }
        }

        private ObservableCollection<nameSource> _nameSourceCol = new ObservableCollection<nameSource>();
        public ObservableCollection<nameSource> nameSourceCol
        {
            get { return _nameSourceCol; }
            set
            {
                _nameSourceCol = value;
                OnPropertyChanged("nameSourceCol");
            }
        }

        CSVParser currentCSVParser;

        public void openFile(string path, string name)
        {
            // use name as default table name
            name = name.Split('.')[0];
            tableName = name;
            currentCSVParser = new CSVParser(path);
            nameSourceCol.Clear();
            foreach(var s in currentCSVParser.headers)
            {
                nameSourceCol.Add(new nameSource() { oldname = s, newname = s, isTimestamp = false });
            }
        }

/*
        Stopwatch sw = new Stopwatch();
        Timer timer = new Timer();
        private string _log_past;
        public void Log(string s)
        {
            _log_past += s + "\n";
            Logger = _log_past;
        }

        public void Log_begin(string s, bool t = true)
        {
            sw.Restart();
            if(t)
                timer.Start();
            _log_past += s;
            Logger = _log_past;
        }

        public void Log_update(int per)
        {
            Logger = _log_past + " (" + per + "%)";
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
*/
        public ObservableCollection<string> Tables { get; set;}
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
