using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataReducer
{
    // date_time,S_1,S_2,S_3,S_4,S_5
    /*
    public class RawData
    {
        public double[] S { get; internal set; }
    }

    public sealed class RawDataMap : CsvClassMap<RawData>
    {
        public RawDataMap()
        {
            Map(x => x.S).ConvertUsing(delegate (ICsvReaderRow row)
            {
                double[] r = new double[5];
                for(int i=0; i<5; i++)
                {
                    r[i] = double.Parse(row.GetField("S_" + (i + 1)));
                }
                return r;
            });
        }
    }*/

    public class CSVParser
    {
        public Dictionary<string, List<double>> raw = new Dictionary<string, List<double>>();
        public string[] headers;
        public int raw_len { get; set; }

        public CSVParser(){
        }

        public void Open(string path)
        {
            raw.Clear();
            using (TextReader textReader = File.OpenText(path))
            {
                var csv = new CsvReader(textReader);
                csv.Read();
                headers = csv.FieldHeaders;
                foreach(string s in headers)
                {
                    raw[s] = new List<double>();
                }

                int idx = 0;
                while (csv.Read())
                {
                    foreach(string s in headers)
                    {
                        if(s == "date_time")
                        {
                            raw[s].Add(idx);
                        }else
                        {
                            raw[s].Add(csv.GetField<double>(s));
                        }
                    }
                    idx++;
                }
                raw_len = idx;
            }
        }
    }
}
