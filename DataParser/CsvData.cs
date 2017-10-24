using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class CsvData
    {
        List<Tuple<List<double>, List<double>>> data;
        List<Tuple<List<double>, List<double>>> normalizedData;
        List<Tuple<double, double>> attributeSpansAndShifts;

        public List<Tuple<List<double>, List<double>>> NormalizedData => this.normalizedData;
        public List<Tuple<List<double>, List<double>>> RawData => this.data;


        public CsvData() { }
        public CsvData(List<Tuple<List<double>, List<double>>> data)
        {
            this.data = data;
            var e = data.First();
            this.attributeSpansAndShifts = new List<Tuple<double, double>>(e.Item1.Count);
            for(var i = 0; i < e.Item1.Count; i++)
            {
                var min = data.Min(x => x.Item1[i]);
                var max = data.Max(x => x.Item1[i]);
                this.attributeSpansAndShifts.Add(new Tuple<double, double>(max - min, min));
            }
            this.normalizedData = this.normalizeData();


        }

        private List<Tuple<List<double>, List<double>>> normalizeData()
            => this.data.Select
               (
                    e => new Tuple<List<double>, List<double>>
                    (
                        e.Item1.Zip(this.attributeSpansAndShifts, (f, info) => (2 * (f - info.Item2) / info.Item1) - 1).ToList(),
                        e.Item2
                    )
                ).ToList();
    }
}
