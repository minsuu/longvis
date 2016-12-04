using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows.Media;

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


        /* notify change variables!! */

        private Brush _step1_col = Brushes.Black;
        public Brush step1_col {
            get { return _step1_col; }
            set
            {
                _step1_col = value;
                OnPropertyChanged("step1_col");
            }
        }

        private Brush _step2_col = Brushes.Black;
        public Brush step2_col {
            get { return _step2_col; }
            set
            {
                _step2_col = value;
                OnPropertyChanged("step2_col");
            }
        }

        private Brush _step3_col = Brushes.Black;
        public Brush step3_col {
            get { return _step3_col; }
            set
            {
                _step3_col = value;
                OnPropertyChanged("step3_col");
            }
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

        /* ------------------------ */


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

        private DBInterface db = new DBInterface();
        public void processFile()
        {
            d_process_file work = processFile_async;
            work.BeginInvoke(null, null);
        }

        private delegate void d_process_file();
        private void processFile_async()
        {
            // insert currentCSVParser's data into table named 'tableName'

            step1_col = Brushes.Red;
            string[] newname = new string[nameSourceCol.Count];
            bool[] is_time = new bool[nameSourceCol.Count];
            for(int i = 0; i < nameSourceCol.Count; i++)
            {
                newname[i] = nameSourceCol[i].newname;
                is_time[i] = nameSourceCol[i].isTimestamp;
            }
            currentCSVParser.open(nameSourceCol.ToArray());
            for(int i = 0; i < 10; i++)
            {
                Debug.Print(currentCSVParser.rawdata["date_time"][i].ToString());
            }
            step1_col = Brushes.LightSeaGreen;

            step2_col = Brushes.Red;
            step2_col = Brushes.LightSeaGreen;
            
        }

        // create table!!
        /*
        public string createTable(string tableName, CSVParser csv)
        {
            StringBuilder q = new StringBuilder(string.Format(@"CREATE TABLE `{0}`(", tableName));
            q.Append(Env.dbTableScheme);
            foreach(string h in csv.headers)
            {
                q.Append(h + " DOUBLE, ");
            }
            q.Remove(q.Length - 2, 2);
            q.Append(")");
            execute(q.ToString());
            return q.ToString();
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
