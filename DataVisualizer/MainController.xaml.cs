using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.Specialized;

namespace DataVisualizer
{
    /// <summary>
    /// Interaction logic for MainController.xaml
    /// </summary>
    public partial class MainController : Window
    {
        private MainViewModel mv;

        public MainController()
        {
            InitializeComponent();
            DataContext = mv = MainViewModel.Instance;
        }

        private string oneOFF = "Enable Multi-Axis Mode";
        private string oneON = "Disable Multi-Axis Mode";
        private string debugOFF = "Enable Debug Mode";
        private string debugON = "Disable Debug Mode";

        public void init()
        {
            oneButton.Content = oneOFF;
            debugButton.Content = debugOFF;
            one = false;
            mv.debugMode = false;
        }

        private bool one = false;
        private void oneButton_Click(object sender, RoutedEventArgs e)
        {
            if (!one) {
                mv.toEachAxis();
                oneButton.Content = oneON;
            }
            else
            {
                mv.toOneAxis();
                oneButton.Content = oneOFF;
            }
            one = !one;
        }

        private void debugButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mv.debugMode)
            {
                mv.toDebugPlot();
                debugButton.Content = debugON;
            }else
            {
                debugButton.Content = debugOFF;
            }
            mv.debugMode = !mv.debugMode;
        }

        private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mv.seriesChanged();
        }
    }
}
