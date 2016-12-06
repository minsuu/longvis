using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Diagnostics;

namespace DataReducer
{
    public class CSVParser
    {
        public int raw_len { get; set; }
        public List<string> raw_dataheader { get; set; }
        public string raw_timestamp { get; set; }
        public Dictionary<string, List<double>> rawdata { get; set; } = new Dictionary<string, List<double>>();

        public string[] headers { get; set; }

        // woking progress. 0~100.
        public double workProgress { get; set; }

        private string path;
        public CSVParser(string path){
            this.path = path;
            using (TextReader textReader = File.OpenText(path))
            {
                var csv = new CsvReader(textReader);
                csv.Read();
                headers = csv.FieldHeaders;
            }
        }

        private static readonly DateTime Epoch = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public bool open(nameSource[] namesource)
        {
            using (TextReader textReader = File.OpenText(path))
            {
                var csv = new CsvReader(textReader);
                csv.Read();

                raw_dataheader = new List<string>();
                for(int i=0;i<namesource.Length;i++)
                {
                    rawdata[namesource[i].newname] = new List<double>();
                    if (namesource[i].isTimestamp)
                        raw_timestamp = namesource[i].newname;
                    else
                        raw_dataheader.Add(namesource[i].newname);
                }

                int len = 0;
                while (csv.Read())
                {
                    for(int i = 0; i < namesource.Length; i++)
                    {
                        double ins = 0;
                        if (namesource[i].isTimestamp)
                        {
                            string time = csv.GetField<string>(namesource[i].oldname);
                            DateTime tmp;
                            if(DateTime.TryParse(time, out tmp) ||
                                DateTime.TryParseExact(time, Env.dateFormats.ToArray(), null, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out tmp))
                            {
                                ins = (tmp - Epoch).TotalMilliseconds;
                            }else
                            {
                                return false;
                            }
                        }else
                        {
                            ins = csv.GetField<double>(headers[i]);
                        }
                        rawdata[namesource[i].newname].Add(ins);
                    }
                    len++;
                }
                raw_len = len;
            }
            return true;
        }
    }
}
