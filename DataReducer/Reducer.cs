using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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


        public static void reduce_MinMax(List<long> px, List<double> py, out long[] result)
        {
            result = new long[px.Count];
            index = 0; len = px.Count;

            // for duplicated timestamp...
            jumpTable = new int[px.Count];
            jumpEntry = new reduce_Entry[px.Count];
            for(int j = 0; j < px.Count;)
            {
                int i = j++;
                jumpEntry[i] = new reduce_Entry(i, i, i, i);
                while(j!=px.Count && px[j] == px[i])
                {
                    if (py[jumpEntry[i].maxp] < py[j])
                        jumpEntry[i].maxp = j;
                    if (py[jumpEntry[i].minp] > py[j])
                        jumpEntry[i].minp = j;
                    j++;
                }
                jumpEntry[i].end = j-1;
                jumpTable[i] = j;
            }

            try
            {
                reduce_MinMax_rec(px.First(), px.Last(), 0, px, py, ref result);
            }catch(Exception e)
            {
                Debug.Print(e.ToString());
            }
        }
        
        // woking progress. 0~100.
        public static double workProgress {
            get
            {
                return (double)index / len * 100;
            }
        }

        private struct reduce_Entry
        {
            public int begin, end, maxp, minp;
            public reduce_Entry(int _begin, int _end, int _maxp, int _minp)
            {
                begin = _begin; end = _end; maxp = _maxp; minp = _minp;
            }
        }
        private static reduce_Entry reduce_EntryNull = new reduce_Entry(-1,-1,-1,-1);
        private static int index;
        private static int len;
        private static int[] jumpTable;
        private static reduce_Entry[] jumpEntry;

        /// <summary>
        ///     [leftBound, rightBound]의 구간에서 index로부터 출발해서 result에다 값을 할당한다
        /// </summary>
        /// <param name="leftBound">좌측 한계</param>
        /// <param name="rightBound">우측 한계</param>
        /// <param name="index">시작 index</param>
        /// <param name="px">x좌표 값들</param>
        /// <param name="py">y좌표 값들</param>
        /// <param name="result">결과 bitmask</param>
        private static reduce_Entry reduce_MinMax_rec(long leftBound, long rightBound, int depth, List<long> px, List<double> py, ref long[] result)
        {
            if (jumpTable[index] == px.Count || rightBound < px[jumpTable[index]]) {
                reduce_Entry ret = jumpEntry[index];
                for(int i=index;i<jumpTable[index];i++)
                    result[i] = 1 << depth;
                index = jumpTable[index];
                return ret;
            }
            
            long mid = leftBound/2 + rightBound/2 + (leftBound%2 + rightBound%2)/2;
            reduce_Entry left = reduce_MinMax_rec(leftBound, mid, depth+1, px, py, ref result);
            reduce_Entry right = reduce_MinMax_rec(mid, rightBound, depth+1, px, py, ref result);
            Debug.Assert(left.begin != -1);
            Debug.Assert(right.begin != -1);

            result[left.begin] = 1 << depth;
            result[right.end] = 1 << depth;
            bool maxb = py[left.maxp] < py[right.maxp];
            bool minb = py[left.minp] < py[right.minp];
            result[maxb ? left.maxp : right.maxp] = 1 << depth;
            result[minb ? right.minp : left.minp] = 1 << depth;
            return new reduce_Entry(left.begin, right.end, maxb ? right.maxp : left.maxp, minb ? left.minp : right.minp);
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
