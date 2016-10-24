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
using System.Threading;

namespace DataReducer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mv;
        private CSVParser csvparser = new CSVParser();

        public MainWindow()
        {
            InitializeComponent();
            mv = DataContext as MainViewModel;
            string app = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if(!Directory.Exists(app + @"\LongVis"))
            {
                Directory.CreateDirectory(app + @"\LongVis");
            }
            Env.root = app + @"\LigVis\";
            Env.dbpath = Env.root + "data.db";
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                Thread t = new Thread(() => csv_open(dlg.FileName));
                t.IsBackground = true;
                t.Start();
            }
        }

        private void csv_open(string path)
        {
            mv.Log_begin("Load " + path);
            csvparser.Open(path);
            mv.Log_end();
        }
    }
}
