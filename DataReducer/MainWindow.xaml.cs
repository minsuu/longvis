using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Data.SQLite;

namespace DataReducer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mv;
        private DBInterface db = new DBInterface();
        private CSVParser csvparser = new CSVParser();
        private bool onload = false;

        public void Table_reload(IAsyncResult res = null)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                mv.Tables.Clear();
                foreach(String t in db.tables())
                {
                    mv.Tables.Add(t);
                }
            }));
            
        }

        public MainWindow()
        {
            InitializeComponent();
            mv = DataContext as MainViewModel;
            Table_reload();
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !onload;
        }

        public delegate void d_csv_open(string path, string name);
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                d_csv_open work = csv_open;
                work.BeginInvoke(dlg.FileName, dlg.SafeFileName, Table_reload, null);
            }
        }

        private void csv_open(string path, string name)
        {
            onload = true;
            mv.Log_begin("Load " + path);
            csvparser.Open(path);
            mv.Log_end();

            name = name.Split('.')[0];
            string s = db.createTable(name, csvparser);
            mv.Log("Created table with " + s);

            using (var timer = new System.Timers.Timer())
            {
                timer.Interval = 500;
                timer.Elapsed += new System.Timers.ElapsedEventHandler((a, b) => mv.Log_update((int)(db.ins_now * 100.0 / db.ins_max)));
                timer.Start();
                mv.Log_begin("Inserting data", false);
                db.insert(name, csvparser);
                mv.Log_end();
                timer.Stop();
            }
            onload = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            db.execute("DROP TABLE " + f_tablelist.SelectedItem);
            db.execute("DELETE FROM sqlite_sequence where name = '" + f_tablelist.SelectedItem + "'");
            Table_reload();
        }
    }
}
