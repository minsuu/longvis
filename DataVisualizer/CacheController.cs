using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using CsvHelper;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;
using System.Data;
using OxyPlot;
using OxyPlot.Axes;

namespace DataVisualizer
{
    public static class CacheController
    {
        /// <summary>
        /// Debug모드의 Plot data를 로드한다. 즉 Q값에 상관없이 모든 데이터 로드
        /// </summary>
        /// <param name="db">db인터페이스</param>
        /// <param name="table">테이블명</param>
        /// <param name="headers">칼럼명</param>
        /// <returns></returns>
        public static List<LineSeries> fillPlotDebug(DBInterface db, string table, string[] headers)
        {
            string q = "SELECT * FROM {0} ORDER BY TS";
            string s = string.Format(q, table);
            DataTable r = db.getDataTable(s);

            List<LineSeries> ret = new List<LineSeries>();
            foreach (var h in headers)
                ret.Add(new LineSeries());

            foreach(var row in r.AsEnumerable())
            {
                DateTime ts = DateTime.FromBinary(Convert.ToInt64(row["TS"]));
                for(int i=0; i<headers.Length;i++)
                    ret[i].Points.Add(new DataPoint(DateTimeAxis.ToDouble(ts),
                                           Convert.ToDouble(row[string.Format("S{0}", i)])));
            }
            return ret;
        }

        /// <summary>
        /// x <= Q = 2^k인 Q값을 구해서 리턴한다
        /// </summary>
        /// <param name="x">그려야 할 영역의 width</param>
        /// <returns></returns>
        private static long minQ(double x)
        {
            long r = 1;
            while (r < x) r *= 2;
            return r*2;
        }

        /// <summary>
        /// Plot Width와 1:1의 Plot을 로드한다. 처음에 x축의 min/max가 파악되지 않았을때 사용된다
        /// </summary>
        /// <param name="db">db인터페이스</param>
        /// <param name="table">테이블명</param>
        /// <param name="header">칼럼명</param>
        /// <param name="i">칼럼인덱스</param>
        /// <param name="plotWidth">그려질 plot control의 width</param>
        /// <returns></returns>
        public static LineSeries fillPlotFrist(DBInterface db, string table, string header, int i, double plotWidth)
        {
            LineSeries a = new LineSeries();
            string q = "SELECT TS, S{1} FROM {0} WHERE Q{1} <= {2} ORDER BY TS";
            string s = string.Format(q, table, i, minQ(plotWidth));
            DataTable r = db.getDataTable(s);
            foreach(var row in r.AsEnumerable())
            {
                DateTime ts = DateTime.FromBinary(Convert.ToInt64(row["TS"]));
                a.Points.Add(new DataPoint(DateTimeAxis.ToDouble(ts),
                                           Convert.ToDouble(row[string.Format("S{0}", i)])));
            }
            return a;
        }

        /// <summary>
        /// 주어진 scale의 Plot을 로드한다. 바깥 영역은 1:1을 기준으로, 지정한 영역안에서는 scale에 맞게 로드한다.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="a"></param>
        /// <param name="table"></param>
        /// <param name="header"></param>
        /// <param name="i"></param>
        /// <param name="plotWidth"></param>
        /// <param name="plotMin"></param>
        /// <param name="plotMax"></param>
        /// <param name="scale"></param>
        public static void fillPlot(DBInterface db, LineSeries a, string table, string header, int i, double plotWidth, double plotMin, double plotMax, double scale)
        {
            long plotMinQ = DateTimeAxis.ToDateTime(plotMin).Ticks;
            long plotMaxQ = DateTimeAxis.ToDateTime(plotMax).Ticks;
            long plotRange = plotMaxQ - plotMinQ;
            plotMinQ -= plotRange / 2;
            plotMaxQ += plotRange / 2;
            string q = "SELECT TS, S{1} FROM {0} WHERE Q{1} <= {2} OR (TS BETWEEN {3} AND {4} AND Q{1} <= {5}) ORDER BY TS";
            string s = string.Format(q, table, i, minQ(plotWidth), plotMinQ, plotMaxQ, minQ(plotWidth * scale));
            Debug.Print(s);
            DataTable r = db.getDataTable(s);
            foreach(var row in r.AsEnumerable())
            {
                DateTime ts = DateTime.FromBinary(Convert.ToInt64(row["TS"]));
                a.Points.Add(new DataPoint(DateTimeAxis.ToDouble(ts),
                                           Convert.ToDouble(row[string.Format("S{0}", i)])));
            }
        }
    }
}