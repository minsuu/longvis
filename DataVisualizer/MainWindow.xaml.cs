using System;
using System.Windows;
using System.Diagnostics;
using OxyPlot;

namespace DataVisualizer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mv;
        private MainController mc;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mv = MainViewModel.Instance;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 1));
            TabViewButtonSvg.Opacity = 0.5;
            TabSettingButtonSvg.Opacity = 0.5;
            TabListButtonSvg.Opacity = 1;

            myPlotView.Controller = new myPlotController();
            myPlotCommands.mw = this;
            Loaded += delegate
            {
                mv.plotWidth = ActualWidth;
                mv.plotHeight = ActualHeight;               
            };
        }
        
        public void onUpdate()
        {
            mv.need_update = true;
        }

        private void myPlotView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement s = sender as FrameworkElement;
            mv.plotHeight = s.ActualHeight;
            mv.plotWidth = s.ActualWidth; 
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

        private void SettingSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Env.dbServerAddress = textDBAddress.Text;
            Env.dbUid = textUserID.Text;
            Env.dbPassword = textUserPass.Password;
            try
            {
                mv.getTableInfo();
            }catch(Exception err)
            {
                MessageBox.Show(err.Message, "에러!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tableListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            tableInfo x = tableListView.SelectedItem as tableInfo;
            if(x != null)
            {
                char[] delim = {'\n'};
                mv.table = x.name;
                mv.headers = x.sensornames.Split(delim);
                mv.createPlot();
                TabViewButtonSvg.Opacity = 1;
                TabSettingButtonSvg.Opacity = 0.5;
                TabListButtonSvg.Opacity = 0.5;
                showMC();
                Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 0));
            }
        }

        private void showMC()
        {
            mc = new MainController();
            mc.ShowInTaskbar = false;
            mc.Owner = Application.Current.MainWindow;
            mc.Show();
        }

        private void TabViewButton_Click(object sender, RoutedEventArgs e)
        {
            TabViewButtonSvg.Opacity = 1;
            TabSettingButtonSvg.Opacity = 0.5;
            TabListButtonSvg.Opacity = 0.5;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 0));
        }

        private void TabListButton_Click(object sender, RoutedEventArgs e)
        {
            TabViewButtonSvg.Opacity = 0.5;
            TabSettingButtonSvg.Opacity = 0.5;
            TabListButtonSvg.Opacity = 1;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 1));
        }

        private void TabSettingButton_Click(object sender, RoutedEventArgs e)
        {
            textDBAddress.Text = Env.dbServerAddress;
            textUserID.Text = Env.dbUid;
            textUserPass.Password = Env.dbPassword;
            TabViewButtonSvg.Opacity = 0.5;
            TabSettingButtonSvg.Opacity = 1;
            TabListButtonSvg.Opacity = 0.5;
            Dispatcher.BeginInvoke((Action)(() => mainTabControl.SelectedIndex = 2));
        }

    }
}
