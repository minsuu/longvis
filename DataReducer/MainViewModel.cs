using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;
using CsvHelper;
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

        private string _step3_pr = "";
        public string step3_pr {
            get { return _step3_pr; }
            set
            {
                _step3_pr = value;
                OnPropertyChanged("step3_pr");
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


        CSVParser currentCSV;

        public int data_len
        {
            get
            {
                return currentCSV.raw_len;
            }
        }
        public void openFile(string path, string name)
        {
            // use name as default table name
            name = name.Split('.')[0];
            tableName = name;
            currentCSV = new CSVParser(path);
            nameSourceCol.Clear();
            bool fst = true;
            foreach(var s in currentCSV.headers)
            {
                nameSourceCol.Add(new nameSource() { oldname = s, newname = s, isTimestamp = fst });
                fst = false;
            }
        }

        public DBInterface db = new DBInterface();
        public void processFile()
        {
            step1_col = Brushes.Black;
            step2_col = Brushes.Black;
            step3_col = Brushes.Black;
            step3_pr = "";
            Task<int> task = new Task<int>(processFile_async);
            task.ContinueWith(processFile_Exception, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }

        private void processFile_Exception(Task<int> task)
        {
            MessageBox.Show(task.Exception.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Stop);
        }

        public int processFile_step;
        private int processFile_async()
        {
            processFile_step = 0;
            // insert currentCSVParser's data into table named 'tableName'
            step1_col = Brushes.Red;
            string[] newname = new string[nameSourceCol.Count];
            bool[] is_time = new bool[nameSourceCol.Count];
            for(int i = 0; i < nameSourceCol.Count; i++)
            {
                newname[i] = nameSourceCol[i].newname;
                is_time[i] = nameSourceCol[i].isTimestamp;
            }
            currentCSV.open(nameSourceCol.ToArray());
            step1_col = Brushes.LightSeaGreen;

            processFile_step++;
            step2_col = Brushes.Red;
            List<long[]> results = new List<long[]>();
            foreach(string s in currentCSV.raw_dataheader)
            {
                long[] result;
                Reducer.reduce_MinMax(currentCSV.rawdata_timestamp, currentCSV.rawdata[s], out result);
                results.Add(result);
            }
            step2_col = Brushes.LightSeaGreen;

            processFile_step++;
            step3_col = Brushes.Red;

            // table creation
            StringBuilder query = new StringBuilder(string.Format("CREATE TABLE {0} ({1}", tableName, Env.dbTableScheme));
            for(int i=0; i<currentCSV.raw_dataheader.Count; i++)
            {
                query.AppendFormat(Env.dbTableAppend, i);
            }
            query.Remove(query.Length - 2, 2);
            query.Append(") ENGINE=MyISAM");
            db.execute(query.ToString());

            Timer a = new Timer(500);
            a.Elapsed += delegate
            {
                step3_pr = db.workProgress.ToString() + "%";
                Debug.Print(step3_pr);
            };

            // data insert
            List<List<double>> datalist = new List<List<double>>();
            foreach(string s in currentCSV.raw_dataheader)
            {
                datalist.Add(currentCSV.rawdata[s]);
            }

            a.Start();
            db.insert(tableName, currentCSV.rawdata_timestamp, datalist, results);
            a.Stop();
            step3_pr = "100%";

            step3_col = Brushes.LightSeaGreen;

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
            // check info table exists.. & update info
            if(!db.table_exists(Env.dbBaseName))
                db.execute(string.Format("CREATE TABLE {0} ({1}) ENGINE=MyISAM", Env.dbBaseName, Env.dbBaseScheme));


            string nowDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
            db.insert(Env.dbBaseName, DBNull.Value, tableName, "default", "min-max", nowDate, nowDate,
                currentCSV.raw_dataheader.Count, string.Join("\n", currentCSV.raw_dataheader));
            processFile_step++;

            return 1;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
