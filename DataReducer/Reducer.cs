using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReducer
{
    public static class Reducer
    {
        static double calcDb(double dx, double dy, double dtx, double dty, double r)
        {
            return (dtx == 0) ? dx : (dy * dtx - dx * dty) / r;
        }

        static double calcRb(double dx, double dy, double dtx, double dty, double r, double Db)
        {
            return (dtx == 0) ? dy * dty / Math.Abs(dtx) : (Db * dty + dx * r) / dtx;
        }

        static double getAreaOfTriangle(double ax, double ay, double bx, double by, double cx, double cy)
        {
            return Math.Abs(ax * (by - cy) + bx * (cy - ay) + cx * (ay - by)) / 2;
        }

        static List<int> reduce_RnW(ref List<double> px, ref List<double> py, double[] tolerance)
        {
            return null;
        }

        static List<int> reduce_VnW(ref List<double> px, ref List<double> py, int[] threshold)
        {
            return null;
        }

        static List<int> reduce_MnM(ref List<double> px, ref List<double> py, int[] blocks)
        {
            return null;
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
}
