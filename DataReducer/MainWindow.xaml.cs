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
using System.Timers;

namespace DataReducer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mv;

        private int tab1state = 0; // 0, 1, 2
        private int tab2state = 3; // 3
        private int tab3state = 4; // 4

        public MainWindow()
        {
            InitializeComponent();
            mv = DataContext as MainViewModel;
            TabInsertButtonSvg.Opacity = 1;
            TabListButtonSvg.Opacity = 0.5;
            TabSettingButtonSvg.Opacity = 0.5;
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
            }
        }

        private void tabInfoBackButton_Click(object sender, RoutedEventArgs e)
        {
            currParser = null;
            tab1state = 0;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 0));
        }

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
            try
            {
                bool simpletest = mv.db.table_exists(Env.dbBaseName);
            }catch(Exception err)
            {
                MessageBox.Show(err.Message, "에러!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void TabSettingButton_Click(object sender, RoutedEventArgs e)
        {
            TabInsertButtonSvg.Opacity = 0.5;
            TabListButtonSvg.Opacity = 0.5;
            TabSettingButtonSvg.Opacity = 1;
            /*
            TabInsertButtonSvg.Source = new Uri("pack://application:,,,/Resources/new-off.svg");
            TabListButtonSvg.Source = new Uri("pack://application:,,,/Resources/list-off.svg");
            TabSettingButtonSvg.Source = new Uri("pack://application:,,,/Resources/menu-on.svg");
            */
            textDBAddress.Text = Env.dbServerAddress;
            textUserID.Text = Env.dbUid;
            textUserPass.Password = Env.dbPassword;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = tab3state));
        }

        private void TabListButton_Click(object sender, RoutedEventArgs e)
        {
            TabInsertButtonSvg.Opacity = 0.5;
            TabListButtonSvg.Opacity = 1;
            TabSettingButtonSvg.Opacity = 0.5;
            /*
            TabInsertButtonSvg.Source = new Uri("pack://application:,,,/Resources/new-off.svg");
            TabListButtonSvg.Source = new Uri("pack://application:,,,/Resources/list-on.svg");
            TabSettingButtonSvg.Source = new Uri("pack://application:,,,/Resources/menu-off.svg");
            */
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = tab2state));
        }

        private void TabInsertButton_Click(object sender, RoutedEventArgs e)
        {
            TabInsertButtonSvg.Opacity = 1;
            TabListButtonSvg.Opacity = 0.5;
            TabSettingButtonSvg.Opacity = 0.5;

            /*  
            TabInsertButtonSvg.Source = new Uri("pack://application:,,,/Resources/new-on.svg");
            TabListButtonSvg.Source = new Uri("pack://application:,,,/Resources/list-off.svg");
            TabSettingButtonSvg.Source = new Uri("pack://application:,,,/Resources/menu-off.svg");
            */
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = tab1state));
        }

        private void tabProgressBackButton_Click(object sender, RoutedEventArgs e)
        {
            tab1state = 0;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = tab1state));
        }
    }
}
