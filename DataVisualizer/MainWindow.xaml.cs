using System.Windows;

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
            mv = this.DataContext as MainViewModel;
        }
    }
}
