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

namespace DataReducer
{
    using CsvHelper;
    using CsvHelper.Configuration;

    // using DataPoint = List<Double>;

    public sealed class DataPoint
    {
        public string date_time { set; get; }
        public double index { set; get; }
        public double[] values { set; get; }
        public List<bool> mask { set; get; }

        public DataPoint(string dt, double index, double[] val)
        {
            this.date_time = dt;
            this.index = index;
            this.values = val;
            this.mask = new List<bool>();
        }

        public override string ToString()
        {
            return String.Format("DataPoint({0}, {1}, {2})", date_time, index, values);
        }
    }

    class Stripper
    {
        static double calcDb(double dx, double dy, double dtx, double dty, double r)
        {
            return (dtx == 0) ? dx : (dy * dtx - dx * dty) / r;
        }

        static double calcRb(double dx, double dy, double dtx, double dty, double r, double Db)
        {
            return (dtx == 0) ? dy * dty / Math.Abs(dtx) : (Db * dty + dx * r) / dtx;
        }

        static void stripRnW(ref List<DataPoint> pList, out List<DataPoint> qList, int i, double tolerance)
        {
            int a, b, n;
            a = b = 0;
            n = pList.Count();

            double dtx, dty, dx, dy, r, Db;

            // Step (i)
            qList = new List<DataPoint>() {
                pList.First()
            };

            // Step (ii)
            while (a < n)
            {
                // Step (1)
                dtx = pList[a + 1].index - pList[a].index;
                dty = pList[a + 1].values[i] - pList[a].values[i];
                r = Math.Sqrt(dtx * dtx + dty * dty);

                // Step (2)
                for (b = a + 2; b < n; b++)
                {
                    dx = pList[b].index - pList[a].index;
                    dy = pList[b].values[i] - pList[a].values[i];
                    Db = calcDb(dx, dy, dtx, dty, r);

                    if (Math.Abs(Db) > tolerance)
                    {
                        qList.Add(pList[b - 1]);
                        a = b - 1;
                        break;
                    }
                }

                if (b == n)
                {
                    qList.Add(pList.Last());
                    a = n;
                }
            }
        }

        static void stripESA(ref List<DataPoint> pList, out List<DataPoint> qList, int i, double d, double ef = 0)
        {
            int a, b, f, m, n;
            a = b = f = m = 0;
            n = pList.Count();

            double dx, dy, dtx, dty;
            double r, rf, rb, rmax;
            double tsq = (d + ef) * (d + ef);
            double Db;

            // Step (i)
            qList = new List<DataPoint>() {
                pList.First()
            };

            // Step (ii)
            while (a < n)
            {
                r = dtx = dty = 0;

                // Step (1)
                for (f = a + 1; f < n; f++)
                {
                    dtx = pList[f].index - pList[a].index;
                    dty = pList[f].values[i] - pList[a].values[i];
                    r = dtx * dtx + dty * dty;

                    if (r > tsq)
                        break;
                }

                // Step (2)
                r = Math.Sqrt(r);
                rf = (dtx == 0) ? Math.Abs(dty) : r;
                rmax = rf;
                m = f;

                // Step (3)
                for (b = f + 1; b < n; b++)
                {
                    dx = pList[b].index - pList[a].index;
                    dy = pList[b].values[i] - pList[a].values[i];
                    Db = calcDb(dx, dy, dtx, dty, r);
                    rb = calcRb(dx, dy, dtx, dty, r, Db);

                    // Step (3.1)
                    if (Math.Abs(Db) > d)
                    {
                        qList.Add(pList[b - 1]);
                        a = b - 1;
                        break;
                    }
                    else if (rb < rmax - d)
                    {
                        qList.Add(pList[m]);
                        a = m;
                        break;
                    }

                    if (rb > rmax)
                    {
                        rmax = rb;
                        m = b;
                    }
                }

                // Step (4)
                if (b >= n)
                {
                    qList.Add(pList.Last());
                    a = n;
                }
            }
        }

        static double getAreaOfTriangle(double ax, double ay, double bx, double by, double cx, double cy)
        {
            return Math.Abs(ax * (by - cy) + bx * (cy - ay) + cx * (ay - by)) / 2;
        }

        static void stripVnW(ref List<DataPoint> pList, out List<DataPoint> qList, int i, double threshold)
        {
            Debug.Assert(threshold >= 0);

            int a, n;
            a = 1;
            n = pList.Count();
            double area;
            qList = new List<DataPoint>() {
                pList.First()
            };

            for (a = 1; a < n - 1; a++)
            {
                if (pList[a - 1].values[i] == pList[a].values[i] && pList[a].values[i] == pList[a + 1].values[i])
                    continue;

                area = getAreaOfTriangle(
                    pList[a - 1].index, pList[a - 1].values[i],
                    pList[a].index, pList[a].values[i],
                    pList[a + 1].index, pList[a + 1].values[i]
                );

                if (area > threshold)
                {
                    qList.Add(pList[a]);
                }
            }

            qList.Add(pList.Last());
        }

        //public static List<DataPoint> getStripData(int type = 1, double d = 1, double ef = 1, double threshold = 0)
        //{
        //    int c, n;
        //    Stopwatch sw = new Stopwatch();
        //    List<DataPoint> qList, pList, rList = null;

        //    readData(out pList, DATA_PATH + FILE_NAME);
        //    c = pList.First().Count();
        //    n = pList.Count();

        //    // Hybrid
        //    Console.WriteLine();
        //    Console.WriteLine("Start Benchmark for Hybrid Algorithm for d={0} and EF={1}", d, ef);

        //    for (int i = 0; i < c; i++)
        //    {
        //        qList = null;
        //        Console.WriteLine("  Striping {0}-th sensor data...", i + 1);

        //        sw.Reset();
        //        sw.Start();
        //        stripVnW(ref pList, out qList, i, 0);
        //        stripESA(ref qList, out rList, i, d, ef);
        //        sw.Stop();

        //        Console.WriteLine("    Reduced Rate: {0} / {1} ({2:00.000} %)", rList.Count(), pList.Count(), (double)rList.Count() * 100 / (double)n);
        //        Console.WriteLine("    Elapsed Time: {0}", sw.Elapsed);
        //    }

        //    return pList;
        //}
    }

    public class FilePath
    {
        public const string input = @"..\..\..\Dataset\dat.csv";
        public const string output = @"..\..\..\Dataset\test.csv";
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<DataPoint> Points;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.DoItButton.Click += processCsv;
        }

        private void processCsv(object sender, RoutedEventArgs e)
        {
            int i, j;
            this.Points = new List<DataPoint>();

            using (TextReader textReader = File.OpenText(FilePath.input))
            {
                var csv = new CsvReader(textReader);
                List<double> vals;
                i = 0;

                while (csv.Read())
                {
                    vals = new List<double>();
                    for (j = 1; j <= 5; j++)
                        vals.Add(csv.GetField<double>(j));
                    this.Points.Add(new DataPoint(csv.GetField<string>("date_time"), i++, vals.ToArray()));
                }
            }

            using (TextWriter textWriter = File.CreateText(FilePath.output))
            {
                var csv = new CsvWriter(textWriter);
                foreach (var item in this.Points)
                {
                    csv.WriteField(item.date_time);
                    csv.WriteField(item.index);
                    foreach (var val in item.values)
                        csv.WriteField(val);
                    csv.NextRecord();
                }
            }

            MessageBoxResult result = MessageBox.Show("Done!");
        }
    }
}
