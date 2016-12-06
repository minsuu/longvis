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
using System.Timers;

namespace DataReducer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mv;
        private bool onload = false;

        private int tab1state = 0; // 0, 1, 2
        private int tab2state = 3; // 3
        private int tab3state = 4; // 4

        /*
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
            
        }*/

        public MainWindow()
        {
            InitializeComponent();
            mv = DataContext as MainViewModel;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !onload;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        /*
        private void csv_open(string path, string name)
        {
            onload = true;
            csvparser.Open();

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
        */

        private void Button_Click(object sender, RoutedEventArgs e)
        {
//            db.execute("DROP TABLE " + f_tablelist.SelectedItem);
//            db.execute("DELETE FROM sqlite_sequence where name = '" + f_tablelist.SelectedItem + "'");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }

        private void buttonWindowMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void buttonWindowMax_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void buttonWindowClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        CSVParser currParser;
        private void buttonFileOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                tab1state = 1;
                Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 1));
                currParser = new CSVParser(dlg.FileName);
                mv.openFile(dlg.FileName, dlg.SafeFileName);
                /*
                d_csv_open work = csv_open;
                work.BeginInvoke(dlg.FileName, dlg.SafeFileName, Table_reload, null);
                */
            }
        }

        private void tabInfoBackButton_Click(object sender, RoutedEventArgs e)
        {
            currParser = null;
            tab1state = 0;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 0));
        }

        Timer timer;
        private void tabInfoSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mv.db.table_exists(textTableName.Text))
                {
                    MessageBox.Show("이미 존재하는 테이블명입니다.", "에러!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                tab1state = 2;
                Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 2));
                mv.processFile();
            }catch(Exception err)
            {
                MessageBox.Show(err.Message, "에러!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void SettingSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Env.dbServerAddress = textDBAddress.Text;
            Env.dbUid = textUserID.Text;
            Env.dbPassword = textUserPass.Password;
        }

        private void TabSettingButton_Click(object sender, RoutedEventArgs e)
        {
            TabInsertButtonSvg.Source = new Uri("pack://application:,,,/Resources/new-off.svg");
            TabListButtonSvg.Source = new Uri("pack://application:,,,/Resources/list-off.svg");
            TabSettingButtonSvg.Source = new Uri("pack://application:,,,/Resources/menu-on.svg");
            textDBAddress.Text = Env.dbServerAddress;
            textUserID.Text = Env.dbUid;
            textUserPass.Password = Env.dbPassword;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = tab3state));
        }

        private void TabListButton_Click(object sender, RoutedEventArgs e)
        {
            TabInsertButtonSvg.Source = new Uri("pack://application:,,,/Resources/new-off.svg");
            TabListButtonSvg.Source = new Uri("pack://application:,,,/Resources/list-on.svg");
            TabSettingButtonSvg.Source = new Uri("pack://application:,,,/Resources/menu-off.svg");
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = tab2state));
        }

        private void TabInsertButton_Click(object sender, RoutedEventArgs e)
        {
            TabInsertButtonSvg.Source = new Uri("pack://application:,,,/Resources/new-on.svg");
            TabListButtonSvg.Source = new Uri("pack://application:,,,/Resources/list-off.svg");
            TabSettingButtonSvg.Source = new Uri("pack://application:,,,/Resources/menu-off.svg");
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = tab1state));
        }
    }
}
