using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser;

namespace SimpleMLP
{
    public class ClassificationNetwork : INetwork
    {
        public List<Tuple<List<double>, List<double>>> Parse(string path, out int classes, out int attributesCount)
        {
            var data = File.ReadAllLines(path)
                    .Skip(1)
                    .Select(e => e.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries));

            attributesCount = data.First().Length - 1;
            var classesCount = data.Select(e => e[e.Length - 1]).Distinct().Count();
            classes = classesCount;

            var output = data.Select(a =>
            {
                var cls = Int32.Parse(a[2]);
                var outputs = new List<double>(classesCount);
                for (var i = 1; i < cls; i++)
                {
                    outputs.Add((double)0);
                }
                outputs.Add((double)1);
                for (var i = 0; i < classesCount - cls; i++)
                {
                    outputs.Add((double)0);
                }

                return new Tuple<List<double>, List<double>>
                (
                    new List<double>() { Double.Parse(a[0].Replace(".", ",")), Double.Parse(a[1].Replace(".", ",")) },
                    outputs
                );
            })
                .ToList();

            return new CsvData(output).NormalizedData;
        }
    }
}
