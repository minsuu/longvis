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
        public static LineSeries fillPlotFrist(DBInterface db, string table, string header, int i, double plotWidth)
        {
            LineSeries a = new LineSeries();
            string q = "SELECT TS, S{1} FROM {0} WHERE Q{1} <= {2} ORDER BY TS";
            string s = string.Format(q, table, i, plotWidth / 3);
            DataTable r = db.getDataTable(s);
            foreach(var row in r.AsEnumerable())
            {
                DateTime ts = DateTime.FromBinary(Convert.ToInt64(row["TS"]));
                a.Points.Add(new DataPoint(DateTimeAxis.ToDouble(ts),
                                           Convert.ToDouble(row[string.Format("S{0}", i)])));
            }
            return a;
        }

        public static void fillPlot(DBInterface db, LineSeries a, string table, string header, int i, double plotWidth, double plotMin, double plotMax, double scale)
        {
            long plotMinQ = DateTimeAxis.ToDateTime(plotMin).Ticks;
            long plotMaxQ = DateTimeAxis.ToDateTime(plotMax).Ticks;
            long plotRange = plotMaxQ - plotMinQ;
            plotMinQ -= plotRange / 2;
            plotMaxQ += plotRange / 2;
            string q = "SELECT TS, S{1} FROM {0} WHERE Q{1} <= {2} OR (TS BETWEEN {3} AND {4} AND Q{1} <= {5}) ORDER BY TS";
            string s = string.Format(q, table, i, plotWidth / 3, plotMinQ, plotMaxQ, plotWidth * scale / 3);
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