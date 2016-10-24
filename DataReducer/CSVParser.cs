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
    public class RawData
    {
        // date_time,S_1,S_2,S_3,S_4,S_5
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
    }

    class CSVParser
    {
        public List<RawData> raw = new List<RawData>();

        public CSVParser(){
        }

        public void Open(string path)
        {
            raw.Clear();
            using (TextReader textReader = File.OpenText(path))
            {
                var csv = new CsvReader(textReader);
                csv.Read();
                var headers = csv.FieldHeaders;

                csv.Configuration.RegisterClassMap(new RawDataMap());
                raw = csv.GetRecords<RawData>().ToList();
            }
        }
    }
}
