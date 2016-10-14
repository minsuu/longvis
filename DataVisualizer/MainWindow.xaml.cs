using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataVisualizer
{
    using System.Collections.Generic;

    using OxyPlot;
    using CsvHelper;

    public class MainViewModel
    {
        private const int BLOCK = 50;
        private const string name = @"..\..\..\Dataset\dat.csv";
        private const int A = 0;
        private const int B = 500000;

        public void toReduced()
        {
            Points.Clear();

            var csv = new CsvReader(File.OpenText(name));
            int i = 1;
            DataPoint mx, mn, dmx, dmn;
            dmx = mx = new DataPoint(0, double.MinValue);
            dmn = mn = new DataPoint(0, double.MaxValue);

            while (csv.Read())
            {
                if (!(A <= i && i <= B)) { i++; continue; }
                double y = csv.GetField<double>("S_1");
                if (i % BLOCK == 0) // block의 시작점
                {
                    Points.Add(new DataPoint(i, y));
                }
                else if (i % BLOCK == BLOCK - 1) // block의 끝점
                {
                    if (mx.X < mn.X)
                    {
                        Points.Add(mx);
                        Points.Add(mn);
                    }
                    else
                    {
                        Points.Add(mn);
                        Points.Add(mx);
                    }
                    Points.Add(new DataPoint(i, y));
                    mx = dmx; mn = dmn;
                }
                else // 그 사이점 :: min/max
                {
                    if (mx.Y < y) mx = new DataPoint(i, y);
                    if (mn.Y > y) mn = new DataPoint(i, y);
                }
                i++;
            }
            OriginT = false;
        }
        public void toOrigin()
        {
            Points.Clear();

            var csv = new CsvReader(File.OpenText(name));
            int i = 0;

            while (csv.Read())
            {
                if (!(A <= i && i <= B)) { i++; continue; }
                double y = csv.GetField<double>("S_1");
                Points.Add(new DataPoint(i++, y));
            }
            OriginT = true;
        }
        public MainViewModel()
        {
            this.Title = "Example 2";
            this.Points = new List<DataPoint>();
            this.OriginT = false;
            // toReduced();
            toOrigin();
        }

        public bool OriginT { get; private set; }
        public string Title { get; private set; }
        public IList<DataPoint> Points { get; private set; }
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //         myLine.Color = conv(OxyColors.LightSkyBlue);
        }

        private Color conv(OxyColor x)
        {
            return Color.FromArgb(x.A, x.R, x.G, x.B);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mv = DataContext as MainViewModel;
            if (mv.OriginT)
            {
                mv.toReduced();
                //               myLine.Color = conv(OxyColors.Aquamarine);
            }
            else
            {
                mv.toOrigin();
                //              myLine.Color = conv(OxyColors.Black);
            }
            Plot1.InvalidatePlot(true);
            lStatus.Content = mv.Points.Count;
        }
    }
}
