using System.Windows;
using System.Diagnostics;

namespace DataVisualizer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mv;

        public MainWindow()
        {
            InitializeComponent();
            mv = DataContext as MainViewModel;
        }

        private void PlotView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement s = sender as FrameworkElement;
            // mv.Plot_width = (int)s.ActualWidth;
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
    }
}
